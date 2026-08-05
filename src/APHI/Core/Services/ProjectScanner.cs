using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using APHI.Core.Models;

namespace APHI.Core.Services;

/// <summary>
/// Orchestrates the scanning process for an ArcGIS Pro project.
/// </summary>
public class ProjectScanner
{
    private readonly AnalysisEngine _analysisEngine;
    private readonly Utilities.LogManager _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectScanner"/> class.
    /// </summary>
    /// <param name="analysisEngine">The analysis engine.</param>
    /// <param name="logger">The log manager.</param>
    public ProjectScanner(AnalysisEngine analysisEngine, Utilities.LogManager logger)
    {
        _analysisEngine = analysisEngine ?? throw new ArgumentNullException(nameof(analysisEngine));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Scans the current project asynchronously.
    /// </summary>
    /// <param name="progress">Progress reporter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A comprehensive health report.</returns>
    public async Task<HealthReport> ScanProjectAsync(IProgress<ScanProgress> progress, CancellationToken cancellationToken)
    {
        _logger.LogInfo("Starting project scan...");
        var startTime = DateTime.Now;
        var issues = new List<HealthIssue>();
        Project currentProject = Project.Current;

        if (currentProject == null)
        {
            _logger.LogError("No active project found.");
            throw new InvalidOperationException("No active ArcGIS Pro project.");
        }

        progress?.Report(new ScanProgress { CurrentOperation = "Initializing Scan", PercentComplete = 5 });

        // Retrieve maps on the MCT (Main CIM Thread)
        IReadOnlyList<Map> maps = new List<Map>();
        await QueuedTask.Run(() =>
        {
            maps = MapView.Active?.Map != null ? new List<Map> { MapView.Active.Map } : new List<Map>();
            // Note: A full implementation would iterate all project items to find all maps.
        });

        var settings = ProjectSettings.Load();
        var context = new AnalysisContext(currentProject, maps, settings, cancellationToken);

        try
        {
            var analyzerResults = await _analysisEngine.RunAllAnalyzersAsync(context, progress, cancellationToken);
            issues.AddRange(analyzerResults);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Scan was cancelled by the user.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred during scanning: {ex.Message}", ex);
        }

        progress?.Report(new ScanProgress { CurrentOperation = "Aggregating Results", PercentComplete = 95 });

        var report = new HealthReport
        {
            ProjectName = currentProject.Name,
            ProjectPath = currentProject.URI,
            ScanStartTime = startTime,
            ScanEndTime = DateTime.Now,
            Issues = issues,
            TotalMaps = maps.Count
            // Further aggregation logic can be added here
        };

        _logger.LogInfo($"Project scan completed in {report.ScanDuration.TotalSeconds} seconds. Found {issues.Count} issues.");
        progress?.Report(new ScanProgress { CurrentOperation = "Scan Complete", PercentComplete = 100 });

        return report;
    }
}
