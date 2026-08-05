import re

# 1. Update Config.daml
daml_path = "src/APHI/Config.daml"
with open(daml_path, 'r') as f:
    content = f.read()

# Fix buttons
content = content.replace('className="OpenInspectorButton"', 'className="UI.Commands.OpenInspectorCommand"')
content = content.replace('className="RunScanButton"', 'className="UI.Commands.RunScanCommand"')
content = content.replace('className="ExportReportButton"', 'className="UI.Commands.ExportReportCommand"')
content = content.replace('className="OpenSettingsButton"', 'className="UI.Commands.OpenSettingsCommand"')

# Fix dockpane
content = content.replace('id="APHI_InspectorDockpane"', 'id="APHI_UI_InspectorDockpane"')
content = content.replace('className="InspectorDockpane"', 'className="UI.InspectorDockpaneViewModel"')
content = content.replace('className="InspectorDockpaneView"', 'className="UI.InspectorDockpane"')

with open(daml_path, 'w') as f:
    f.write(content)

# 2. Create OpenSettingsCommand.cs
cmd_path = "src/APHI/UI/Commands/OpenSettingsCommand.cs"
cmd_content = """using ArcGIS.Desktop.Framework.Contracts;

namespace APHI.UI.Commands
{
    internal class OpenSettingsCommand : Button
    {
        protected override void OnClick()
        {
            var vm = new SettingsViewModel();
            var window = new SettingsWindow { DataContext = vm };
            window.ShowDialog();
        }
    }
}
"""
with open(cmd_path, 'w') as f:
    f.write(cmd_content)

print("Fixed UI configuration.")
