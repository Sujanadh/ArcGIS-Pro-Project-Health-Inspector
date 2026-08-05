using System.Collections.Generic;

namespace APHI.Utilities;

/// <summary>
/// Constant values used throughout the application.
/// </summary>
public static class Constants
{
    /// <summary>
    /// General application constants.
    /// </summary>
    public const string PluginName = "Project Health Inspector";
    public const string PluginVersion = "1.0.0";

    /// <summary>
    /// Default thresholds.
    /// </summary>
    public const long DefaultLargeFeatureClassThreshold = 100000;
    public const long DefaultLargeRasterSizeMB = 500;
    public const long DefaultLargeGdbSizeMB = 1000;

    /// <summary>
    /// Severity color codes (Hex strings).
    /// </summary>
    public static class Colors
    {
        public const string Critical = "#DC3545"; // Red
        public const string High = "#FD7E14";     // Orange
        public const string Medium = "#FFC107";   // Yellow
        public const string Low = "#17A2B8";      // Teal/Cyan
        public const string Information = "#6C757D"; // Gray
    }

    /// <summary>
    /// Standard file extensions.
    /// </summary>
    public static class FileExtensions
    {
        public static readonly IReadOnlyList<string> Raster = new List<string> { ".tif", ".jpg", ".png", ".img", ".sid", ".jp2" }.AsReadOnly();
        public static readonly IReadOnlyList<string> Vector = new List<string> { ".shp", ".dwg", ".dxf", ".dgn" }.AsReadOnly();
        public static readonly IReadOnlyList<string> Tabular = new List<string> { ".csv", ".xls", ".xlsx", ".dbf" }.AsReadOnly();
    }
}
