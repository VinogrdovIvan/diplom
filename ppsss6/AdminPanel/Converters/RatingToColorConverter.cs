using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AdminPanel.Converters
{
    public class RatingToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int rating)
            {
                return rating switch
                {
                    1 => Brushes.IndianRed,
                    2 => Brushes.Orange,
                    3 => Brushes.Gold,
                    4 => Brushes.LightGreen,
                    5 => Brushes.ForestGreen,
                    _ => Brushes.Gray
                };
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}