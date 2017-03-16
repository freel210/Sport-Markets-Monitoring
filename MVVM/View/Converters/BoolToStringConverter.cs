using System;
using System.Globalization;
using System.Windows.Data;

namespace View.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]

    public sealed class BoolToStringConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool f = (bool)value;

            if(f) return "*";

            return string.Empty;
        }
    }
}
