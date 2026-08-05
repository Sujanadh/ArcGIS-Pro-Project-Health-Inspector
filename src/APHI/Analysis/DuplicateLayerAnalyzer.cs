using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using APHI.Core.Models;
using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace APHI.Analysis;

/// <summary>
/// Analyzes maps for duplicate layers and data sources.
/// </summary>
public class DuplicateLayerAnalyzer : IAnalyzer
{
    public string Name => "Duplicate Layer Analyzer";
    public string Description => "Detects layers with duplicate names or duplicate data sources within a map.";
    public IssueCategory Category => IssueCategory.DuplicateLayer;

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
                totalItems += map.GetLayersAsFlattenedList().Count;
            }
        });

        foreach (var map in context.Maps)
        {
            IReadOnlyList<Layer> layers = null;

            await QueuedTask.Run(() =>
            {
                layers = map.GetLayersAsFlattenedList();
            });

            var layerNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var dataSources = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var layer in layers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                itemsProcessed++;

                progress?.Report(new ScanProgress
                {
                    CurrentOperation = "Checking duplicate layers",
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
                        // Check for duplicate names
                        if (!layerNames.Add(layer.Name))
                        {
                            issues.Add(new HealthIssue
                            {
                                Category = Category,
                                Severity = IssueSeverity.Medium,
                                Title = "Duplicate Layer Name",
                                Description = $"Multiple layers named '{layer.Name}' exist in the map.",
                                AffectedItem = layer.Name,
                                AffectedItemPath = layer.URI,
                                CurrentValue = layer.Name,
                                ExpectedValue = "Unique layer name",
                                Recommendation = "Rename or remove duplicate layers to avoid confusion.",
                                Impact = "Can cause confusion for users and scripting errors.",
                                EstimatedBenefit = "Improves map clarity.",
                                IsAutoFixable = true,
                                AutoFixDescription = "Rename duplicate layer with numeric suffix.",
                                MapName = map.Name,
                                LayerName = layer.Name
                            });
                        }

                        // Check for duplicate data sources
                        var definition = layer.GetDefinition() as CIMDataLayer;
                        if (definition?.DataConnection != null)
                        {
                            var connString = GetConnectionString(definition.DataConnection);
                            if (!string.IsNullOrEmpty(connString) && !dataSources.Add(connString))
                            {
                                issues.Add(new HealthIssue
                                {
                                    Category = Category,
                                    Severity = IssueSeverity.Low,
                                    Title = "Duplicate Data Source",
                                    Description = $"Layer '{layer.Name}' uses a data source already present in another layer.",
                                    AffectedItem = layer.Name,
                                    AffectedItemPath = layer.URI,
                                    CurrentValue = connString,
                                    ExpectedValue = "Only one layer per data source unless intentional",
                                    Recommendation = "Consider removing duplicate layers pointing to the same data unless using different definition queries or symbology.",
                                    Impact = "Increases project file size and potential drawing time overhead.",
                                    EstimatedBenefit = "Optimizes performance and project organization.",
                                    IsAutoFixable = true,
                                    AutoFixDescription = "Remove duplicate layer if properties are identical.",
                                    MapName = map.Name,
                                    LayerName = layer.Name
                                });
                            }
                        }
                    });
                }
                catch (Exception)
                {
                    // Ignore exceptions for individual layers
                }
            }
        }

        return issues;
    }

    private string GetConnectionString(CIMDataConnection connection)
    {
        if (connection is CIMWorkspaceConnection workspaceConn)
            return workspaceConn.WorkspaceConnectionString;
        if (connection is CIMStandardDataConnection stdConn)
            return $"{stdConn.WorkspaceConnectionString} | {stdConn.Dataset}";
        return connection.GetType().Name;
    }
}
