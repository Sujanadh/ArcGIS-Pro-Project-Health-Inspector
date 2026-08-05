using System.Collections.Generic;

namespace APHI.Core.Models;

/// <summary>
/// Metrics specific to the performance of the ArcGIS Pro project.
/// </summary>
public class PerformanceMetrics
{
    /// <summary>
    /// The overall performance score from 0 to 100.
    /// </summary>
    public int OverallPerformanceScore { get; set; } = 100;

    /// <summary>
    /// Score based on the number of features rendered.
    /// </summary>
    public int FeatureCountScore { get; set; } = 100;

    /// <summary>
    /// Score based on the use of layer transparency.
    /// </summary>
    public int TransparencyScore { get; set; } = 100;

    /// <summary>
    /// Score based on labeling complexity.
    /// </summary>
    public int LabelingScore { get; set; } = 100;

    /// <summary>
    /// Score based on raster display resolution and settings.
    /// </summary>
    public int RasterResolutionScore { get; set; } = 100;

    /// <summary>
    /// Score based on the total number of layers.
    /// </summary>
    public int LayerCountScore { get; set; } = 100;

    /// <summary>
    /// Score based on table joins.
    /// </summary>
    public int JoinScore { get; set; } = 100;

    /// <summary>
    /// Score based on definition query complexity.
    /// </summary>
    public int DefinitionQueryScore { get; set; } = 100;

    /// <summary>
    /// Score based on symbology complexity.
    /// </summary>
    public int SymbologyComplexityScore { get; set; } = 100;

    /// <summary>
    /// Score based on network path usage for data sources.
    /// </summary>
    public int NetworkStorageScore { get; set; } = 100;

    /// <summary>
    /// Estimated time to render the current extent in milliseconds.
    /// </summary>
    public double EstimatedRenderTime { get; set; }

    /// <summary>
    /// List of recommendations for improving performance.
    /// </summary>
    public List<string> Recommendations { get; set; } = new List<string>();
}
