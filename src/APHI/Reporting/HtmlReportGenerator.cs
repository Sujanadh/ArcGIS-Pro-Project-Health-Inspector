using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using APHI.Models;

namespace APHI.Reporting
{
    /// <summary>
    /// Generates an HTML format health report.
    /// </summary>
    public class HtmlReportGenerator : IReportGenerator
    {
        /// <summary>
        /// Generates an HTML report from the provided HealthReport model.
        /// </summary>
        /// <param name="report">The health report data.</param>
        /// <returns>The generated HTML content.</returns>
        public string Generate(HealthReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset=\"UTF-8\">");
            sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine($"    <title>Health Report - {System.Web.HttpUtility.HtmlEncode(report.ProjectName)}</title>");
            
            sb.AppendLine("    <style>");
            sb.AppendLine(GetCssStyles());
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class=\"container\">");
            
            // Header
            sb.AppendLine("        <header>");
            sb.AppendLine("            <div>");
            sb.AppendLine($"                <h1>{System.Web.HttpUtility.HtmlEncode(report.ProjectName)}</h1>");
            sb.AppendLine("                <p>ArcGIS Pro Project Health Report</p>");
            sb.AppendLine("            </div>");
            sb.AppendLine($"            <div>Generated: {report.Timestamp:yyyy-MM-dd HH:mm:ss}</div>");
            sb.AppendLine("        </header>");

            // Summary
            sb.AppendLine("        <div class=\"summary-card\">");
            sb.AppendLine("            <h2>Executive Summary</h2>");
            sb.AppendLine($"            <p>{System.Web.HttpUtility.HtmlEncode(report.Summary)}</p>");
            
            // Scores
            if (report.Score != null)
            {
                sb.AppendLine("            <div class=\"score-grid\">");
                AppendScoreItem(sb, "Overall Health", report.Score.TotalScore);
                AppendScoreItem(sb, "Performance", report.Score.PerformanceScore);
                AppendScoreItem(sb, "Data Health", report.Score.DataScore);
                AppendScoreItem(sb, "Layouts", report.Score.LayoutScore);
                AppendScoreItem(sb, "Security", report.Score.SecurityScore);
                sb.AppendLine("            </div>");
            }
            sb.AppendLine("        </div>");

            // Issues List
            if (report.Issues != null && report.Issues.Count > 0)
            {
                sb.AppendLine("        <div class=\"issues-section\">");
                sb.AppendLine("            <h2>Detailed Findings</h2>");
                
                var sortedIssues = report.Issues
                    .OrderBy(i => i.Severity == IssueSeverity.Critical ? 0 :
                                  i.Severity == IssueSeverity.Warning ? 1 :
                                  i.Severity == IssueSeverity.Info ? 2 : 3)
                    .ThenBy(i => i.Category).ToList();

                foreach (var issue in sortedIssues)
                {
                    string severityClass = issue.Severity.ToString().ToLower();
                    
                    sb.AppendLine($"            <details class=\"issue-item\">");
                    sb.AppendLine("                <summary>");
                    sb.AppendLine($"                    <span class=\"badge {severityClass}\">{issue.Severity}</span>");
                    sb.AppendLine($"                    <span class=\"issue-title\">{System.Web.HttpUtility.HtmlEncode(issue.Title)}</span>");
                    sb.AppendLine($"                    <span style=\"font-size: 0.8em; color: #7f8c8d;\">{System.Web.HttpUtility.HtmlEncode(issue.Category)} - {System.Web.HttpUtility.HtmlEncode(issue.Component)}</span>");
                    sb.AppendLine("                </summary>");
                    sb.AppendLine("                <div class=\"issue-content\">");
                    sb.AppendLine($"                    <p><strong>Description:</strong> {System.Web.HttpUtility.HtmlEncode(issue.Description)}</p>");
                    if (!string.IsNullOrEmpty(issue.Recommendation))
                    {
                        sb.AppendLine("                    <div class=\"recommendation-box\">");
                        sb.AppendLine("                        <strong>Recommendation:</strong>");
                        sb.AppendLine($"                        <p>{System.Web.HttpUtility.HtmlEncode(issue.Recommendation)}</p>");
                        sb.AppendLine("                    </div>");
                    }
                    sb.AppendLine("                </div>");
                    sb.AppendLine("            </details>");
                }
                sb.AppendLine("        </div>");
            }

            // Footer
            sb.AppendLine("        <footer>");
            sb.AppendLine("            <p>Generated by APHI - ArcGIS Pro Project Health Inspector</p>");
            sb.AppendLine("        </footer>");
            
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private void AppendScoreItem(StringBuilder sb, string title, int score)
        {
            string scoreClass = score >= 80 ? "high" : score >= 60 ? "medium" : "low";
            sb.AppendLine("                <div class=\"score-item\">");
            sb.AppendLine($"                    <h3>{System.Web.HttpUtility.HtmlEncode(title)}</h3>");
            sb.AppendLine($"                    <div class=\"score-value {scoreClass}\">{score}%</div>");
            sb.AppendLine("                </div>");
        }

        /// <summary>
        /// Reads the embedded or local CSS file to include in the HTML report.
        /// </summary>
        private string GetCssStyles()
        {
            try
            {
                // Try reading from file first if executing in same directory
                string cssPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "ReportStyles.css");
                if (File.Exists(cssPath))
                {
                    return File.ReadAllText(cssPath);
                }
                
                // Fallback direct file path from the project structure (for dev environment)
                string localDevPath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ReportStyles.css");
                if (File.Exists(localDevPath))
                {
                    return File.ReadAllText(localDevPath);
                }
            }
            catch
            {
                // Ignore exceptions and fall through to default fallback
            }

            // Fallback CSS if file not found
            return @"
            body { font-family: sans-serif; padding: 20px; }
            header { background: #2c3e50; color: white; padding: 20px; }
            .score-grid { display: flex; gap: 20px; }
            .score-item { padding: 15px; background: #eee; }
            .badge { padding: 4px 8px; border-radius: 4px; }
            .critical { background: #e74c3c; color: white; }
            .warning { background: #f39c12; color: white; }
            .info { background: #3498db; color: white; }
            .success { background: #2ecc71; color: white; }
            details { margin-bottom: 10px; border: 1px solid #ccc; padding: 10px; }
            summary { font-weight: bold; cursor: pointer; }
            ";
        }
    }
}
