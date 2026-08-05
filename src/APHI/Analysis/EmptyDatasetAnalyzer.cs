using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using APHI.Core.Models;
using APHI.Core.Interfaces;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace APHI.Analysis;

/// <summary>
/// Detects empty layers or tables in maps.
/// </summary>
public class EmptyDatasetAnalyzer : IAnalyzer
{
    public string Name => "Empty Dataset Analyzer";
    public string Description => "Detects feature layers and standalone tables that contain zero records.";
    public IssueCategory Category => IssueCategory.EmptyDataset;

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
                totalItems += map.GetLayersAsFlattenedList().OfType<FeatureLayer>().Count() + 
                              map.GetStandaloneTablesAsFlattenedList().Count;
            }
        });

        foreach (var map in context.Maps)
        {
            IReadOnlyList<FeatureLayer> featureLayers = null;
            IReadOnlyList<StandaloneTable> standaloneTables = null;

            await QueuedTask.Run(() =>
            {
                featureLayers = map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList();
                standaloneTables = map.GetStandaloneTablesAsFlattenedList();
            });

            foreach (var layer in featureLayers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                itemsProcessed++;

                progress?.Report(new ScanProgress
                {
                    CurrentOperation = "Checking dataset records",
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
                        var table = layer.GetTable();
                        if (table != null)
                        {
                            long count = table.GetCount();
                            if (count == 0)
                            {
                                issues.Add(new HealthIssue
                                {
                                    Category = Category,
                                    Severity = IssueSeverity.Low,
                                    Title = "Empty Feature Layer",
                                    Description = $"Feature layer '{layer.Name}' contains 0 records.",
                                    AffectedItem = layer.Name,
                                    AffectedItemPath = layer.URI,
                                    CurrentValue = "0 records",
                                    ExpectedValue = "> 0 records",
                                    Recommendation = "Remove the empty layer to declutter the map, or verify if data load is pending.",
                                    Impact = "Unnecessary clutter in the Table of Contents.",
                                    EstimatedBenefit = "Cleaner project structure.",
                                    IsAutoFixable = true,
                                    AutoFixDescription = "Remove the layer from the map.",
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

            foreach (var table in standaloneTables)
            {
                cancellationToken.ThrowIfCancellationRequested();
                itemsProcessed++;

                progress?.Report(new ScanProgress
                {
                    CurrentOperation = "Checking dataset records",
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
                        var dataTable = table.GetTable();
                        if (dataTable != null)
                        {
                            long count = dataTable.GetCount();
                            if (count == 0)
                            {
                                issues.Add(new HealthIssue
                                {
                                    Category = Category,
                                    Severity = IssueSeverity.Information,
                                    Title = "Empty Standalone Table",
                                    Description = $"Standalone table '{table.Name}' contains 0 rows.",
                                    AffectedItem = table.Name,
                                    AffectedItemPath = table.URI,
                                    CurrentValue = "0 rows",
                                    ExpectedValue = "> 0 rows",
                                    Recommendation = "Remove the empty table if not required.",
                                    Impact = "Minor clutter.",
                                    EstimatedBenefit = "Cleaner project structure.",
                                    IsAutoFixable = true,
                                    AutoFixDescription = "Remove the table from the map.",
                                    MapName = map.Name,
                                    LayerName = table.Name
                                });
                            }
                        }
                    });
                }
                catch (Exception)
                {
                    // Ignore exceptions
                }
            }
        }

        return issues;
    }
}
