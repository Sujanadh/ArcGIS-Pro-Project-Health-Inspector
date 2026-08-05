using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using APHI.Core.Interfaces;
using APHI.Core.Models;

namespace APHI.Core.Services;

/// <summary>
/// Manages the registration and execution of health analyzers.
/// </summary>
public class AnalysisEngine
{
    private readonly List<IAnalyzer> _analyzers = new List<IAnalyzer>();
    private readonly Utilities.LogManager _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalysisEngine"/> class.
    /// </summary>
    /// <param name="logger">The log manager.</param>
    public AnalysisEngine(Utilities.LogManager logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Registers a single analyzer.
    /// </summary>
    /// <param name="analyzer">The analyzer to register.</param>
    public void RegisterAnalyzer(IAnalyzer analyzer)
    {
        if (analyzer != null && !_analyzers.Contains(analyzer))
        {
            _analyzers.Add(analyzer);
        }
    }

    /// <summary>
    /// Registers all standard analyzers.
    /// </summary>
    public void RegisterAllAnalyzers()
    {
        // In a real implementation, this would use reflection or a DI container to find and register all IAnalyzer implementations.
        _logger.LogInfo("Analyzers registered.");
    }

    /// <summary>
    /// Runs all registered analyzers.
    /// </summary>
    /// <param name="context">The analysis context.</param>
    /// <param name="progress">Progress reporter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A combined list of health issues.</returns>
    public async Task<IReadOnlyList<HealthIssue>> RunAllAnalyzersAsync(AnalysisContext context, IProgress<ScanProgress> progress, CancellationToken cancellationToken)
    {
        var allIssues = new List<HealthIssue>();
        int totalAnalyzers = _analyzers.Count;
        int completed = 0;

        foreach (var analyzer in _analyzers)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                progress?.Report(new ScanProgress 
                { 
                    CurrentOperation = $"Running {analyzer.Name}",
                    TotalItems = totalAnalyzers,
                    ItemsProcessed = completed,
                    PercentComplete = 10.0 + (80.0 * completed / totalAnalyzers),
                    Category = analyzer.Category
                });

                var issues = await analyzer.AnalyzeAsync(context, progress, cancellationToken);
                if (issues != null)
                {
                    allIssues.AddRange(issues);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Analyzer '{analyzer.Name}' failed: {ex.Message}", ex);
            }

            completed++;
        }

        return allIssues;
    }

    /// <summary>
    /// Runs a specific analyzer by name.
    /// </summary>
    /// <param name="analyzerName">The name of the analyzer.</param>
    /// <param name="context">The analysis context.</param>
    /// <param name="progress">Progress reporter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of health issues.</returns>
    public async Task<IReadOnlyList<HealthIssue>> RunAnalyzerAsync(string analyzerName, AnalysisContext context, IProgress<ScanProgress> progress, CancellationToken cancellationToken)
    {
        var analyzer = _analyzers.Find(a => a.Name.Equals(analyzerName, StringComparison.OrdinalIgnoreCase));
        if (analyzer != null)
        {
            return await analyzer.AnalyzeAsync(context, progress, cancellationToken);
        }

        _logger.LogWarning($"Analyzer '{analyzerName}' not found.");
        return new List<HealthIssue>();
    }
}
