using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace APHI.UI.Converters
{
    /// <summary>
    /// Converts a boolean value to a <see cref="Visibility"/> value.
    /// Supports parameter to invert the logic (e.g. parameter="Inverse" or "Invert").
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean to Visibility. True maps to Visible, False maps to Collapsed.
        /// If parameter is "Inverse", the logic is inverted.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = false;
            if (value is bool b)
            {
                boolValue = b;
            }

            bool inverse = false;
            if (parameter is string paramString && 
                (paramString.Equals("Inverse", StringComparison.OrdinalIgnoreCase) || 
                 paramString.Equals("Invert", StringComparison.OrdinalIgnoreCase)))
            {
                inverse = true;
            }

            if (inverse)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Converts Visibility back to boolean.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool boolValue = visibility == Visibility.Visible;
                
                bool inverse = false;
                if (parameter is string paramString && 
                    (paramString.Equals("Inverse", StringComparison.OrdinalIgnoreCase) || 
                     paramString.Equals("Invert", StringComparison.OrdinalIgnoreCase)))
                {
                    inverse = true;
                }

                return inverse ? !boolValue : boolValue;
            }
            return false;
        }
    }
}
