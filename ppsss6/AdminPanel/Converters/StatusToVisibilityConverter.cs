using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AdminPanel.Converters
{
    public class StatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string status || parameter is not string expectedStatus)
                return Visibility.Collapsed;

            return status == expectedStatus ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}