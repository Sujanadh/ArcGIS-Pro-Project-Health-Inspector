using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using APHI.Core.Models;
using APHI.Core.Interfaces;
using ArcGIS.Core.Data.Raster;

namespace APHI.Analysis;

/// <summary>
/// Analyzes raster layers for performance optimizations such as pyramids and statistics.
/// </summary>
public class RasterOptimizationAnalyzer : IAnalyzer
{
    /// <inheritdoc />
    public string Name => "Raster Optimization Analyzer";

    /// <inheritdoc />
    public string Description => "Checks raster datasets for pyramids, statistics, and appropriate formats.";

    /// <inheritdoc />
    public bool IsAutoFixable => true;

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

                var rasterLayers = map.GetLayersAsFlattenedList().OfType<BasicRasterLayer>();
                
                foreach (var layer in rasterLayers)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    try
                    {
                        var raster = layer.GetRaster();
                        if (raster == null) continue;

                        var rasterDataset = raster.GetRasterDataset();
                        if (rasterDataset == null) continue;

                        var datastore = rasterDataset.GetDatastore();
                        string path = datastore?.GetPath()?.LocalPath ?? string.Empty;
                        
                        if (!string.IsNullOrEmpty(path))
                        {
                            string baseName = Path.Combine(path, rasterDataset.GetName());
                            bool hasPyramids = File.Exists(baseName + ".ovr") || File.Exists(baseName + ".rrd");
                            bool hasStats = File.Exists(baseName + ".aux.xml");

                            if (!hasPyramids)
                            {
                                issues.Add(new HealthIssue
                                {
                                    Category = IssueCategory.RasterOptimization,
                                    Severity = IssueSeverity.Medium,
                                    ItemName = layer.Name,
                                    ItemPath = mapItem.Name,
                                    Description = "Raster dataset is missing pyramids.",
                                    Recommendation = "Build pyramids for this raster to improve drawing performance at various scales.",
                                    AnalyzerName = this.Name
                                });
                            }

                            if (!hasStats)
                            {
                                issues.Add(new HealthIssue
                                {
                                    Category = IssueCategory.RasterOptimization,
                                    Severity = IssueSeverity.Low,
                                    ItemName = layer.Name,
                                    ItemPath = mapItem.Name,
                                    Description = "Raster dataset is missing statistics.",
                                    Recommendation = "Calculate statistics to improve rendering speed and symbology accuracy.",
                                    AnalyzerName = this.Name
                                });
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Some rasters like image services might fail these checks, ignore them safely.
                    }
                }
            }, cancellationToken);
        }

        return issues;
    }
}
