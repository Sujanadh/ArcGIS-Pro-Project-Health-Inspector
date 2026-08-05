using System;
using System.Text;
using APHI.Core.Models;

namespace APHI.Reporting
{
    /// <summary>
    /// Generates a CSV format health report.
    /// </summary>
    public class CsvReportGenerator : IReportGenerator
    {
        /// <summary>
        /// Generates a CSV report from the provided HealthReport model.
        /// </summary>
        /// <param name="report">The health report data.</param>
        /// <returns>The generated CSV content.</returns>
        public string Generate(HealthReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            var sb = new StringBuilder();

            // Write header
            sb.AppendLine("ProjectName,Timestamp,TotalScore,Severity,Category,Component,Title,Description,Recommendation");

            // Define base properties
            string projectName = EscapeCsv(report.ProjectName);
            string timestamp = report.ScanStartTime.ToString("yyyy-MM-dd HH:mm:ss");
            string totalScore = report.HealthScore != null ? report.HealthScore.OverallScore.ToString() : "N/A";

            if (report.Issues != null && report.Issues.Count > 0)
            {
                foreach (var issue in report.Issues)
                {
                    sb.Append($"{projectName},");
                    sb.Append($"{timestamp},");
                    sb.Append($"{totalScore},");
                    sb.Append($"{issue.Severity},");
                    sb.Append($"{EscapeCsv(issue.Category.ToString())},");
                    sb.Append($"{EscapeCsv(issue.AffectedItem)},");
                    sb.Append($"{EscapeCsv(issue.Title)},");
                    sb.Append($"{EscapeCsv(issue.Description)},");
                    sb.AppendLine($"{EscapeCsv(issue.Recommendation)}");
                }
            }
            else
            {
                // No issues found
                sb.AppendLine($"{projectName},{timestamp},{totalScore},Success,General,N/A,No Issues Found,Project is perfectly healthy.,None");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Escapes a field for CSV format.
        /// </summary>
        /// <param name="field">The field to escape.</param>
        /// <returns>The escaped field.</returns>
        private string EscapeCsv(string field)
        {
            if (string.IsNullOrEmpty(field)) return string.Empty;

            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                field = field.Replace("\"", "\"\"");
                return $"\"{field}\"";
            }

            return field;
        }
    }
}
