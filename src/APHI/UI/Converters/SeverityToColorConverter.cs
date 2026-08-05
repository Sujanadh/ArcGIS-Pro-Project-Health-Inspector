using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace APHI.UI.Converters
{
    /// <summary>
    /// Converts an IssueSeverity enum value to a <see cref="SolidColorBrush"/>.
    /// </summary>
    public class SeverityToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts an IssueSeverity value to a solid color brush.
        /// Critical: #DC3545 (Red)
        /// High: #FD7E14 (Orange)
        /// Medium: #FFC107 (Yellow/Amber)
        /// Low: #17A2B8 (Teal)
        /// Information: #6C757D (Gray)
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new SolidColorBrush(Colors.Gray);

            string severity = value.ToString();

            switch (severity)
            {
                case "Critical":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC3545"));
                case "High":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FD7E14"));
                case "Medium":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC107"));
                case "Low":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#17A2B8"));
                case "Information":
                default:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6C757D"));
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
