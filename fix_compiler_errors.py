import re

# 1. Add IsFixed to HealthIssue.cs
hi_path = "src/APHI/Core/Models/HealthIssue.cs"
with open(hi_path, "r") as f:
    hi_content = f.read()
if "public bool IsFixed" not in hi_content:
    hi_content = hi_content.replace("public string FixerName { get; set; } = string.Empty;", "public string FixerName { get; set; } = string.Empty;\n    public bool IsFixed { get; set; }")
    with open(hi_path, "w") as f:
        f.write(hi_content)

# 2. Fix InspectorDockpaneViewModel.cs
vm_path = "src/APHI/UI/InspectorDockpaneViewModel.cs"
with open(vm_path, "r") as f:
    vm_content = f.read()
vm_content = vm_content.replace("IssueSeverity.Info", "IssueSeverity.Information")
vm_content = vm_content.replace("var vm = new AutoFixPreviewViewModel(SelectedIssue.Model);", "var vm = new AutoFixPreviewViewModel();\n            vm.ProposedFixes.Add(SelectedIssue);")

with open(vm_path, "w") as f:
    f.write(vm_content)

print("Fixed compiler errors")
