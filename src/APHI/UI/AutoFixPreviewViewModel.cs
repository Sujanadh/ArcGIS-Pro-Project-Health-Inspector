using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using APHI.Core.Models;

namespace APHI.UI
{
    /// <summary>
    /// View model for the Auto-Fix Preview window.
    /// </summary>
    public class AutoFixPreviewViewModel : INotifyPropertyChanged
    {
        private bool _isApplying;
        private double _fixProgress;
        private HealthIssueViewModel _selectedFix;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoFixPreviewViewModel"/> class.
        /// </summary>
        public AutoFixPreviewViewModel()
        {
            ProposedFixes = new ObservableCollection<HealthIssueViewModel>();
            ApplyCommand = new RelayCommand(ExecuteApply, CanExecuteApply);
            CancelCommand = new RelayCommand(ExecuteCancel);
            SelectAllCommand = new RelayCommand(ExecuteSelectAll);
            DeselectAllCommand = new RelayCommand(ExecuteDeselectAll);
        }

        /// <summary>
        /// Gets the collection of proposed fixes.
        /// </summary>
        public ObservableCollection<HealthIssueViewModel> ProposedFixes { get; }

        /// <summary>
        /// Gets or sets the currently selected fix.
        /// </summary>
        public HealthIssueViewModel SelectedFix
        {
            get => _selectedFix;
            set
            {
                if (_selectedFix != value)
                {
                    _selectedFix = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether fixes are currently being applied.
        /// </summary>
        public bool IsApplying
        {
            get => _isApplying;
            set
            {
                if (_isApplying != value)
                {
                    _isApplying = value;
                    OnPropertyChanged();
                    ((RelayCommand)ApplyCommand).RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the progress of applying fixes.
        /// </summary>
        public double FixProgress
        {
            get => _fixProgress;
            set
            {
                if (_fixProgress != value)
                {
                    _fixProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the command to apply selected fixes.
        /// </summary>
        public ICommand ApplyCommand { get; }

        /// <summary>
        /// Gets the command to cancel the auto-fix operation.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Gets the command to select all proposed fixes.
        /// </summary>
        public ICommand SelectAllCommand { get; }

        /// <summary>
        /// Gets the command to deselect all proposed fixes.
        /// </summary>
        public ICommand DeselectAllCommand { get; }

        private bool CanExecuteApply(object parameter)
        {
            return !IsApplying && ProposedFixes.Count > 0;
        }

        private void ExecuteApply(object parameter)
        {
            // Logic to apply fixes
            IsApplying = true;
            // Simulated applying
        }

        private void ExecuteCancel(object parameter)
        {
            // Logic to cancel
        }

        private void ExecuteSelectAll(object parameter)
        {
            foreach (var fix in ProposedFixes)
            {
                fix.IsSelected = true;
            }
        }

        private void ExecuteDeselectAll(object parameter)
        {
            foreach (var fix in ProposedFixes)
            {
                fix.IsSelected = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
