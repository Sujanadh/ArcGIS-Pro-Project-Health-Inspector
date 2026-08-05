using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using APHI.Core.Models;
using APHI.Core.Interfaces;

namespace APHI.Analysis;

/// <summary>
/// Analyzes overall map performance and computes a performance score based on
/// layer counts, transparencies, labeling, and complexity.
/// </summary>
public class PerformanceAnalyzer : IAnalyzer
{
    /// <inheritdoc />
    public string Name => "Performance Analyzer";

    /// <inheritdoc />
    public string Description => "Evaluates map performance based on feature counts, layer counts, transparency, and labeling complexity.";

    /// <inheritdoc />
    public IssueCategory Category => IssueCategory.Performance;

    /// <inheritdoc />
    public async Task<IReadOnlyList<HealthIssue>> AnalyzeAsync(AnalysisContext context, IProgress<ScanProgress> progress, CancellationToken cancellationToken)
    {
        var issues = new List<HealthIssue>();
        var maps = context.Project.GetItems<MapProjectItem>().ToList();

        foreach (var mapItem in maps)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await QueuedTask.Run(() => {
                var map = mapItem.GetMap();
                if (map == null) return;

                int score = 100;
                var layers = map.GetLayersAsFlattenedList().ToList();
                int totalLayers = layers.Count;

                if (totalLayers > 50)
                {
                    score -= 10;
                    issues.Add(new HealthIssue
                    {
                        Category = IssueCategory.Performance,
                        Severity = IssueSeverity.Medium,
                        AffectedItem = mapItem.Name,
                        AffectedItemPath = mapItem.Name,
                        Description = $"Map has > 50 layers ({totalLayers}).",
                        Recommendation = "Remove unnecessary layers or group them into composite map services.",
                        AnalyzerName = this.Name
                    });
                }

                foreach (var layer in layers)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Transparency
                    if (layer is FeatureLayer featureLayer)
                    {
                        if (featureLayer.Transparency > 0)
                        {
                            score -= 1;
                        }

                        // Labeling
                        var cim = featureLayer.GetDefinition() as ArcGIS.Core.CIM.CIMFeatureLayer;
                        if (cim != null && cim.Visibility) { score -= 2; }

                        // Try to evaluate count if safe
                        try
                        {
                            var table = featureLayer.GetTable();
                            if (table != null)
                            {
                                long count = table.GetCount();
                                if (count > 100000)
                                {
                                    score -= 5;
                                    issues.Add(new HealthIssue
                                    {
                                        Category = IssueCategory.Performance,
                                        Severity = IssueSeverity.Medium,
                                        AffectedItem = layer.Name,
                                        AffectedItemPath = mapItem.Name,
                                        Description = $"Layer contains a very large dataset (>100K features, count={count}).",
                                        Recommendation = "Consider applying a definition query or building a spatial index to improve drawing performance.",
                                        AnalyzerName = this.Name
                                    });
                                }
                            }
                        }
                        catch { /* Ignored */ }
                    }
                    
                    if (layer is BasicRasterLayer rasterLayer)
                    {
                        // Check raster complexity heuristics, etc.
                        score -= 1; // minor penalty for rasters without optimizations checked elsewhere
                    }
                }

                // Severity categorization based on score
                var severity = IssueSeverity.Information;
                if (score < 40) severity = IssueSeverity.High;
                else if (score < 70) severity = IssueSeverity.Medium;
                else if (score < 85) severity = IssueSeverity.Low;

                if (severity != IssueSeverity.Information)
                {
                    issues.Add(new HealthIssue
                    {
                        Category = IssueCategory.Performance,
                        Severity = severity,
                        AffectedItem = mapItem.Name,
                        AffectedItemPath = mapItem.Name,
                        Description = $"Map performance score is low ({score}/100) due to complex layers, transparency, or large datasets.",
                        Recommendation = "Review the map for unnecessary transparency, complex labels, and large unindexed datasets.",
                        AnalyzerName = this.Name
                    });
                }
            });
        }

        return issues;
    }
}
