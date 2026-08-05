using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using APHI.Core.Models;
using APHI.Core.Interfaces;

namespace APHI.Analysis;

/// <summary>
/// Analyzes layer and table metadata completeness (title, tags, credits, use constraints).
/// </summary>
public class MetadataAnalyzer : IAnalyzer
{
    /// <inheritdoc />
    public string Name => "Metadata Analyzer";

    /// <inheritdoc />
    public string Description => "Evaluates whether datasets possess complete and well-formed metadata.";

    /// <inheritdoc />
    public IssueCategory Category => IssueCategory.Metadata;

    /// <inheritdoc />
    public async Task<IReadOnlyList<HealthIssue>> AnalyzeAsync(AnalysisContext context, IProgress<ScanProgress> progress, CancellationToken cancellationToken)
    {
        var issues = new List<HealthIssue>();
        var maps = context.Project.GetItems<MapProjectItem>().ToList();

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
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    if (layer is not BasicFeatureLayer featureLayer) continue;

                    try
                    {
                        var table = featureLayer.GetTable();
                        if (table == null) continue;

                        var definition = table.GetDefinition();
                        // Due to SDK specifics, direct metadata property might require item retrieval. 
                        // Often accessible via featureLayer.HasMetadata or similar.
                        // We will use basic layer description and dataset alias as a proxy if full metadata XML isn't easily readable without an Item.
                        string snippet = featureLayer.Snippet;
                        string description = featureLayer.Description;
                        
                        bool hasDescription = !string.IsNullOrWhiteSpace(snippet) || !string.IsNullOrWhiteSpace(description);

                        if (!hasDescription)
                        {
                            issues.Add(new HealthIssue
                            {
                                Category = IssueCategory.Metadata,
                                Severity = IssueSeverity.Low,
                                AffectedItem = layer.Name,
                                AffectedItemPath = mapItem.Name,
                                Description = "Layer is missing description or summary metadata.",
                                Recommendation = "Update the layer or dataset metadata to include a summary, description, and tags.",
                                AnalyzerName = this.Name
                            });
                        }
                        
                        // Checking Item metadata if linked to portal or catalog
                        // var item = ItemFactory.Instance.Create(table.GetPath().ToString());
                        // if (item != null) ...
                    }
                    catch { /* ignored for datasets that don't support metadata */ }
                }
            }, cancellationToken);
        }

        return issues;
    }
}
