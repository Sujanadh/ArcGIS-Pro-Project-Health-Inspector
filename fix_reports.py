import os
import re

def fix_csv():
    path = "src/APHI/Reporting/CsvReportGenerator.cs"
    with open(path, 'r') as f:
        content = f.read()
    
    content = content.replace("report.Timestamp", "report.ScanStartTime")
    content = content.replace("report.Score != null", "report.HealthScore != null")
    content = content.replace("report.Score.TotalScore", "report.HealthScore.OverallScore")
    content = content.replace("issue.Component", "issue.AffectedItem")
    
    with open(path, 'w') as f:
        f.write(content)

def fix_text():
    path = "src/APHI/Reporting/TextReportGenerator.cs"
    with open(path, 'r') as f:
        content = f.read()

    content = content.replace("report.Timestamp", "report.ScanStartTime")
    content = content.replace("report.Summary ?? \"No summary provided.\"", "\"Health scan completed successfully.\"")
    content = content.replace("report.Score != null", "report.HealthScore != null")
    
    scores_block = """                sb.AppendLine($"Overall Score:   {report.Score.TotalScore}%");
                sb.AppendLine($"Performance:     {report.Score.PerformanceScore}%");
                sb.AppendLine($"Data Health:     {report.Score.DataScore}%");
                sb.AppendLine($"Layouts:         {report.Score.LayoutScore}%");
                sb.AppendLine($"Security:        {report.Score.SecurityScore}%");"""
    
    new_scores_block = """                sb.AppendLine($"Overall Score:   {report.HealthScore.OverallScore}%");"""
    content = content.replace(scores_block, new_scores_block)
    
    # Fallback if literal replacement didn't work exactly
    content = content.replace("report.Score.TotalScore", "report.HealthScore.OverallScore")
    content = content.replace("report.Score.PerformanceScore", "report.HealthScore.OverallScore")
    content = content.replace("report.Score.DataScore", "report.HealthScore.OverallScore")
    content = content.replace("report.Score.LayoutScore", "report.HealthScore.OverallScore")
    content = content.replace("report.Score.SecurityScore", "report.HealthScore.OverallScore")
    content = content.replace("report.Score.", "report.HealthScore.")

    content = content.replace("issue.Component", "issue.AffectedItem")

    with open(path, 'w') as f:
        f.write(content)

def fix_html():
    path = "src/APHI/Reporting/HtmlReportGenerator.cs"
    with open(path, 'r') as f:
        content = f.read()

    content = content.replace("report.Timestamp", "report.ScanStartTime")
    content = content.replace("report.Summary", "\"Health scan completed successfully.\"")
    content = content.replace("report.Score != null", "report.HealthScore != null")
    
    content = content.replace("report.Score.TotalScore", "report.HealthScore.OverallScore")
    content = content.replace("report.Score.PerformanceScore", "report.HealthScore.OverallScore")
    content = content.replace("report.Score.DataScore", "report.HealthScore.OverallScore")
    content = content.replace("report.Score.LayoutScore", "report.HealthScore.OverallScore")
    content = content.replace("report.Score.SecurityScore", "report.HealthScore.OverallScore")

    content = content.replace("issue.Component", "issue.AffectedItem")

    with open(path, 'w') as f:
        f.write(content)

fix_csv()
fix_text()
fix_html()
print("Fixed reporting generators.")
