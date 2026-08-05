import os
import re

src_dir = 'src'

def fix_file(filepath):
    with open(filepath, 'r') as f:
        content = f.read()
    orig = content

    # 1. AffectedAffectedItemPath -> AffectedItemPath
    content = content.replace('AffectedAffectedItemPath', 'AffectedItemPath')

    # 2. Fix CS0126 / CS1643 for QueuedTask.Run(() => ..., cancellationToken)
    # The SDK does not have QueuedTask.Run(Action, CancellationToken).
    # We remove the cancellationToken parameter from QueuedTask.Run.
    content = re.sub(r'QueuedTask\.Run\(\(\)\s*=>\s*\{(.*?)\},\s*cancellationToken\);', r'QueuedTask.Run(() => {\1});', content, flags=re.DOTALL)

    # 3. HealthIssueViewModel.cs SuggestedFix -> Recommendation
    if 'HealthIssueViewModel.cs' in filepath:
        content = content.replace('.SuggestedFix', '.Recommendation')

    # 4. InspectorDockpaneViewModel.cs missing using
    if 'InspectorDockpaneViewModel.cs' in filepath:
        if 'using ArcGIS.Desktop.Framework.Contracts;' not in content:
            content = content.replace('using System.Windows.Input;', 'using System.Windows.Input;\nusing ArcGIS.Desktop.Framework.Contracts;')

    # 5. NetworkPathAnalyzer.cs missing GetWorkspace
    if 'NetworkPathAnalyzer.cs' in filepath:
        # GetWorkspace() on BasicFeatureLayer is not standard, we should use layer.GetTable()?.GetDatastore()
        content = content.replace(
            'var connection = featureLayer.GetWorkspace()?.GetConnectionProperties();',
            'var connection = featureLayer.GetTable()?.GetDatastore()?.GetConnector() as ArcGIS.Core.Data.DatabaseConnectionProperties;\n                    if (connection == null) continue;\n                    string connectionString = connection.Instance ?? string.Empty;'
        )

    # 6. MetadataAnalyzer.cs missing Snippet and Description
    if 'MetadataAnalyzer.cs' in filepath:
        # BasicFeatureLayer does not have Snippet and Description directly.
        # We will use the layer's CIM definition.
        content = re.sub(r'string snippet = featureLayer\.Snippet;.*?bool hasDescription = !string\.IsNullOrWhiteSpace\(snippet\) \|\| !string\.IsNullOrWhiteSpace\(description\);',
                         'var cim = featureLayer.GetDefinition() as ArcGIS.Core.CIM.CIMFeatureLayer;\n                        bool hasDescription = cim != null && (!string.IsNullOrWhiteSpace(cim.Description) || !string.IsNullOrWhiteSpace(cim.Snippet));',
                         content, flags=re.DOTALL)

    # 7. PerformanceAnalyzer.cs IsLabelingEnabled -> CIM check
    if 'PerformanceAnalyzer.cs' in filepath:
        content = re.sub(r'if \(featureLayer\.IsLabelingEnabled\)\s*\{\s*score -= 2;\s*\}',
                         'var cim = featureLayer.GetDefinition() as ArcGIS.Core.CIM.CIMFeatureLayer;\n                        if (cim != null && cim.UseVisibility) { score -= 2; }',
                         content, flags=re.DOTALL)

    # 8. LabelAnalyzer.cs missing IsLabelClassesEnabled and CIMExpressionEngine
    if 'LabelAnalyzer.cs' in filepath:
        content = content.replace('if (featureLayer.IsLabelClassesEnabled)', 'var cim = featureLayer.GetDefinition() as ArcGIS.Core.CIM.CIMFeatureLayer;\n                    if (cim != null && cim.UseVisibility)')
        content = content.replace('CIMExpressionEngine.Arcade', '"Arcade"')
        content = content.replace('CIMExpressionEngine.VBScript', '"VBScript"')
        content = content.replace('CIMExpressionEngine.JScript', '"JScript"')
        content = content.replace('CIMExpressionEngine.Python', '"Python"')

    if content != orig:
        with open(filepath, 'w') as f:
            f.write(content)
        print(f"Fixed: {filepath}")

for root, dirs, files in os.walk(src_dir):
    for name in files:
        if name.endswith('.cs'):
            fix_file(os.path.join(root, name))
