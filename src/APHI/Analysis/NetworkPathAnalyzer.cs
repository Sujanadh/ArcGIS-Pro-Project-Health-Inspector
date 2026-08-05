using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using APHI.Core.Models;
using APHI.Core.Interfaces;

namespace APHI.Analysis;

/// <summary>
/// Analyzes network paths used by layers to identify potential connectivity,
/// performance, or portability issues.
/// </summary>
public class NetworkPathAnalyzer : IAnalyzer
{
    /// <inheritdoc />
    public string Name => "Network Path Analyzer";

    /// <inheritdoc />
    public string Description => "Checks layer data sources for UNC paths, mapped drives, or disconnected network paths.";

    /// <inheritdoc />
    public IssueCategory Category => IssueCategory.NetworkPath;

    /// <inheritdoc />
    public async Task<IReadOnlyList<HealthIssue>> AnalyzeAsync(AnalysisContext context, IProgress<ScanProgress> progress, CancellationToken cancellationToken)
    {
        var issues = new List<HealthIssue>();
        var maps = context.Project.GetItems<MapProjectItem>().ToList();

        foreach (var mapItem in maps)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await QueuedTask.Run(() => {
                var map = mapItem.GetMap();
                if (map == null) return;

                var layers = map.GetLayersAsFlattenedList();
                
                foreach (var layer in layers)
                {
                    if (layer is not BasicFeatureLayer featureLayer) continue;

                    var connection = featureLayer.GetTable()?.GetDatastore()?.GetConnector() as ArcGIS.Core.Data.DatabaseConnectionProperties;
                    if (connection == null) continue;
                    string connectionString = connection.Instance ?? string.Empty;
                    // duplicate code removed
                    
                    if (string.IsNullOrEmpty(connectionString)) continue;

                    bool isUnc = connectionString.StartsWith(@"\\");
                    bool isMappedDrive = connectionString.Length >= 2 && connectionString[1] == ':' && !connectionString.StartsWith(@"C:", StringComparison.OrdinalIgnoreCase);

                    if (isUnc)
                    {
                        issues.Add(new HealthIssue
                        {
                            Category = IssueCategory.NetworkPath,
                            Severity = IssueSeverity.Medium,
                            AffectedItem = layer.Name,
                            AffectedItemPath = mapItem.Name,
                            Description = $"Layer uses a UNC network path: {connectionString}",
                            Recommendation = "Consider storing data locally or in an enterprise geodatabase for better performance, or ensure robust network connectivity.",
                            AnalyzerName = this.Name
                        });
                        
                        // Check if accessible
                        if (!Directory.Exists(Path.GetDirectoryName(connectionString)) && !File.Exists(connectionString))
                        {
                            issues.Add(new HealthIssue
                            {
                                Category = IssueCategory.NetworkPath,
                                Severity = IssueSeverity.High,
                                AffectedItem = layer.Name,
                                AffectedItemPath = mapItem.Name,
                                Description = $"Network path is currently inaccessible: {connectionString}",
                                Recommendation = "Check network connection, VPN, or repair the broken data source.",
                                AnalyzerName = this.Name
                            });
                        }
                    }
                    else if (isMappedDrive)
                    {
                        issues.Add(new HealthIssue
                        {
                            Category = IssueCategory.NetworkPath,
                            Severity = IssueSeverity.Low,
                            AffectedItem = layer.Name,
                            AffectedItemPath = mapItem.Name,
                            Description = $"Layer uses a mapped network drive: {connectionString}",
                            Recommendation = "Mapped drives can cause portability issues across different user profiles. Use relative paths or enterprise geodatabases instead.",
                            AnalyzerName = this.Name
                        });
                    }
                }
            });
        }

        return issues;
    }
}
