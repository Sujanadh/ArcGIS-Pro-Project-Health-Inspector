using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace APHI.Core.Models;

/// <summary>
/// Configuration settings for the Project Health Inspector.
/// </summary>
public class ProjectSettings
{
    /// <summary>
    /// Folders to ignore during scanning.
    /// </summary>
    public List<string> IgnoredFolders { get; set; } = new List<string>();

    /// <summary>
    /// Layer types to ignore during scanning.
    /// </summary>
    public List<string> IgnoredLayerTypes { get; set; } = new List<string>();

    /// <summary>
    /// Whether auto-fix features are enabled.
    /// </summary>
    public bool EnableAutoFix { get; set; } = true;

    /// <summary>
    /// The threshold for a feature class to be considered "large".
    /// </summary>
    public long LargeFeatureClassThreshold { get; set; } = 100000;

    /// <summary>
    /// The threshold in MB for a raster to be considered "large".
    /// </summary>
    public long LargeRasterSizeMB { get; set; } = 500;

    /// <summary>
    /// The threshold in MB for a file geodatabase to be considered "large".
    /// </summary>
    public long LargeGdbSizeMB { get; set; } = 1000;

    /// <summary>
    /// The default folder to save exported reports.
    /// </summary>
    public string DefaultReportFolder { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "APHI_Reports");

    /// <summary>
    /// The default format for reports (e.g., "HTML", "PDF", "JSON").
    /// </summary>
    public string ReportFormat { get; set; } = "HTML";

    /// <summary>
    /// Whether dark mode is enabled for reports and UI.
    /// </summary>
    public bool EnableDarkMode { get; set; } = false;

    /// <summary>
    /// Whether informational issues should be displayed.
    /// </summary>
    public bool ShowInformationalIssues { get; set; } = true;

    /// <summary>
    /// Weights assigned to different categories for score calculation.
    /// </summary>
    public Dictionary<IssueCategory, double> CategoryWeights { get; set; } = new Dictionary<IssueCategory, double>
    {
        { IssueCategory.BrokenPath, 0.20 },
        { IssueCategory.ProjectionInconsistency, 0.15 },
        { IssueCategory.Performance, 0.15 },
        { IssueCategory.DuplicateLayer, 0.05 },
        { IssueCategory.EmptyDataset, 0.05 },
        { IssueCategory.LargeDataset, 0.05 },
        { IssueCategory.SpatialIndex, 0.05 },
        { IssueCategory.BrokenJoin, 0.10 },
        { IssueCategory.LabelIssue, 0.05 },
        { IssueCategory.SymbologyIssue, 0.05 },
        { IssueCategory.DefinitionQuery, 0.05 },
        { IssueCategory.NetworkPath, 0.02 },
        { IssueCategory.RelativePath, 0.01 },
        { IssueCategory.RasterOptimization, 0.02 },
        { IssueCategory.LayerNaming, 0.01 },
        { IssueCategory.Metadata, 0.04 }
    };

    /// <summary>
    /// Loads settings from the user's AppData folder.
    /// </summary>
    /// <returns>The loaded settings or defaults if file not found.</returns>
    public static ProjectSettings Load()
    {
        try
        {
            var path = GetSettingsFilePath();
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<ProjectSettings>(json) ?? new ProjectSettings();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
        }
        return new ProjectSettings();
    }

    /// <summary>
    /// Saves current settings to the user's AppData folder.
    /// </summary>
    public void Save()
    {
        try
        {
            var path = GetSettingsFilePath();
            var dir = Path.GetDirectoryName(path);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
        }
    }

    private static string GetSettingsFilePath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, "APHI", "Settings.json");
    }
}
