using System;
using System.Collections.Generic;
using APHI.Models;

namespace APHI.AutoFix
{
    /// <summary>
    /// Represents a log entry for a single fix operation.
    /// </summary>
    public class OperationLogEntry
    {
        /// <summary>
        /// Gets the timestamp of the operation.
        /// </summary>
        public DateTime Timestamp { get; } = DateTime.Now;

        /// <summary>
        /// Gets the issue that was addressed.
        /// </summary>
        public IssueModel Issue { get; set; }

        /// <summary>
        /// Gets a value indicating whether the fix was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets any error messages or additional information.
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Maintains a log of operations performed by the AutoFix engine.
    /// </summary>
    public class OperationLog
    {
        private readonly List<OperationLogEntry> _entries = new List<OperationLogEntry>();

        /// <summary>
        /// Gets the entries in the log.
        /// </summary>
        public IReadOnlyList<OperationLogEntry> Entries => _entries.AsReadOnly();

        /// <summary>
        /// Adds a new entry to the log.
        /// </summary>
        /// <param name="issue">The issue being addressed.</param>
        /// <param name="success">Whether the fix was successful.</param>
        /// <param name="message">Optional message.</param>
        public void AddEntry(IssueModel issue, bool success, string message = null)
        {
            _entries.Add(new OperationLogEntry
            {
                Issue = issue,
                Success = success,
                Message = message ?? (success ? "Operation completed successfully." : "Operation failed.")
            });
        }
    }
}
