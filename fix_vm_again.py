import re

path = "src/APHI/UI/InspectorDockpaneViewModel.cs"
with open(path, 'r') as f:
    content = f.read()

# 1. Fix _analyzerService -> ProjectScanner
content = content.replace("private AnalyzerService _analyzerService;", "private APHI.Core.Services.ProjectScanner _projectScanner;")

# 2. Fix constructor instantiation
content = content.replace("_analyzerService = ServiceLocator.GetService<AnalyzerService>();", "_projectScanner = ServiceLocator.Current.GetInstance<APHI.Core.Services.ProjectScanner>();")

# 3. Fix ExecuteScanAsync
content = content.replace("_analyzerService.AnalyzeCurrentProjectAsync(progress);", "_projectScanner.ScanProjectAsync(progress, new System.Threading.CancellationToken());")

# 4. Fix HealthScore and PerformanceScore assignments
content = content.replace("HealthScore = results.HealthScore;", "HealthScore = results.HealthScore.OverallScore;")
content = content.replace("PerformanceScore = results.PerformanceScore;", "PerformanceScore = results.PerformanceScore.OverallPerformanceScore;")

# 5. Fix AutoFix logic
autofix_replacement = """                var fixer = new APHI.AutoFix.AutoFixEngine();
                // A full implementation would register fixers here
                var log = await fixer.FixIssuesAsync(new[] { SelectedIssue.Model });
                var logEntry = System.Linq.Enumerable.FirstOrDefault(log.Entries);
                if (logEntry != null && logEntry.Success)
                {
                    StatusMessage = "Fix applied successfully.";
                    SelectedIssue.Model.IsFixed = true;
                }
                else
                {
                    string err = logEntry != null ? logEntry.Message : "Unknown error";
                    StatusMessage = $"Fix failed: {err}";
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(err, "Auto-Fix Error");
                }"""

# Use regex to replace the old autofix logic inside ExecuteAutoFixAsync
pattern = r"var fixer = ServiceLocator\.GetService<APHI\.AutoFix\.AutoFixService>\(\);\s*var result = await fixer\.FixIssueAsync\(SelectedIssue\.Model\);\s*if \(result\.Success\)\s*\{\s*StatusMessage = \"Fix applied successfully\.\";\s*SelectedIssue\.Model\.IsFixed = true;\s*\}\s*else\s*\{\s*StatusMessage = \$\"Fix failed: \{result\.ErrorMessage\}\";\s*ArcGIS\.Desktop\.Framework\.Dialogs\.MessageBox\.Show\(result\.ErrorMessage, \"Auto-Fix Error\"\);\s*\}"

content = re.sub(pattern, autofix_replacement, content, flags=re.MULTILINE)

with open(path, 'w') as f:
    f.write(content)
print("Applied fixes to InspectorDockpaneViewModel.cs")
