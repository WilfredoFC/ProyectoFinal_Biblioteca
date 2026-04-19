using System.Globalization;

namespace ProyectoFinal_Biblioteca.Converters
{
    public class BoolToReadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "✅ Leído" : "📖 Pendiente";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}