import os

content = """using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Core;
using APHI.Core.Models;
using APHI.Core.Services;
using APHI.Reporting;

namespace APHI.UI
{
    public class InspectorDockpaneViewModel : DockPane, INotifyPropertyChanged
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
        private AnalyzerService _analyzerService;

        public InspectorDockpaneViewModel()
        {
            _analyzerService = ServiceLocator.GetService<AnalyzerService>();

            ScanCommand = new RelayCommand(async _ => await ExecuteScanAsync(), _ => !IsScanning);
            CancelScanCommand = new RelayCommand(_ => ExecuteCancelScan(), _ => IsScanning);
            ExportHtmlCommand = new RelayCommand(async _ => await ExecuteExportAsync("HTML"), _ => Issues.Count > 0);
            ExportCsvCommand = new RelayCommand(async _ => await ExecuteExportAsync("CSV"), _ => Issues.Count > 0);
            ExportJsonCommand = new RelayCommand(async _ => await ExecuteExportAsync("JSON"), _ => Issues.Count > 0);
            ExportTextCommand = new RelayCommand(async _ => await ExecuteExportAsync("TXT"), _ => Issues.Count > 0);
            AutoFixSelectedCommand = new RelayCommand(async _ => await ExecuteAutoFixAsync(), _ => SelectedIssue != null && SelectedIssue.IsAutoFixable);
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
            StatusMessage = "Initializing scan...";

            try
            {
                if (Project.Current != null)
                {
                    ProjectName = Project.Current.Name;
                }

                var progress = new Progress<int>(p => ScanProgress = p);
                
                var results = await _analyzerService.AnalyzeCurrentProjectAsync(progress);
                
                Issues.Clear();
                foreach (var issue in results.Issues)
                {
                    Issues.Add(new HealthIssueViewModel(issue));
                }
                
                HealthScore = results.HealthScore;
                PerformanceScore = results.PerformanceScore;
                CriticalCount = results.Issues.Count(i => i.Severity == IssueSeverity.Critical);
                HighCount = results.Issues.Count(i => i.Severity == IssueSeverity.High);
                MediumCount = results.Issues.Count(i => i.Severity == IssueSeverity.Medium);
                LowCount = results.Issues.Count(i => i.Severity == IssueSeverity.Low);
                InfoCount = results.Issues.Count(i => i.Severity == IssueSeverity.Info);
                ScanDuration = results.ScanDuration;

                ApplyFilters();
                StatusMessage = $"Scan complete. Found {results.Issues.Count} issues in {ScanDuration.TotalSeconds:F1}s.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Scan failed: {ex.Message}";
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message, "Scan Error");
            }
            finally
            {
                IsScanning = false;
                ScanProgress = 100;
                ProgressMessage = "Ready";
            }
        }

        private void ExecuteCancelScan()
        {
            _cancelScanRequested = true;
            StatusMessage = "Cancelling scan...";
        }

        private void ApplyFilters()
        {
            FilteredIssues = new ObservableCollection<HealthIssueViewModel>(
                Issues.Where(i => 
                    (string.IsNullOrEmpty(SearchText) || i.Title.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 || i.Description.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0) &&
                    (SelectedSeverityFilter == "All" || i.Severity.ToString() == SelectedSeverityFilter) &&
                    (SelectedCategoryFilter == "All" || i.Category.ToString() == SelectedCategoryFilter)
                )
            );
        }

        private async Task ExecuteExportAsync(string format)
        {
            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog();
                string ext = format.ToLower();
                dialog.FileName = $"HealthReport_{DateTime.Now:yyyyMMdd_HHmmss}.{ext}";
                dialog.DefaultExt = $".{ext}";
                dialog.Filter = $"{format} File|*.{ext}";

                if (dialog.ShowDialog() == true)
                {
                    var results = new ScanResult
                    {
                        Issues = Issues.Select(i => i.Issue).ToList(),
                        HealthScore = HealthScore,
                        PerformanceScore = PerformanceScore,
                        ScanDuration = ScanDuration,
                        ScanTime = DateTime.Now
                    };
                    
                    ReportGenerator generator = null;
                    if (format == "HTML") generator = new HtmlReportGenerator();
                    else if (format == "CSV") generator = new CsvReportGenerator();
                    else if (format == "JSON") generator = new JsonReportGenerator();
                    else generator = new TextReportGenerator();
                    
                    if (generator != null)
                    {
                        await generator.GenerateAsync(results, dialog.FileName);
                        ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show($"Report exported to {dialog.FileName}", "Export Successful");
                    }
                }
            }
            catch (Exception ex)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show($"Failed to export report: {ex.Message}", "Export Error");
            }
        }

        private async Task ExecuteAutoFixAsync()
        {
            if (SelectedIssue == null || !SelectedIssue.IsAutoFixable) return;
            
            var vm = new AutoFixPreviewViewModel(SelectedIssue.Issue);
            var window = new AutoFixPreviewWindow { DataContext = vm };
            
            if (window.ShowDialog() == true)
            {
                StatusMessage = "Applying fix...";
                var fixer = ServiceLocator.GetService<APHI.AutoFix.AutoFixService>();
                var result = await fixer.FixIssueAsync(SelectedIssue.Issue);
                if (result.Success)
                {
                    StatusMessage = "Fix applied successfully.";
                    SelectedIssue.Issue.IsFixed = true;
                    SelectedIssue.Refresh();
                }
                else
                {
                    StatusMessage = $"Fix failed: {result.ErrorMessage}";
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(result.ErrorMessage, "Auto-Fix Error");
                }
            }
        }

        private void ExecuteOpenSettings()
        {
            var vm = new SettingsViewModel();
            var window = new SettingsWindow { DataContext = vm };
            window.ShowDialog();
        }

        private void ExecuteCopyIssue()
        {
            if (SelectedIssue == null) return;
            System.Windows.Clipboard.SetText($"{SelectedIssue.Title}: {SelectedIssue.Description}");
        }

        private void ExecuteSelectAllFixable()
        {
            // Not strictly needed in a standard listview unless we have a checkbox for multi-select.
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == nameof(SearchText) || propertyName == nameof(SelectedSeverityFilter) || propertyName == nameof(SelectedCategoryFilter))
            {
                // Delay filter slightly if typing fast, but directly calling ApplyFilters is fine.
            }
        }

        internal static void Show()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find("APHI_UI_InspectorDockpane");
            if (pane == null) return;
            pane.Activate();
        }
    }
}
"""

with open("src/APHI/UI/InspectorDockpaneViewModel.cs", "w") as f:
    f.write(content)
