using ArcGIS.Desktop.Framework.Contracts;

namespace APHI.UI.Commands
{
    /// <summary>
    /// Command to open the Inspector DockPane.
    /// </summary>
    internal class OpenInspectorCommand : Button
    {
        protected override void OnClick()
        {
            InspectorDockpaneViewModel.Show();
        }
    }
}
