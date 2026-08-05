using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using APHI.Core.Models;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace APHI.Analysis;

/// <summary>
/// Analyzes map and layer projection consistencies.
/// </summary>
public class ProjectionConsistencyAnalyzer : IAnalyzer
{
    public string Name => "Projection Consistency Analyzer";
    public string Description => "Checks for projection mismatches between layers and their maps.";
    public IssueCategory Category => IssueCategory.ProjectionInconsistency;

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
            SpatialReference mapSr = null;
            IReadOnlyList<FeatureLayer> featureLayers = null;

            await QueuedTask.Run(() =>
            {
                mapSr = map.SpatialReference;
                featureLayers = map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList();
            });

            if (mapSr == null) continue;

            foreach (var layer in featureLayers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                itemsProcessed++;

                progress?.Report(new ScanProgress
                {
                    CurrentOperation = "Checking spatial references",
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
                        var layerSr = layer.GetSpatialReference();
                        if (layerSr != null && !mapSr.IsEqual(layerSr))
                        {
                            var severity = (mapSr.IsProjected != layerSr.IsProjected) 
                                ? IssueSeverity.Medium 
                                : IssueSeverity.High;

                            issues.Add(new HealthIssue
                            {
                                Category = Category,
                                Severity = severity,
                                Title = "Projection Mismatch",
                                Description = $"Layer '{layer.Name}' has a different spatial reference than the map.",
                                AffectedItem = layer.Name,
                                AffectedAffectedItemPath = layer.URI,
                                CurrentValue = $"WKID: {layerSr.Wkid}, Name: {layerSr.Name} ({(layerSr.IsProjected ? "Projected" : "Geographic")})",
                                ExpectedValue = $"WKID: {mapSr.Wkid}, Name: {mapSr.Name} ({(mapSr.IsProjected ? "Projected" : "Geographic")})",
                                Recommendation = "Project the dataset to match the map's coordinate system to avoid on-the-fly projection overhead.",
                                Impact = "On-the-fly projection degrades drawing performance and analysis accuracy.",
                                EstimatedBenefit = "Improves drawing speed and spatial analysis reliability.",
                                IsAutoFixable = false,
                                AutoFixDescription = string.Empty,
                                MapName = map.Name,
                                LayerName = layer.Name
                            });
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
}
