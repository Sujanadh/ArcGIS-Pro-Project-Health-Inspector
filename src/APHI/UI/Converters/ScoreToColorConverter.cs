using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace APHI.UI.Converters
{
    /// <summary>
    /// Converts a health score to a <see cref="SolidColorBrush"/> representing its status.
    /// </summary>
    public class ScoreToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a score to a solid color brush.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int score || (value is double doubleScore && int.TryParse(doubleScore.ToString(), out score)))
            {
                if (score >= 90) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#28A745"));
                if (score >= 75) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5CB85C"));
                if (score >= 60) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC107"));
                if (score >= 40) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FD7E14"));
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC3545"));
            }
            return new SolidColorBrush(Colors.Gray);
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
