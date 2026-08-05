using System;

namespace APHI.Core.Models;

/// <summary>
/// Represents a single issue detected by the health inspector.
/// </summary>
public class HealthIssue
{
    /// <summary>
    /// Unique identifier for the issue.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The category of the issue.
    /// </summary>
    public IssueCategory Category { get; set; }

    /// <summary>
    /// The severity level of the issue.
    /// </summary>
    public IssueSeverity Severity { get; set; }

    /// <summary>
    /// A short title describing the issue.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the issue.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The name of the item affected by the issue.
    /// </summary>
    public string AffectedItem { get; set; } = string.Empty;

    /// <summary>
    /// The path or URI of the affected item.
    /// </summary>
    public string AffectedItemPath { get; set; } = string.Empty;

    /// <summary>
    /// The expected or recommended value.
    /// </summary>
    public string ExpectedValue { get; set; } = string.Empty;

    /// <summary>
    /// The current problematic value.
    /// </summary>
    public string CurrentValue { get; set; } = string.Empty;

    /// <summary>
    /// Recommendation on how to fix the issue.
    /// </summary>
    public string Recommendation { get; set; } = string.Empty;

    /// <summary>
    /// The impact this issue has on the project.
    /// </summary>
    public string Impact { get; set; } = string.Empty;

    /// <summary>
    /// Estimated benefit of fixing the issue.
    /// </summary>
    public string EstimatedBenefit { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether this issue can be automatically fixed by the tool.
    /// </summary>
    public bool IsAutoFixable { get; set; }

    /// <summary>
    /// Description of what the auto-fix will do.
    /// </summary>
    public string AutoFixDescription { get; set; } = string.Empty;

    /// <summary>
    /// The time the issue was detected.
    /// </summary>
    public DateTime DetectedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// The name of the map where the issue was found (if applicable).
    /// </summary>
    public string MapName { get; set; } = string.Empty;

    /// <summary>
    /// The name of the layer where the issue was found (if applicable).
    /// </summary>
    public string LayerName { get; set; } = string.Empty;

    /// <summary>
    /// The target object (e.g. Map, Layer, ProjectItem) associated with this issue.
    /// </summary>
    public object TargetObject { get; set; }

    /// <summary>
    /// Additional properties used for autofixing.
    /// </summary>
    public System.Collections.Generic.Dictionary<string, string> Properties { get; set; } = new System.Collections.Generic.Dictionary<string, string>();

    /// <summary>
    /// The name of the analyzer that generated this issue.
    /// </summary>
    public string AnalyzerName { get; set; } = string.Empty;
    public string FixerName { get; set; } = string.Empty;
    public bool IsFixed { get; set; }

    /// <summary>
    /// Returns a string representation of the issue.
    /// </summary>
    /// <returns>A string in the format [Severity] Title: Description</returns>
    public override string ToString()
    {
        return $"[{Severity}] {Title}: {AffectedItem}";
    }
}
