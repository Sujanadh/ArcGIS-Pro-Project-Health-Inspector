using System;

namespace APHI.Core.Models;

/// <summary>
/// Represents the result of an auto-fix operation.
/// </summary>
public class AutoFixResult
{
    /// <summary>
    /// Indicates whether the fix was successfully applied.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// A message describing the result of the operation.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Description of the state before the fix was applied.
    /// </summary>
    public string OriginalState { get; set; } = string.Empty;

    /// <summary>
    /// Description of the state after the fix was applied.
    /// </summary>
    public string NewState { get; set; } = string.Empty;

    /// <summary>
    /// An action that can be invoked to roll back the fix (if supported).
    /// </summary>
    public Action? RollbackAction { get; set; }

    /// <summary>
    /// The issue that this fix addressed.
    /// </summary>
    public HealthIssue FixedIssue { get; set; } = new HealthIssue();

    /// <summary>
    /// The time the fix was applied.
    /// </summary>
    public DateTime AppliedAt { get; set; } = DateTime.Now;
}
