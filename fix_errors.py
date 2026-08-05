import os
import re

src_dir = 'src'

def fix_file(filepath):
    with open(filepath, 'r') as f:
        content = f.read()

    orig = content
    
    # Namespaces
    content = content.replace('using APHI.Models;', 'using APHI.Core.Models;')
    content = content.replace('using APHI.Interfaces;', 'using APHI.Core.Interfaces;')
    
    # Class usages
    content = content.replace('IssueModel', 'HealthIssue')
    
    # HealthIssue fields
    content = content.replace('ItemName', 'AffectedItem')
    content = content.replace('ItemPath', 'AffectedItemPath')
    
    # AnalyzeAsync signature
    content = content.replace('Task<IEnumerable<HealthIssue>> AnalyzeAsync', 'Task<IReadOnlyList<HealthIssue>> AnalyzeAsync')
    
    # Fix the missing Category vs IsAutoFixable
    category_map = {
        'DefinitionQueryAnalyzer': 'DefinitionQuery',
        'LayerNamingAnalyzer': 'LayerNaming',
        'MetadataAnalyzer': 'Metadata',
        'NetworkPathAnalyzer': 'NetworkPath',
        'PerformanceAnalyzer': 'Performance',
        'RasterOptimizationAnalyzer': 'RasterOptimization',
        'RelativePathAnalyzer': 'RelativePath',
        'SymbologyAnalyzer': 'Symbology',
        'LabelAnalyzer': 'LabelIssue'
    }
    
    class_name_match = re.search(r'public class (\w+) : IAnalyzer', content)
    if class_name_match:
        class_name = class_name_match.group(1)
        if class_name in category_map:
            cat = category_map[class_name]
            # Replace IsAutoFixable property if present
            content = re.sub(r'public bool IsAutoFixable\s*=>\s*(true|false);', f'public IssueCategory Category => IssueCategory.{cat};', content)
            
            # If Category is missing entirely and IsAutoFixable was not there
            if 'IssueCategory Category' not in content:
                content = re.sub(r'(public string Description\s*=>\s*.*?;)', r'\1\n\n    public IssueCategory Category => IssueCategory.' + cat + ';', content)

    # IAnalyzer requires IReadOnlyList but the methods might end in `return issues;` where issues is a List<HealthIssue>. 
    # List<T> implements IReadOnlyList<T> so it compiles, BUT IEnumerable doesn't. 
    # If the subagent returned `issues` it's fine.

    if content != orig:
        with open(filepath, 'w') as f:
            f.write(content)
        print(f"Fixed: {filepath}")

for root, dirs, files in os.walk(src_dir):
    for name in files:
        if name.endswith('.cs'):
            fix_file(os.path.join(root, name))
