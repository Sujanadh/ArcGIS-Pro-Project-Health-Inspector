using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using APHI.Core.Models;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Threading.Tasks;

namespace APHI.UI
{
    /// <summary>
    /// The main View Model for the Inspector DockPane.
    /// </summary>
    public class InspectorDockpaneViewModel : ArcGIS.Desktop.Framework.Contracts.DockPane, INotifyPropertyChanged
    {
        private ObservableCollection<HealthIssueViewModel> _issues = new ObservableCollection<HealthIssueViewModel>();
        private ObservableCollection<HealthIssueViewModel> _filteredIssues = new ObservableCollection<HealthIssueViewModel>();
        private HealthIssueViewModel _selectedIssue;
        private bool _isScanning;
        private double _scanProgress;
        private string _progressMessage;
        private int _healthScore;
        private int _performanceScore;
        private string _projectName;
        private string _searchText;
        private string _selectedSeverityFilter = "All";
        private string _selectedCategoryFilter = "All";
        private int _criticalCount;
        private int _highCount;
        private int _mediumCount;
        private int _lowCount;
        private int _infoCount;
        private TimeSpan _scanDuration;
        private string _statusMessage;
        private int _selectedTabIndex;
        private bool _cancelScanRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectorDockpaneViewModel"/> class.
        /// </summary>
        public InspectorDockpaneViewModel()
        {
            ScanCommand = new RelayCommand(async _ => await ExecuteScanAsync(), _ => !IsScanning);
            CancelScanCommand = new RelayCommand(_ => ExecuteCancelScan(), _ => IsScanning);
            ExportHtmlCommand = new RelayCommand(_ => ExecuteExport("HTML"), _ => Issues.Count > 0);
            ExportCsvCommand = new RelayCommand(_ => ExecuteExport("CSV"), _ => Issues.Count > 0);
            ExportJsonCommand = new RelayCommand(_ => ExecuteExport("JSON"), _ => Issues.Count > 0);
            ExportTextCommand = new RelayCommand(_ => ExecuteExport("TXT"), _ => Issues.Count > 0);
            AutoFixSelectedCommand = new RelayCommand(_ => ExecuteAutoFix(), _ => SelectedIssue != null && SelectedIssue.IsAutoFixable);
            OpenSettingsCommand = new RelayCommand(_ => ExecuteOpenSettings());
            CopyIssueCommand = new RelayCommand(_ => ExecuteCopyIssue(), _ => SelectedIssue != null);
            SelectAllFixableCommand = new RelayCommand(_ => ExecuteSelectAllFixable(), _ => FilteredIssues.Any(i => i.IsAutoFixable));
        }

        #region Properties
        public ObservableCollection<HealthIssueViewModel> Issues { get => _issues; set { _issues = value; OnPropertyChanged(); } }
        public ObservableCollection<HealthIssueViewModel> FilteredIssues { get => _filteredIssues; set { _filteredIssues = value; OnPropertyChanged(); } }
        public HealthIssueViewModel SelectedIssue { get => _selectedIssue; set { _selectedIssue = value; OnPropertyChanged(); } }
        public bool IsScanning { get => _isScanning; set { _isScanning = value; OnPropertyChanged(); } }
        public double ScanProgress { get => _scanProgress; set { _scanProgress = value; OnPropertyChanged(); } }
        public string ProgressMessage { get => _progressMessage; set { _progressMessage = value; OnPropertyChanged(); } }
        public int HealthScore { get => _healthScore; set { _healthScore = value; OnPropertyChanged(); } }
        public int PerformanceScore { get => _performanceScore; set { _performanceScore = value; OnPropertyChanged(); } }
        public string ProjectName { get => _projectName; set { _projectName = value; OnPropertyChanged(); } }
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); ApplyFilters(); } }
        public string SelectedSeverityFilter { get => _selectedSeverityFilter; set { _selectedSeverityFilter = value; OnPropertyChanged(); ApplyFilters(); } }
        public string SelectedCategoryFilter { get => _selectedCategoryFilter; set { _selectedCategoryFilter = value; OnPropertyChanged(); ApplyFilters(); } }
        public int CriticalCount { get => _criticalCount; set { _criticalCount = value; OnPropertyChanged(); } }
        public int HighCount { get => _highCount; set { _highCount = value; OnPropertyChanged(); } }
        public int MediumCount { get => _mediumCount; set { _mediumCount = value; OnPropertyChanged(); } }
        public int LowCount { get => _lowCount; set { _lowCount = value; OnPropertyChanged(); } }
        public int InfoCount { get => _infoCount; set { _infoCount = value; OnPropertyChanged(); } }
        public TimeSpan ScanDuration { get => _scanDuration; set { _scanDuration = value; OnPropertyChanged(); } }
        public string StatusMessage { get => _statusMessage; set { _statusMessage = value; OnPropertyChanged(); } }
        public int SelectedTabIndex { get => _selectedTabIndex; set { _selectedTabIndex = value; OnPropertyChanged(); } }
        #endregion

        #region Commands
        public ICommand ScanCommand { get; }
        public ICommand CancelScanCommand { get; }
        public ICommand ExportHtmlCommand { get; }
        public ICommand ExportCsvCommand { get; }
        public ICommand ExportJsonCommand { get; }
        public ICommand ExportTextCommand { get; }
        public ICommand AutoFixSelectedCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand CopyIssueCommand { get; }
        public ICommand SelectAllFixableCommand { get; }
        #endregion

        private async Task ExecuteScanAsync()
        {
            _cancelScanRequested = false;
            IsScanning = true;
            ProgressMessage = "Scanning project...";
            ScanProgress = 0;

            await QueuedTask.Run(async () =>
            {
                // Simulate scan
                for (int i = 0; i <= 100; i += 10)
                {
                    if (_cancelScanRequested) break;
                    ScanProgress = i;
                    await Task.Delay(100);
                }
            });

            IsScanning = false;
            StatusMessage = _cancelScanRequested ? "Scan cancelled" : "Scan complete";
        }

        private void ExecuteCancelScan()
        {
            _cancelScanRequested = true;
        }

        private void ApplyFilters()
        {
            // Simple filter logic
            FilteredIssues = new ObservableCollection<HealthIssueViewModel>(
                Issues.Where(i => 
                    (string.IsNullOrEmpty(SearchText) || i.Title.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0) &&
                    (SelectedSeverityFilter == "All" || i.Severity.ToString() == SelectedSeverityFilter) &&
                    (SelectedCategoryFilter == "All" || i.Category.ToString() == SelectedCategoryFilter)
                )
            );
        }

        private void ExecuteExport(string format) { }
        private void ExecuteAutoFix() { }
        private void ExecuteOpenSettings() { }
        private void ExecuteCopyIssue() { }
        private void ExecuteSelectAllFixable() { }

        public new event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Shows the pane.
        /// </summary>
        internal static void Show()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find("APHI_UI_InspectorDockpane");
            if (pane == null) return;
            pane.Activate();
        }
    }
}
