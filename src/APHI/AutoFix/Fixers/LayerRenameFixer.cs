using System;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using APHI.Models;

namespace APHI.AutoFix.Fixers
{
    /// <summary>
    /// Fixer for renaming layers that do not adhere to naming conventions.
    /// </summary>
    public class LayerRenameFixer : IFixer
    {
        /// <summary>
        /// Gets the name of the fixer.
        /// </summary>
        public string Name => "LayerRenameFixer";

        /// <summary>
        /// Executes the layer rename fix.
        /// </summary>
        /// <param name="issue">The issue with the incorrect layer name.</param>
        /// <returns>True if fixed, otherwise false.</returns>
        public async Task<bool> FixAsync(IssueModel issue)
        {
            return await QueuedTask.Run(() =>
            {
                try
                {
                    if (issue.TargetObject is Layer layer && issue.Properties.TryGetValue("SuggestedName", out var newNameObj) && newNameObj is string newName)
                    {
                        layer.SetName(newName);
                        return true;
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
