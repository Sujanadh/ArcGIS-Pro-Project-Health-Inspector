import re

path = "src/APHI/UI/InspectorDockpaneViewModel.cs"
with open(path, 'r') as f:
    content = f.read()

content = content.replace("new HealthScore {", "new APHI.Core.Models.HealthScore {")
content = content.replace("new PerformanceMetrics {", "new APHI.Core.Models.PerformanceMetrics {")

with open(path, 'w') as f:
    f.write(content)
print("Applied namespace fixes")
