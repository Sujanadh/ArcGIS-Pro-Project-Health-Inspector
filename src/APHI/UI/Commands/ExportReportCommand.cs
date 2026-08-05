using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework;
using Microsoft.Win32;

namespace APHI.UI.Commands
{
    /// <summary>
    /// Command to export the current report.
    /// </summary>
    internal class ExportReportCommand : Button
    {
        protected override void OnClick()
        {
            var pane = FrameworkApplication.DockPaneManager.Find("APHI_UI_InspectorDockpane") as InspectorDockpaneViewModel;
            if (pane != null)
            {
                if (pane.ExportHtmlCommand.CanExecute(null))
                {
                    pane.ExportHtmlCommand.Execute(null);
                }
            }
        }
    }
}
