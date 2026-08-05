using System;
using System.Linq;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using APHI.Core.Models;

namespace APHI.AutoFix.Fixers
{
    /// <summary>
    /// Fixer for removing duplicate layers in a map.
    /// </summary>
    public class DuplicateLayerFixer : IFixer
    {
        /// <summary>
        /// Gets the name of the fixer.
        /// </summary>
        public string Name => "DuplicateLayerFixer";

        /// <summary>
        /// Executes the duplicate layer removal fix.
        /// </summary>
        /// <param name="issue">The issue indicating a duplicate layer.</param>
        /// <returns>True if fixed, otherwise false.</returns>
        public async Task<bool> FixAsync(HealthIssue issue)
        {
            return await QueuedTask.Run(() =>
            {
                try
                {
                    if (issue.TargetObject is Layer duplicateLayer)
                    {
                        var map = duplicateLayer.Map;
                        if (map != null)
                        {
                            map.RemoveLayer(duplicateLayer);
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
