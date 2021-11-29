using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace CSharp_Steganography_WPF.ViewModels
{
    public sealed class ImageConverter : IValueConverter
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Bitmap bitmap = (Bitmap)value;
            if (value is null)
                throw new ArgumentException();

            return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapImage? bitmapImage = value as BitmapImage;
            if (bitmapImage is null)
            {
                throw new ArgumentException();
            }

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                bitmapEncoder.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }
    }
}
