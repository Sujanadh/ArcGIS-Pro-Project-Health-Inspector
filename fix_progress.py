import re

path = "src/APHI/UI/InspectorDockpaneViewModel.cs"
with open(path, 'r') as f:
    content = f.read()

content = content.replace("var progress = new Progress<int>(p => ScanProgress = p);", "var progress = new Progress<ScanProgress>(p => { ScanProgress = p.PercentComplete; ProgressMessage = p.CurrentOperation; });")

with open(path, 'w') as f:
    f.write(content)
print("Applied progress fix")
