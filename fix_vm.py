import re

path = "src/APHI/UI/InspectorDockpaneViewModel.cs"
with open(path, 'r') as f:
    content = f.read()

content = content.replace("new HealthIssueViewModel(issue)", "new HealthIssueViewModel(issue)") # already fine
content = content.replace("i.Issue", "i.Model")
content = content.replace("SelectedIssue.Issue", "SelectedIssue.Model")
content = content.replace("SelectedIssue.Refresh();", "")

with open(path, 'w') as f:
    f.write(content)
print("Fixed InspectorDockpaneViewModel")
