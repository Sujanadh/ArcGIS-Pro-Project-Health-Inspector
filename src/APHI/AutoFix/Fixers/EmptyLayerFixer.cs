using System;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using APHI.Core.Models;

namespace APHI.AutoFix.Fixers
{
    /// <summary>
    /// Fixer for removing empty (no features) layers.
    /// </summary>
    public class EmptyLayerFixer : IFixer
    {
        /// <summary>
        /// Gets the name of the fixer.
        /// </summary>
        public string Name => "EmptyLayerFixer";

        /// <summary>
        /// Executes the empty layer removal fix.
        /// </summary>
        /// <param name="issue">The issue for the empty layer.</param>
        /// <returns>True if fixed, otherwise false.</returns>
        public async Task<bool> FixAsync(HealthIssue issue)
        {
            return await QueuedTask.Run(() =>
            {
                try
                {
                    if (issue.TargetObject is Layer layer)
                    {
                        var map = layer.Map;
                        if (map != null)
                        {
                            map.RemoveLayer(layer);
                            return true;
                        }
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }
    }
}
