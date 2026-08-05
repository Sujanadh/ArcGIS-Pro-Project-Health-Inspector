using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using APHI.Core.Models;
using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace APHI.Analysis;

/// <summary>
/// Analyzes maps for layers with broken data source paths.
/// </summary>
public class BrokenPathAnalyzer : IAnalyzer
{
    public string Name => "Broken Path Analyzer";
    public string Description => "Detects layers and tables with broken or inaccessible data sources.";
    public IssueCategory Category => IssueCategory.BrokenPath;

    public async Task<IReadOnlyList<HealthIssue>> AnalyzeAsync(AnalysisContext context, IProgress<ScanProgress> progress, CancellationToken cancellationToken)
    {
        var issues = new List<HealthIssue>();
        int totalItems = 0;
        int itemsProcessed = 0;
        var startTime = DateTime.Now;

        await QueuedTask.Run(() =>
        {
            foreach (var map in context.Maps)
            {
                totalItems += map.GetLayersAsFlattenedList().Count + map.GetStandaloneTablesAsFlattenedList().Count;
            }
        });

        foreach (var map in context.Maps)
        {
            IReadOnlyList<Layer> layers = null;
            IReadOnlyList<StandaloneTable> tables = null;

            await QueuedTask.Run(() =>
            {
                layers = map.GetLayersAsFlattenedList();
                tables = map.GetStandaloneTablesAsFlattenedList();
            });

            foreach (var layer in layers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                itemsProcessed++;

                progress?.Report(new ScanProgress
                {
                    CurrentOperation = "Checking broken paths",
                    CurrentItem = layer.Name,
                    ItemsProcessed = itemsProcessed,
                    TotalItems = totalItems,
                    PercentComplete = (double)itemsProcessed / totalItems * 100,
                    ElapsedTime = DateTime.Now - startTime,
                    Category = Category
                });

                try
                {
                    await QueuedTask.Run(() =>
                    {
                        var definition = layer.GetDefinition() as CIMDataLayer;
                        if (definition?.DataConnection != null)
                        {
                            bool isBroken = CheckDataConnection(definition.DataConnection);
                            if (isBroken)
                            {
                                issues.Add(new HealthIssue
                                {
                                    Category = Category,
                                    Severity = IssueSeverity.Critical,
                                    Title = "Broken Data Source",
                                    Description = $"The data source for layer '{layer.Name}' is broken or inaccessible.",
                                    AffectedItem = layer.Name,
                                    AffectedItemPath = layer.URI,
                                    CurrentValue = GetConnectionString(definition.DataConnection),
                                    ExpectedValue = "Accessible data source",
                                    Recommendation = "Update the layer's data source to point to a valid location.",
                                    Impact = "Layer will not draw or participate in analysis.",
                                    EstimatedBenefit = "Restores map functionality and visibility.",
                                    IsAutoFixable = true,
                                    AutoFixDescription = "Search for missing file in the project folder.",
                                    MapName = map.Name,
                                    LayerName = layer.Name
                                });
                            }
                        }
                    });
                }
                catch (Exception)
                {
                    // Skip layers that throw exceptions
                }
            }
            
            foreach (var table in tables)
            {
                cancellationToken.ThrowIfCancellationRequested();
                itemsProcessed++;

                progress?.Report(new ScanProgress
                {
                    CurrentOperation = "Checking broken paths",
                    CurrentItem = table.Name,
                    ItemsProcessed = itemsProcessed,
                    TotalItems = totalItems,
                    PercentComplete = (double)itemsProcessed / totalItems * 100,
                    ElapsedTime = DateTime.Now - startTime,
                    Category = Category
                });

                try
                {
                    await QueuedTask.Run(() =>
                    {
                        var definition = table.GetDefinition();
                        if (definition?.DataConnection != null)
                        {
                            bool isBroken = CheckDataConnection(definition.DataConnection);
                            if (isBroken)
                            {
                                issues.Add(new HealthIssue
                                {
                                    Category = Category,
                                    Severity = IssueSeverity.Critical,
                                    Title = "Broken Data Source",
                                    Description = $"The data source for table '{table.Name}' is broken or inaccessible.",
                                    AffectedItem = table.Name,
                                    AffectedItemPath = table.URI,
                                    CurrentValue = GetConnectionString(definition.DataConnection),
                                    ExpectedValue = "Accessible data source",
                                    Recommendation = "Update the table's data source to point to a valid location.",
                                    Impact = "Table data cannot be viewed or joined.",
                                    EstimatedBenefit = "Restores table functionality.",
                                    IsAutoFixable = true,
                                    AutoFixDescription = "Search for missing file in the project folder.",
                                    MapName = map.Name,
                                    LayerName = table.Name
                                });
                            }
                        }
                    });
                }
                catch (Exception)
                {
                    // Skip tables that throw exceptions
                }
            }
        }

        return issues;
    }

    private bool CheckDataConnection(CIMDataConnection connection)
    {
        var connString = GetConnectionString(connection);
        if (string.IsNullOrEmpty(connString)) return false;

        if (connString.Contains("DATABASE="))
        {
            var path = ExtractPathFromConnectionString(connString, "DATABASE=");
            if (!string.IsNullOrEmpty(path))
            {
                // Simple file/directory existence check as proxy for validity
                return !Directory.Exists(path) && !File.Exists(path);
            }
        }
        return false;
    }

    private string GetConnectionString(CIMDataConnection connection)
    {
        if (connection is CIMWorkspaceConnection workspaceConn)
            return workspaceConn.WorkspaceConnectionString;
        if (connection is CIMStandardDataConnection stdConn)
            return $"{stdConn.WorkspaceConnectionString} | {stdConn.Dataset}";
        return connection.GetType().Name;
    }

    private string ExtractPathFromConnectionString(string connectionString, string key)
    {
        var parts = connectionString.Split(';');
        foreach (var part in parts)
        {
            if (part.StartsWith(key, StringComparison.OrdinalIgnoreCase))
            {
                return part.Substring(key.Length);
            }
        }
        return string.Empty;
    }
}
