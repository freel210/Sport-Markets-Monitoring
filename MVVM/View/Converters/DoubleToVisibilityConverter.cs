using System;
using System.Windows.Data;
using System.Windows;

namespace View.Converters
{
    [ValueConversion(typeof(double), typeof(Visibility))]
    public sealed class DoubleToVisibilityConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double temp = (double)value;
            return temp >= 100 ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
