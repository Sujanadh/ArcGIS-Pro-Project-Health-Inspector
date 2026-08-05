using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework;

namespace APHI.UI.Commands
{
    /// <summary>
    /// Command to trigger a scan from the ribbon.
    /// </summary>
    internal class RunScanCommand : Button
    {
        protected override void OnClick()
        {
            var pane = FrameworkApplication.DockPaneManager.Find("APHI_UI_InspectorDockpane") as InspectorDockpaneViewModel;
            if (pane != null)
            {
                pane.Activate();
                if (pane.ScanCommand.CanExecute(null))
                {
                    pane.ScanCommand.Execute(null);
                }
            }
        }
    }
}
