import re

def fix_broken_path():
    path = "src/APHI/Analysis/BrokenPathAnalyzer.cs"
    with open(path, 'r') as f: content = f.read()
    content = content.replace("definition?.DataConnection", "definition?.FeatureTable?.DataConnection")
    content = content.replace("definition.DataConnection", "definition.FeatureTable.DataConnection")
    with open(path, 'w') as f: f.write(content)

def fix_duplicate_layer():
    path = "src/APHI/Analysis/DuplicateLayerAnalyzer.cs"
    with open(path, 'r') as f: content = f.read()
    content = content.replace("definition?.DataConnection", "definition?.FeatureTable?.DataConnection")
    content = content.replace("definition.DataConnection", "definition.FeatureTable.DataConnection")
    with open(path, 'w') as f: f.write(content)

fix_broken_path()
fix_duplicate_layer()
print("Fixed DataConnection references.")
