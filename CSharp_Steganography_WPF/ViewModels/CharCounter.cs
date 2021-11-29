using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CSharp_Steganography_WPF.ViewModels
{
    internal class CharCounter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is not null)
                return value.ToString().Length;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => null;


        //Content="{Binding Text, ElementName=TxtBox,Converter={StaticResource ResourceKey=CharCounter}}"
    }
}
