using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using APHI.Core.Models;
using APHI.Core.Interfaces;
using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace APHI.Analysis;

/// <summary>
/// Analyzes map labels for expression validity, complexity, and performance concerns.
/// </summary>
public class LabelAnalyzer : IAnalyzer
{
    public string Name => "Label Analyzer";
    public string Description => "Checks layer label expressions for errors and potential performance impacts.";
    public IssueCategory Category => IssueCategory.LabelIssue;

    public async Task<IReadOnlyList<HealthIssue>> AnalyzeAsync(AnalysisContext context, IProgress<ScanProgress> progress, CancellationToken cancellationToken)
    {
        var issues = new List<HealthIssue>();
        int totalItems = 0;
        int itemsProcessed = 0;
        var startTime = DateTime.Now;

        await QueuedTask.Run(() =>
        {
            foreach (var map in context.Maps)
            {
                totalItems += map.GetLayersAsFlattenedList().OfType<FeatureLayer>().Count();
            }
        });

        foreach (var map in context.Maps)
        {
            IReadOnlyList<FeatureLayer> featureLayers = null;

            await QueuedTask.Run(() =>
            {
                featureLayers = map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList();
            });

            foreach (var layer in featureLayers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                itemsProcessed++;

                progress?.Report(new ScanProgress
                {
                    CurrentOperation = "Checking label expressions",
                    CurrentItem = layer.Name,
                    ItemsProcessed = itemsProcessed,
                    TotalItems = totalItems,
                    PercentComplete = (double)itemsProcessed / totalItems * 100,
                    ElapsedTime = DateTime.Now - startTime,
                    Category = Category
                });

                try
                {
                    await QueuedTask.Run(() =>
                    {
                        var definition = layer.GetDefinition() as CIMFeatureLayer;
                        var labelClasses = definition?.LabelClasses;

                        if (labelClasses != null)
                        {
                            foreach (var labelClass in labelClasses)
                            {
                                if (labelClass.ExpressionEngine == LabelExpressionEngine.VBScript)
                                {
                                    issues.Add(new HealthIssue
                                    {
                                        Category = Category,
                                        Severity = IssueSeverity.Medium,
                                        Title = "Deprecated Label Engine",
                                        Description = $"Layer '{layer.Name}' uses VBScript for labeling, which is deprecated.",
                                        AffectedItem = layer.Name,
                                        AffectedItemPath = layer.URI,
                                        CurrentValue = LabelExpressionEngine.VBScript.ToString(),
                                        ExpectedValue = "Arcade or Python",
                                            Recommendation = "Migrate label expressions to Arcade or Python.",
                                            Impact = "VBScript expressions may cause errors or not run in future releases/enterprise environments.",
                                            EstimatedBenefit = "Modernizes project and ensures future compatibility.",
                                            IsAutoFixable = false,
                                            AutoFixDescription = string.Empty,
                                            MapName = map.Name,
                                            LayerName = layer.Name
                                        });
                                    }
                                    else if (labelClass.ExpressionEngine == LabelExpressionEngine.Python || labelClass.ExpressionEngine == LabelExpressionEngine.Arcade)
                                    {
                                        if (labelClass.Expression != null && labelClass.Expression.Length > 200)
                                        {
                                            issues.Add(new HealthIssue
                                            {
                                                Category = Category,
                                                Severity = IssueSeverity.Medium,
                                                Title = "Complex Label Expression",
                                                Description = $"Layer '{layer.Name}' has a highly complex label expression.",
                                                AffectedItem = layer.Name,
                                                AffectedItemPath = layer.URI,
                                                CurrentValue = "Length > 200 characters",
                                                ExpectedValue = "Simpler expression or pre-calculated field",
                                                Recommendation = "Consider calculating complex label strings into a new attribute field to improve drawing performance.",
                                                Impact = "Complex scripting per-feature slows down map drawing significantly.",
                                                EstimatedBenefit = "Faster map rendering.",
                                                IsAutoFixable = false,
                                                AutoFixDescription = string.Empty,
                                                MapName = map.Name,
                                                LayerName = layer.Name
                                            });
                                        }
                                    }
                                }
                            }
                    });
                }
                catch (Exception)
                {
                    // Ignore exception
                }
            }
        }

        return issues;
    }
}
