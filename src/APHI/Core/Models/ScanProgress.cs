using System;

namespace APHI.Core.Models;

/// <summary>
/// Represents the progress of the scanning operation.
/// </summary>
public class ScanProgress
{
    /// <summary>
    /// Description of the current operation.
    /// </summary>
    public string CurrentOperation { get; set; } = string.Empty;

    /// <summary>
    /// The specific item currently being processed.
    /// </summary>
    public string CurrentItem { get; set; } = string.Empty;

    /// <summary>
    /// Number of items processed so far.
    /// </summary>
    public int ItemsProcessed { get; set; }

    /// <summary>
    /// Total number of items to process.
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Percentage of completion (0.0 to 100.0).
    /// </summary>
    public double PercentComplete { get; set; }

    /// <summary>
    /// Time elapsed since the scan started.
    /// </summary>
    public TimeSpan ElapsedTime { get; set; }

    /// <summary>
    /// The category currently being analyzed, if applicable.
    /// </summary>
    public IssueCategory? Category { get; set; }
}
