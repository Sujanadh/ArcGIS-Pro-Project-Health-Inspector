using System.Threading;
using System.Threading.Tasks;
using APHI.Core.Models;

namespace APHI.Core.Interfaces;

/// <summary>
/// Interface for components that can automatically fix detected health issues.
/// </summary>
public interface IAutoFixer
{
    /// <summary>
    /// Gets the name of the auto-fixer.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the category of issues this fixer can resolve.
    /// </summary>
    IssueCategory TargetCategory { get; }

    /// <summary>
    /// Previews the fix without applying it.
    /// </summary>
    /// <param name="issue">The issue to fix.</param>
    /// <param name="context">The analysis context.</param>
    /// <returns>A result detailing what would be changed.</returns>
    Task<AutoFixResult> PreviewFixAsync(HealthIssue issue, AnalysisContext context);

    /// <summary>
    /// Applies the fix.
    /// </summary>
    /// <param name="issue">The issue to fix.</param>
    /// <param name="context">The analysis context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result indicating success or failure of the fix.</returns>
    Task<AutoFixResult> ApplyFixAsync(HealthIssue issue, AnalysisContext context, CancellationToken cancellationToken);
}
