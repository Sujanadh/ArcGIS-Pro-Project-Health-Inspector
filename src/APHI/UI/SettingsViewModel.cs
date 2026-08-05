using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using APHI.Core.Models;

namespace APHI.UI
{
    /// <summary>
    /// View model for the Settings window.
    /// </summary>
    public class SettingsViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private ProjectSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel()
        {
            _settings = new ProjectSettings();
            SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);
            ResetCommand = new RelayCommand(ExecuteReset);
        }

        /// <summary>
        /// Gets or sets the project settings.
        /// </summary>
        public ProjectSettings Settings
        {
            get => _settings;
            set
            {
                if (_settings != value)
                {
                    _settings = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the command to save settings.
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Gets the command to cancel and close.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Gets the command to reset to defaults.
        /// </summary>
        public ICommand ResetCommand { get; }

        private bool CanExecuteSave(object parameter)
        {
            return string.IsNullOrEmpty(Error);
        }

        private void ExecuteSave(object parameter)
        {
            // Save logic
        }

        private void ExecuteCancel(object parameter)
        {
            // Cancel logic
        }

        private void ExecuteReset(object parameter)
        {
            Settings = new ProjectSettings(); // Reset to defaults
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        public string Error => null;

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        public string this[string columnName]
        {
            get
            {
                string result = null;
                // Add validation logic here based on column name if needed
                return result;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
