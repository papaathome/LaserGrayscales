using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace As.Applications.Data
{
    [ValueConversion(typeof(double), typeof(int))]
    public class ConverterIntToDouble : IValueConverter
    {
        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return (value is int)
                ? System.Convert.ToDouble((int)value)
                : DependencyProperty.UnsetValue;
        }

        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return (value is double)
                ? System.Convert.ToInt32((double)value)
                : DependencyProperty.UnsetValue;
        }
    }
}
