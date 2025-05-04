using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AdminPanel.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string status) return Brushes.Transparent;

            return status switch
            {
                "Подтвержден" => Brushes.LightBlue,
                "Активен" => Brushes.LightGreen,
                "Завершен" => Brushes.LightGray,
                "Отменен" => Brushes.LightCoral,
                _ => Brushes.White
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}