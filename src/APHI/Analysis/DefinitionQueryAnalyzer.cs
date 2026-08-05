using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Core.Data;
using APHI.Core.Models;
using APHI.Core.Interfaces;

namespace APHI.Analysis;

/// <summary>
/// Analyzes definition queries on feature layers to identify invalid SQL, missing fields,
/// or queries that return no records.
/// </summary>
public class DefinitionQueryAnalyzer : IAnalyzer
{
    /// <inheritdoc />
    public string Name => "Definition Query Analyzer";

    /// <inheritdoc />
    public string Description => "Checks definition queries for syntax errors, missing fields, and empty results.";

    /// <inheritdoc />
    public bool IsAutoFixable => false;

    /// <inheritdoc />
    public async Task<IEnumerable<HealthIssue>> AnalyzeAsync(AnalysisContext context, IProgress<ScanProgress> progress, CancellationToken cancellationToken)
    {
        var issues = new List<HealthIssue>();
        var maps = context.Project.GetItems<MapProjectItem>().ToList();
        
        foreach (var mapItem in maps)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            await QueuedTask.Run(() =>
            {
                var map = mapItem.GetMap();
                if (map == null) return;

                var featureLayers = map.GetLayersAsFlattenedList().OfType<FeatureLayer>();
                
                foreach (var layer in featureLayers)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    var defQueries = layer.DefinitionQueries;
                    if (defQueries == null || defQueries.Count == 0) continue;

                    var activeQuery = layer.DefinitionQuery;
                    var table = layer.GetTable();
                    List<string> fieldNames = new List<string>();
                    
                    if (table != null)
                    {
                        fieldNames = table.GetDefinition().GetFields().Select(f => f.Name.ToUpperInvariant()).ToList();
                    }

                    foreach (var dq in defQueries)
                    {
                        string sql = dq.WhereClause;
                        if (string.IsNullOrWhiteSpace(sql)) continue;

                        bool isActive = string.Equals(sql, activeQuery, StringComparison.OrdinalIgnoreCase);

                        if (!isActive)
                        {
                            issues.Add(new HealthIssue
                            {
                                Category = IssueCategory.DefinitionQuery,
                                Severity = IssueSeverity.Low,
                                ItemName = layer.Name,
                                ItemPath = mapItem.Name,
                                Description = $"Inactive definition query found: '{dq.Name}'.",
                                Recommendation = "Consider removing unused definition queries to avoid confusion and clutter.",
                                AnalyzerName = this.Name
                            });
                        }

                        // Basic parenthesis check
                        int openParens = sql.Count(c => c == '(');
                        int closeParens = sql.Count(c => c == ')');
                        if (openParens != closeParens)
                        {
                            issues.Add(new HealthIssue
                            {
                                Category = IssueCategory.DefinitionQuery,
                                Severity = IssueSeverity.High,
                                ItemName = layer.Name,
                                ItemPath = mapItem.Name,
                                Description = $"Definition query '{dq.Name}' has unbalanced parentheses.",
                                Recommendation = "Fix the SQL syntax of the definition query.",
                                AnalyzerName = this.Name
                            });
                        }

                        // Evaluate against empty results if layer is valid
                        if (isActive && table != null)
                        {
                            try
                            {
                                using var queryFilter = new QueryFilter { WhereClause = sql };
                                using var rowCursor = table.Search(queryFilter, false);
                                if (!rowCursor.MoveNext())
                                {
                                    issues.Add(new HealthIssue
                                    {
                                        Category = IssueCategory.DefinitionQuery,
                                        Severity = IssueSeverity.High,
                                        ItemName = layer.Name,
                                        ItemPath = mapItem.Name,
                                        Description = $"Definition query '{dq.Name}' returns 0 records.",
                                        Recommendation = "Verify that the definition query logic is correct and matches the underlying data.",
                                        AnalyzerName = this.Name
                                    });
                                }
                            }
                            catch (Exception)
                            {
                                issues.Add(new HealthIssue
                                {
                                    Category = IssueCategory.DefinitionQuery,
                                    Severity = IssueSeverity.High,
                                    ItemName = layer.Name,
                                    ItemPath = mapItem.Name,
                                    Description = $"Definition query '{dq.Name}' failed to execute (invalid SQL or missing field).",
                                    Recommendation = "Fix the SQL syntax and ensure all referenced fields exist in the layer.",
                                    AnalyzerName = this.Name
                                });
                            }
                        }
                    }
                }
            }, cancellationToken);
        }

        return issues;
    }
}
