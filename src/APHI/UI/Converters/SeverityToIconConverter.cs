using System;
using System.Globalization;
using System.Windows.Data;

namespace APHI.UI.Converters
{
    /// <summary>
    /// Converts an IssueSeverity enum value to a corresponding icon path.
    /// </summary>
    public class SeverityToIconConverter : IValueConverter
    {
        /// <summary>
        /// Converts an IssueSeverity value to a pack:// URI string for the icon.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            string severity = value.ToString();
            switch (severity)
            {
                case "Critical": return "pack://application:,,,/APHI;component/Images/Critical16.png";
                case "High": return "pack://application:,,,/APHI;component/Images/High16.png";
                case "Medium": return "pack://application:,,,/APHI;component/Images/Medium16.png";
                case "Low": return "pack://application:,,,/APHI;component/Images/Low16.png";
                case "Information": default: return "pack://application:,,,/APHI;component/Images/Info16.png";
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
