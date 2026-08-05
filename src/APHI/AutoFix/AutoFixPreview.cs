using System;
using System.Collections.Generic;
using APHI.Models;

namespace APHI.AutoFix
{
    /// <summary>
    /// Represents a preview of the changes that will be made by an autofix operation.
    /// </summary>
    public class AutoFixPreview
    {
        /// <summary>
        /// Gets or sets the issue that is being previewed.
        /// </summary>
        public IssueModel Issue { get; set; }

        /// <summary>
        /// Gets or sets the description of the change that will be made.
        /// </summary>
        public string ChangeDescription { get; set; }

        /// <summary>
        /// Gets or sets the estimated time to complete the fix, in seconds.
        /// </summary>
        public int EstimatedTimeSeconds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the fix is recommended.
        /// </summary>
        public bool IsRecommended { get; set; }

        /// <summary>
        /// Generates a preview for a given issue and fixer.
        /// </summary>
        /// <param name="issue">The issue to fix.</param>
        /// <param name="fixerName">The name of the fixer.</param>
        /// <returns>An AutoFixPreview instance.</returns>
        public static AutoFixPreview GeneratePreview(IssueModel issue, string fixerName)
        {
            return new AutoFixPreview
            {
                Issue = issue,
                ChangeDescription = $"Applying {fixerName} to resolve '{issue.Title}'.",
                EstimatedTimeSeconds = 5,
                IsRecommended = true
            };
        }
    }
}
