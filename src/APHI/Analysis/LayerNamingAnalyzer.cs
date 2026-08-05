using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using APHI.Core.Models;
using APHI.Core.Interfaces;

namespace APHI.Analysis;

/// <summary>
/// Analyzes layer naming conventions to detect poorly named layers or draft versions.
/// </summary>
public class LayerNamingAnalyzer : IAnalyzer
{
    /// <inheritdoc />
    public string Name => "Layer Naming Analyzer";

    /// <inheritdoc />
    public string Description => "Checks for generically named, duplicated, or draft-named layers.";

    /// <inheritdoc />
    public IssueCategory Category => IssueCategory.LayerNaming;

    /// <inheritdoc />
    public async Task<IReadOnlyList<HealthIssue>> AnalyzeAsync(AnalysisContext context, IProgress<ScanProgress> progress, CancellationToken cancellationToken)
    {
        var issues = new List<HealthIssue>();
        var maps = context.Project.GetItems<MapProjectItem>().ToList();

        var genericNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Layer", "Layer 1", "Layer 2", "New Layer" };
        var draftKeywords = new[] { "Final", "Final Final", "Final_v2", "temp", "test", "delete me" };
        var copyRegex = new Regex(@"copy(\s\(\d+\))?|- copy", RegexOptions.IgnoreCase);
        var underscoreRegex = new Regex(@"^[a-zA-Z0-9]+_[a-zA-Z0-9_]+$");

        foreach (var mapItem in maps)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await QueuedTask.Run(() =>
            {
                var map = mapItem.GetMap();
                if (map == null) return;

                var layers = map.GetLayersAsFlattenedList();
                
                foreach (var layer in layers)
                {
                    string name = layer.Name;

                    if (genericNames.Contains(name))
                    {
                        issues.Add(CreateNamingIssue(name, layer, mapItem, "Generic name detected."));
                    }
                    else if (copyRegex.IsMatch(name))
                    {
                        issues.Add(CreateNamingIssue(name, layer, mapItem, "Copy indicator detected in name."));
                    }
                    else if (draftKeywords.Any(k => name.Contains(k, StringComparison.OrdinalIgnoreCase)))
                    {
                        issues.Add(CreateNamingIssue(name, layer, mapItem, "Draft or temporary indicator detected in name."));
                    }
                    else if (underscoreRegex.IsMatch(name))
                    {
                        issues.Add(new HealthIssue
                        {
                            Category = IssueCategory.LayerNaming,
                            Severity = IssueSeverity.Information,
                            AffectedItem = name,
                            AffectedItemPath = mapItem.Name,
                            Description = "Name appears to be auto-generated from a dataset with underscores.",
                            Recommendation = "Consider replacing underscores with spaces for a cleaner map legend.",
                            AnalyzerName = this.Name
                        });
                    }
                }
            }, cancellationToken);
        }

        return issues;
    }

    private HealthIssue CreateNamingIssue(string name, Layer layer, MapProjectItem mapItem, string description)
    {
        return new HealthIssue
        {
            Category = IssueCategory.LayerNaming,
            Severity = IssueSeverity.Low,
            AffectedItem = name,
            AffectedItemPath = mapItem.Name,
            Description = description,
            Recommendation = "Rename the layer to be more descriptive and user-friendly for map legends.",
            AnalyzerName = this.Name
        };
    }
}
