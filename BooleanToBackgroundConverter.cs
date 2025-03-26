using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfChatApp
{
    /// <summary>
    /// Конвертирует bool в Brush
    /// true → LightBlue (мои сообщения)
    /// false → White (чужие сообщения)
    /// </summary>
    public class BooleanToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isMyMessage)
                return isMyMessage ?
                    new SolidColorBrush(Colors.LightBlue) :
                    new SolidColorBrush(Colors.White);

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}