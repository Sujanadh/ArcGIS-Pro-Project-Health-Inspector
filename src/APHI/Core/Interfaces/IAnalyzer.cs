using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using APHI.Core.Models;

namespace APHI.Core.Interfaces;

/// <summary>
/// Interface for all project health analyzers.
/// </summary>
public interface IAnalyzer
{
    /// <summary>
    /// Gets the name of the analyzer.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of what the analyzer checks.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the category of issues this analyzer detects.
    /// </summary>
    IssueCategory Category { get; }

    /// <summary>
    /// Executes the analysis asynchronously.
    /// </summary>
    /// <param name="context">The analysis context.</param>
    /// <param name="progress">Progress reporter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of health issues found by this analyzer.</returns>
    Task<IReadOnlyList<HealthIssue>> AnalyzeAsync(AnalysisContext context, IProgress<ScanProgress> progress, CancellationToken cancellationToken);
}
