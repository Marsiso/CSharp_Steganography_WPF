using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CSharp_Steganography_WPF.ViewModels
{
    internal class ExtensionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is not null)
            { 
                ImageFormat? format = value as ImageFormat;
                if (format is null)
                    return string.Empty;
                if (format == ImageFormat.Png)
                    return ".png";
                return ".bmp";
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => null;
    }
}
