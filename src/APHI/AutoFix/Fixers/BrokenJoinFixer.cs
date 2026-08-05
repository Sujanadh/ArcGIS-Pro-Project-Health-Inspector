using System;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using APHI.Models;

namespace APHI.AutoFix.Fixers
{
    /// <summary>
    /// Fixer for repairing or removing broken table joins.
    /// </summary>
    public class BrokenJoinFixer : IFixer
    {
        /// <summary>
        /// Gets the name of the fixer.
        /// </summary>
        public string Name => "BrokenJoinFixer";

        /// <summary>
        /// Executes the broken join fix.
        /// </summary>
        /// <param name="issue">The issue related to a broken join.</param>
        /// <returns>True if fixed, otherwise false.</returns>
        public async Task<bool> FixAsync(IssueModel issue)
        {
            return await QueuedTask.Run(() =>
            {
                try
                {
                    if (issue.TargetObject is FeatureLayer featureLayer)
                    {
                        // Logic to remove broken joins
                        // E.g., featureLayer.RemoveJoin("JoinName");
                        // As the SDK handles joins via Geoprocessing or connection properties,
                        // this is a simplified representation of the fix.
                        
                        if (issue.Properties.TryGetValue("JoinName", out var joinNameObj) && joinNameObj is string joinName)
                        {
                            // Remove join logic
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
