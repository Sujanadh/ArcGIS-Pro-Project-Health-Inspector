using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using APHI.Core.Models;
using APHI.Core.Interfaces;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace APHI.Analysis;

/// <summary>
/// Analyzes feature layers to ensure spatial indexes are present.
/// </summary>
public class SpatialIndexAnalyzer : IAnalyzer
{
    public string Name => "Spatial Index Analyzer";
    public string Description => "Checks if feature layers have an active spatial index.";
    public IssueCategory Category => IssueCategory.SpatialIndex;

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
                totalItems += map.GetLayersAsFlattenedList().OfType<FeatureLayer>().Count();
            }
        });

        foreach (var map in context.Maps)
        {
            IReadOnlyList<FeatureLayer> featureLayers = null;

            await QueuedTask.Run(() =>
            {
                featureLayers = map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList();
            });

            foreach (var layer in featureLayers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                itemsProcessed++;

                progress?.Report(new ScanProgress
                {
                    CurrentOperation = "Checking spatial indexes",
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
                        var featureClass = layer.GetFeatureClass();
                        if (featureClass != null)
                        {
                            var definition = featureClass.GetDefinition();
                            if (!definition.HasSpatialIndex())
                            {
                                issues.Add(new HealthIssue
                                {
                                    Category = Category,
                                    Severity = IssueSeverity.Medium,
                                    Title = "Missing Spatial Index",
                                    Description = $"Feature layer '{layer.Name}' does not have a spatial index.",
                                    AffectedItem = layer.Name,
                                    AffectedAffectedItemPath = layer.URI,
                                    CurrentValue = "No spatial index",
                                    ExpectedValue = "Has spatial index",
                                    Recommendation = "Create a spatial index for the feature class to improve drawing and spatial query performance.",
                                    Impact = "Slow map rendering and inefficient spatial queries.",
                                    EstimatedBenefit = "Significantly faster drawing and selection.",
                                    IsAutoFixable = true,
                                    AutoFixDescription = "Create spatial index on the geodatabase feature class.",
                                    MapName = map.Name,
                                    LayerName = layer.Name
                                });
                            }
                        }
                    });
                }
                catch (Exception)
                {
                    // Ignore exception
                }
            }
        }

        return issues;
    }
}
