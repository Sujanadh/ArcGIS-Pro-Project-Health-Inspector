using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using APHI.Core.Models;

namespace APHI.UI
{
    /// <summary>
    /// View model wrapper for a <see cref="HealthIssue"/> to provide UI-specific properties.
    /// </summary>
    public class HealthIssueViewModel : INotifyPropertyChanged
    {
        private readonly HealthIssue _model;
        private bool _isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthIssueViewModel"/> class.
        /// </summary>
        /// <param name="model">The underlying health issue model.</param>
        public HealthIssueViewModel(HealthIssue model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        /// <summary>
        /// Gets the underlying health issue model.
        /// </summary>
        public HealthIssue Model => _model;

        /// <summary>
        /// Gets or sets a value indicating whether this issue is selected in the UI.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the issue's severity.
        /// </summary>
        public IssueSeverity Severity => _model.Severity;

        /// <summary>
        /// Gets the title of the issue.
        /// </summary>
        public string Title => _model.Title;

        /// <summary>
        /// Gets the description of the issue.
        /// </summary>
        public string Description => _model.Description;

        /// <summary>
        /// Gets the category of the issue.
        /// </summary>
        public IssueCategory Category => _model.Category;

        /// <summary>
        /// Gets the display name for the category.
        /// </summary>
        public string CategoryDisplayName => _model.Category.ToString();

        /// <summary>
        /// Gets the name of the affected item.
        /// </summary>
        public string AffectedItem => _model.AffectedItem;

        /// <summary>
        /// Gets a value indicating whether this issue can be automatically fixed.
        /// </summary>
        public bool IsAutoFixable => _model.IsAutoFixable;

        /// <summary>
        /// Gets the suggested fix for this issue.
        /// </summary>
        public string SuggestedFix => _model.SuggestedFix;

        /// <summary>
        /// Gets the severity color based on the severity.
        /// </summary>
        public Brush SeverityColor
        {
            get
            {
                switch (Severity)
                {
                    case IssueSeverity.Critical: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC3545"));
                    case IssueSeverity.High: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FD7E14"));
                    case IssueSeverity.Medium: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC107"));
                    case IssueSeverity.Low: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#17A2B8"));
                    case IssueSeverity.Information: default: return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6C757D"));
                }
            }
        }

        /// <summary>
        /// Gets the severity icon pack URI based on the severity.
        /// </summary>
        public string SeverityIcon
        {
            get
            {
                switch (Severity)
                {
                    case IssueSeverity.Critical: return "pack://application:,,,/APHI;component/Images/Critical16.png";
                    case IssueSeverity.High: return "pack://application:,,,/APHI;component/Images/High16.png";
                    case IssueSeverity.Medium: return "pack://application:,,,/APHI;component/Images/Medium16.png";
                    case IssueSeverity.Low: return "pack://application:,,,/APHI;component/Images/Low16.png";
                    case IssueSeverity.Information: default: return "pack://application:,,,/APHI;component/Images/Info16.png";
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
