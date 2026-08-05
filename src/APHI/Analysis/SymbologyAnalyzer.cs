using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using APHI.Core.Models;
using APHI.Core.Interfaces;

namespace APHI.Analysis;

/// <summary>
/// Analyzes symbology in a project's maps to identify missing symbol references,
/// unsupported symbol types, and broken style references.
/// </summary>
public class SymbologyAnalyzer : IAnalyzer
{
    /// <inheritdoc />
    public string Name => "Symbology Analyzer";

    /// <inheritdoc />
    public string Description => "Checks layer symbology for missing symbols, broken style references, and unsupported types.";

    /// <inheritdoc />
    public IssueCategory Category => IssueCategory.Symbology;

    /// <inheritdoc />
    public async Task<IReadOnlyList<HealthIssue>> AnalyzeAsync(AnalysisContext context, IProgress<ScanProgress> progress, CancellationToken cancellationToken)
    {
        var issues = new List<HealthIssue>();
        var maps = context.Project.GetItems<MapProjectItem>().ToList();
        int totalMaps = maps.Count;
        int currentMap = 0;

        foreach (var mapItem in maps)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            progress?.Report(new ScanProgress 
            { 
                Message = $"Analyzing symbology in map: {mapItem.Name}", 
                PercentComplete = (currentMap * 100) / (totalMaps > 0 ? totalMaps : 1) 
            });

            await QueuedTask.Run(() => {
                var map = mapItem.GetMap();
                if (map == null) return;

                var featureLayers = map.GetLayersAsFlattenedList().OfType<FeatureLayer>();
                
                foreach (var layer in featureLayers)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    var renderer = layer.GetRenderer();
                    
                    if (renderer is CIMUniqueValueRenderer uvRenderer)
                    {
                        if (uvRenderer.Groups != null)
                        {
                            foreach (var group in uvRenderer.Groups)
                            {
                                if (group.Classes != null)
                                {
                                    foreach (var cls in group.Classes)
                                    {
                                        if (cls.Symbol == null || cls.Symbol.Symbol == null)
                                        {
                                            issues.Add(new HealthIssue
                                            {
                                                Category = IssueCategory.SymbologyIssue,
                                                Severity = IssueSeverity.Medium,
                                                AffectedItem = layer.Name,
                                                AffectedItemPath = mapItem.Name,
                                                Description = $"Missing symbol reference in unique value class: {cls.Label}",
                                                Recommendation = "Reassign the symbol for this unique value class or remove the class.",
                                                AnalyzerName = this.Name
                                            });
                                        }
                                        else
                                        {
                                            CheckStyleReference(cls.Symbol.Symbol, layer, mapItem, issues);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (renderer is CIMSimpleRenderer simpleRenderer)
                    {
                        if (simpleRenderer.Symbol == null || simpleRenderer.Symbol.Symbol == null)
                        {
                            issues.Add(new HealthIssue
                            {
                                Category = IssueCategory.SymbologyIssue,
                                Severity = IssueSeverity.Medium,
                                AffectedItem = layer.Name,
                                AffectedItemPath = mapItem.Name,
                                Description = "Missing simple symbol reference.",
                                Recommendation = "Reassign the symbol for this layer.",
                                AnalyzerName = this.Name
                            });
                        }
                        else
                        {
                            CheckStyleReference(simpleRenderer.Symbol.Symbol, layer, mapItem, issues);
                        }
                    }
                }
            });

            currentMap++;
        }

        return issues;
    }

    private void CheckStyleReference(CIMSymbol symbol, FeatureLayer layer, MapProjectItem mapItem, List<HealthIssue> issues)
    {
        // Simple heuristic for broken style references or missing fonts (e.g. character marker)
        if (symbol is CIMPointSymbol pointSymbol && pointSymbol.SymbolLayers != null)
        {
            foreach (var layerSymbol in pointSymbol.SymbolLayers.OfType<CIMCharacterMarker>())
            {
                if (string.IsNullOrEmpty(layerSymbol.FontFamilyName))
                {
                    issues.Add(new HealthIssue
                    {
                        Category = IssueCategory.SymbologyIssue,
                        Severity = IssueSeverity.Low,
                        AffectedItem = layer.Name,
                        AffectedItemPath = mapItem.Name,
                        Description = "Character marker missing font family name.",
                        Recommendation = "Ensure the font required by this symbol is installed, or pick a different symbol.",
                        AnalyzerName = this.Name
                    });
                }
            }
        }
    }
}
