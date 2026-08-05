using System;
using System.Linq;
using System.Text;
using APHI.Core.Models;

namespace APHI.Reporting
{
    /// <summary>
    /// Generates a plain text format health report.
    /// </summary>
    public class TextReportGenerator : IReportGenerator
    {
        /// <summary>
        /// Generates a plain text report from the provided HealthReport model.
        /// </summary>
        /// <param name="report">The health report data.</param>
        /// <returns>The generated plain text content.</returns>
        public string Generate(HealthReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            var sb = new StringBuilder();

            sb.AppendLine("==================================================");
            sb.AppendLine("          ARCGIS PRO PROJECT HEALTH REPORT        ");
            sb.AppendLine("==================================================");
            sb.AppendLine($"Project Name: {report.ProjectName}");
            sb.AppendLine($"Date Generated: {report.Timestamp}");
            sb.AppendLine();
            
            sb.AppendLine("--- SUMMARY ---");
            sb.AppendLine(report.Summary ?? "No summary provided.");
            sb.AppendLine();

            if (report.Score != null)
            {
                sb.AppendLine("--- SCORES ---");
                sb.AppendLine($"Overall Score:   {report.Score.TotalScore}%");
                sb.AppendLine($"Performance:     {report.Score.PerformanceScore}%");
                sb.AppendLine($"Data Health:     {report.Score.DataScore}%");
                sb.AppendLine($"Layouts:         {report.Score.LayoutScore}%");
                sb.AppendLine($"Security:        {report.Score.SecurityScore}%");
                sb.AppendLine();
            }

            sb.AppendLine("--- DETAILED FINDINGS ---");

            if (report.Issues == null || report.Issues.Count == 0)
            {
                sb.AppendLine("No issues found. The project is perfectly healthy.");
            }
            else
            {
                var sortedIssues = report.Issues
                    .OrderBy(i => i.Severity)
                    .ThenBy(i => i.Category)
                    .ToList();

                foreach (var issue in sortedIssues)
                {
                    sb.AppendLine($"[{issue.Severity.ToString().ToUpper()}] {issue.Category} - {issue.Component}");
                    sb.AppendLine($"Title: {issue.Title}");
                    sb.AppendLine($"Description: {issue.Description}");
                    if (!string.IsNullOrEmpty(issue.Recommendation))
                    {
                        sb.AppendLine($"Recommendation: {issue.Recommendation}");
                    }
                    sb.AppendLine(new string('-', 50));
                }
            }

            sb.AppendLine();
            sb.AppendLine("End of Report");
            sb.AppendLine("==================================================");

            return sb.ToString();
        }
    }
}
