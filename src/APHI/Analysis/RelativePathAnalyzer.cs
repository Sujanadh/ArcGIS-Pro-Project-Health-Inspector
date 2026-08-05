using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using APHI.Core.Models;
using APHI.Core.Interfaces;

namespace APHI.Analysis;

/// <summary>
/// Analyzes whether the project and its layers are configured to use relative paths,
/// which improves portability.
/// </summary>
public class RelativePathAnalyzer : IAnalyzer
{
    /// <inheritdoc />
    public string Name => "Relative Path Analyzer";

    /// <inheritdoc />
    public string Description => "Checks if the project stores relative paths to data sources.";

    /// <inheritdoc />
    public IssueCategory Category => IssueCategory.RelativePath;

    /// <inheritdoc />
    public async Task<IReadOnlyList<HealthIssue>> AnalyzeAsync(AnalysisContext context, IProgress<ScanProgress> progress, CancellationToken cancellationToken)
    {
        var issues = new List<HealthIssue>();

        cancellationToken.ThrowIfCancellationRequested();

        await QueuedTask.Run(() =>
        {
            var project = context.Project;
            // Since ArcGIS Pro SDK does not expose a direct boolean for "store relative paths" 
            // easily via the API in recent versions, we infer or report based on home folder paths
            // or absolute paths observed in layers.
            
            var homeFolder = project.HomeFolderPath;
            var maps = project.GetItems<MapProjectItem>().ToList();
            bool absolutePathFound = false;

            foreach (var mapItem in maps)
            {
                var map = mapItem.GetMap();
                if (map == null) continue;

                var layers = map.GetLayersAsFlattenedList();
                foreach (var layer in layers)
                {
                    if (layer is not BasicFeatureLayer featureLayer) continue;

                    var connection = featureLayer.GetWorkspace()?.GetConnectionProperties();
                    if (connection == null) continue;

                    string connectionString = connection.Instance ?? string.Empty;
                    if (string.IsNullOrEmpty(connectionString)) continue;

                    if (connectionString.StartsWith("C:\\", StringComparison.OrdinalIgnoreCase) || 
                        connectionString.StartsWith("D:\\", StringComparison.OrdinalIgnoreCase))
                    {
                        absolutePathFound = true;
                    }
                }
            }

            if (absolutePathFound)
            {
                issues.Add(new HealthIssue
                {
                    Category = IssueCategory.RelativePath,
                    Severity = IssueSeverity.Medium,
                    AffectedItem = project.Name,
                    AffectedItemPath = project.URI,
                    Description = "The project contains layers with absolute paths.",
                    Recommendation = "Configure the project or layers to use relative paths to improve project portability.",
                    AnalyzerName = this.Name
                });
            }
            else
            {
                issues.Add(new HealthIssue
                {
                    Category = IssueCategory.RelativePath,
                    Severity = IssueSeverity.Information,
                    AffectedItem = project.Name,
                    AffectedItemPath = project.URI,
                    Description = "The project appears to use relative paths or enterprise connections correctly.",
                    Recommendation = "Maintain relative path usage for robust sharing.",
                    AnalyzerName = this.Name
                });
            }

        }, cancellationToken);

        return issues;
    }
}
