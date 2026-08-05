using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using APHI.Models;

namespace APHI.AutoFix
{
    /// <summary>
    /// Interface for all fixers in the AutoFix module.
    /// </summary>
    public interface IFixer
    {
        /// <summary>
        /// Gets the name of the fixer.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Executes the fix operation.
        /// </summary>
        /// <param name="issue">The issue to fix.</param>
        /// <returns>A boolean indicating success.</returns>
        Task<bool> FixAsync(IssueModel issue);
    }

    /// <summary>
    /// Engine for managing and executing automatic fixes for project issues.
    /// </summary>
    public class AutoFixEngine
    {
        private readonly List<IFixer> _fixers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoFixEngine"/> class.
        /// </summary>
        public AutoFixEngine()
        {
            _fixers = new List<IFixer>();
        }

        /// <summary>
        /// Registers a new fixer with the engine.
        /// </summary>
        /// <param name="fixer">The fixer to register.</param>
        public void RegisterFixer(IFixer fixer)
        {
            if (fixer != null && !_fixers.Contains(fixer))
            {
                _fixers.Add(fixer);
            }
        }

        /// <summary>
        /// Executes fixes for a list of issues.
        /// </summary>
        /// <param name="issues">The issues to fix.</param>
        /// <returns>A log of the operations performed.</returns>
        public async Task<OperationLog> FixIssuesAsync(IEnumerable<IssueModel> issues)
        {
            var log = new OperationLog();

            foreach (var issue in issues)
            {
                var fixer = _fixers.FirstOrDefault(f => f.Name == issue.FixerName);
                if (fixer != null)
                {
                    bool success = await fixer.FixAsync(issue);
                    log.AddEntry(issue, success);
                }
                else
                {
                    log.AddEntry(issue, false, "No appropriate fixer found.");
                }
            }

            return log;
        }
    }
}
