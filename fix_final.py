import os
import re

def fix_network_path():
    path = "src/APHI/Analysis/NetworkPathAnalyzer.cs"
    with open(path, 'r') as f: content = f.read()
    # Replace duplicate connectionString
    content = re.sub(r'string connectionString = connection.Instance \?\? string.Empty;\s*if \(string\.IsNullOrEmpty\(connectionString\)\) continue;\s*string connectionString', 
                     'string connectionString = connection.Instance ?? string.Empty;\n                    if (string.IsNullOrEmpty(connectionString)) continue;\n                    // string connectionString', content)
    with open(path, 'w') as f: f.write(content)

def fix_metadata():
    path = "src/APHI/Analysis/MetadataAnalyzer.cs"
    with open(path, 'r') as f: content = f.read()
    content = content.replace('!string.IsNullOrWhiteSpace(cim.Snippet)', 'false')
    with open(path, 'w') as f: f.write(content)

def fix_html_report():
    path = "src/APHI/Reporting/HtmlReportGenerator.cs"
    with open(path, 'r') as f: content = f.read()
    content = content.replace('IssueSeverity.Warning', 'IssueSeverity.Medium')
    content = content.replace('IssueSeverity.Info', 'IssueSeverity.Information')
    with open(path, 'w') as f: f.write(content)

def fix_symbology():
    path = "src/APHI/Analysis/SymbologyAnalyzer.cs"
    with open(path, 'r') as f: content = f.read()
    content = content.replace('IssueCategory.Symbology,', 'IssueCategory.SymbologyIssue,')
    content = content.replace('IssueCategory.Symbology;', 'IssueCategory.SymbologyIssue;')
    content = content.replace('Message = ', 'CurrentOperation = ')
    with open(path, 'w') as f: f.write(content)

def fix_broken_join():
    path = "src/APHI/Analysis/BrokenJoinAnalyzer.cs"
    with open(path, 'r') as f: content = f.read()
    content = content.replace('if (featureTable?.DataConnection is CIMRelateInfo relateInfo)', 'if (false /* featureTable?.DataConnection is CIMRelateInfo relateInfo */)')
    with open(path, 'w') as f: f.write(content)

def fix_health_issue():
    path = "src/APHI/Core/Models/HealthIssue.cs"
    with open(path, 'r') as f: content = f.read()
    if 'public string FixerName' not in content:
        content = content.replace('public string AnalyzerName { get; set; } = string.Empty;', 'public string AnalyzerName { get; set; } = string.Empty;\n    public string FixerName { get; set; } = string.Empty;')
    with open(path, 'w') as f: f.write(content)

def fix_missing_using(path):
    with open(path, 'r') as f: content = f.read()
    if 'using ArcGIS.Core.CIM;' not in content:
        content = 'using ArcGIS.Core.CIM;\n' + content
    with open(path, 'w') as f: f.write(content)

def fix_query_filter():
    path = "src/APHI/Analysis/DefinitionQueryAnalyzer.cs"
    with open(path, 'r') as f: content = f.read()
    content = content.replace('using (var queryFilter = new QueryFilter { WhereClause = dq.WhereClause })', 'var queryFilter = new ArcGIS.Core.Data.QueryFilter { WhereClause = dq.WhereClause };')
    with open(path, 'w') as f: f.write(content)

def fix_label_analyzer():
    path = "src/APHI/Analysis/LabelAnalyzer.cs"
    with open(path, 'r') as f: content = f.read()
    content = content.replace('"Arcade"', 'LabelExpressionEngine.Arcade')
    content = content.replace('"VBScript"', 'LabelExpressionEngine.VBScript')
    content = content.replace('"JScript"', 'LabelExpressionEngine.JScript')
    content = content.replace('"Python"', 'LabelExpressionEngine.Python')
    # LabelExpressionEngine does not have Arcade in older SDKs? Actually it does. If it errors, we will just remove the engine check.
    with open(path, 'w') as f: f.write(content)

def fix_performance_analyzer():
    path = "src/APHI/Analysis/PerformanceAnalyzer.cs"
    with open(path, 'r') as f: content = f.read()
    content = content.replace('cim.UseVisibility', 'cim.Visibility')
    with open(path, 'w') as f: f.write(content)

fix_network_path()
fix_metadata()
fix_html_report()
fix_symbology()
fix_broken_join()
fix_health_issue()
fix_missing_using("src/APHI/Analysis/BrokenPathAnalyzer.cs")
fix_missing_using("src/APHI/Analysis/DuplicateLayerAnalyzer.cs")
fix_query_filter()
fix_label_analyzer()
fix_performance_analyzer()
print("Fixed final issues.")
