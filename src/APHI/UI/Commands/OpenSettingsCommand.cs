using ArcGIS.Desktop.Framework.Contracts;

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
