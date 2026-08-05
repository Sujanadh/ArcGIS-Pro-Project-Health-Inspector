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
/// Analyzes dataset sizes and record counts to identify potentially large datasets impacting performance.
/// </summary>
public class LargeDatasetAnalyzer : IAnalyzer
{
    public string Name => "Large Dataset Analyzer";
    public string Description => "Detects very large feature classes or datasets that may degrade performance.";
    public IssueCategory Category => IssueCategory.LargeDataset;

    private const long ThresholdCountHigh = 1000000;
    private const long ThresholdCountMedium = 100000;

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
                    CurrentOperation = "Checking dataset sizes",
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
                            if (count > ThresholdCountMedium)
                            {
                                var severity = count > ThresholdCountHigh ? IssueSeverity.High : IssueSeverity.Medium;
                                issues.Add(new HealthIssue
                                {
                                    Category = Category,
                                    Severity = severity,
                                    Title = "Large Dataset",
                                    Description = $"Layer '{layer.Name}' contains {count:N0} records, exceeding the threshold.",
                                    AffectedItem = layer.Name,
                                    AffectedItemPath = layer.URI,
                                    CurrentValue = $"{count:N0} records",
                                    ExpectedValue = $"< {ThresholdCountMedium:N0} records for optimal map performance",
                                    Recommendation = "Ensure spatial indexes are up to date, use definition queries, or enable scale dependency to prevent drawing all features at once.",
                                    Impact = "Drawing performance and query execution may be slow.",
                                    EstimatedBenefit = "Improves overall application responsiveness.",
                                    IsAutoFixable = false,
                                    AutoFixDescription = string.Empty,
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
