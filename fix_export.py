import re

path = "src/APHI/UI/InspectorDockpaneViewModel.cs"
with open(path, 'r') as f:
    content = f.read()

export_replacement = """        private async Task ExecuteExportAsync(string format)
        {
            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog();
                string ext = format.ToLower();
                dialog.FileName = $"HealthReport_{DateTime.Now:yyyyMMdd_HHmmss}.{ext}";
                dialog.DefaultExt = $".{ext}";
                dialog.Filter = $"{format} File|*.{ext}";

                if (dialog.ShowDialog() == true)
                {
                    var report = new HealthReport
                    {
                        ProjectName = this.ProjectName,
                        Issues = Issues.Select(i => i.Model).ToList(),
                        HealthScore = new HealthScore { OverallScore = this.HealthScore },
                        PerformanceScore = new PerformanceMetrics { OverallPerformanceScore = this.PerformanceScore },
                        ScanStartTime = DateTime.Now - this.ScanDuration,
                        ScanEndTime = DateTime.Now
                    };
                    
                    var manager = new APHI.Reporting.ReportManager();
                    APHI.Reporting.ReportFormat rFormat = APHI.Reporting.ReportFormat.Text;
                    if (format == "HTML") rFormat = APHI.Reporting.ReportFormat.Html;
                    else if (format == "CSV") rFormat = APHI.Reporting.ReportFormat.Csv;
                    else if (format == "JSON") rFormat = APHI.Reporting.ReportFormat.Json;
                    
                    await manager.SaveReportAsync(report, dialog.FileName, rFormat);
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show($"Report exported to {dialog.FileName}", "Export Successful");
                }
            }
            catch (Exception ex)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show($"Failed to export report: {ex.Message}", "Export Error");
            }
        }"""

pattern = r"private async Task ExecuteExportAsync\(string format\)\s*\{[\s\S]*?catch \(Exception ex\)\s*\{\s*ArcGIS\.Desktop\.Framework\.Dialogs\.MessageBox\.Show\(\$\"Failed to export report: \{ex\.Message\}\", \"Export Error\"\);\s*\}\s*\}"

content = re.sub(pattern, export_replacement, content, flags=re.MULTILINE)

with open(path, 'w') as f:
    f.write(content)
print("Applied export fixes")
