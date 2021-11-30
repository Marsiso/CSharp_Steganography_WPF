using Microsoft.Win32;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CSharp_Steganography_WPF.ViewModels
{
    public sealed class MainWindowViewModel : BaseViewModel
    {
        private Bitmap sourceImage;

        public Bitmap SourceImage
        {
            get => sourceImage;
            set
            {
                OnPropertyChanged(ref sourceImage, value);
                MaxLength = (value.Width * value.Height) >> 1;
            }
        }

        private Bitmap outputImage;

        public Bitmap OutputImage { get => outputImage; set => OnPropertyChanged(ref outputImage, value); }

        private string text;

        public string Text { get => text; set => OnPropertyChanged(ref text, value); }

        private int maxLength;

        public int MaxLength { get => maxLength; private set => OnPropertyChanged(ref maxLength, value); }

        private string charCounter;

        public string CharCounter { get => charCounter; set => OnPropertyChanged(ref charCounter, value); }

        public enum State
        {
            Hiding,
            Filling_With_Zeros
        };

        public MainWindowViewModel()
        {
            var uri = new Uri("pack://application:,,,/Assets/Image_Transparent_Image.png");
            BitmapImage bitmapImage = new BitmapImage(uri);
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                bitmapEncoder.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);
                SourceImage = OutputImage = new Bitmap(bitmap);
            }
            Text = string.Empty;
            CharCounter = @"/" + maxLength.ToString();
        }

        [SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
        public Bitmap MessageIntoImage(string msg, Bitmap bitmap)
        {
            State state = State.Hiding;
            var charIndex = 0;
            var charAsValue = 0;
            long pxElementIndex = 0;
            var trailingZeroes = 0;
            int A = 0, R = 0, G = 0, B = 0;

            for (int i = 0; i < bitmap.Height; i++)
                for (int j = 0; j < bitmap.Width; j++)
                {
                    Color pixel = bitmap.GetPixel(j, i);
                    A = pixel.A - pixel.A % 2;
                    R = pixel.R - pixel.R % 2;
                    G = pixel.G - pixel.G % 2;
                    B = pixel.B - pixel.B % 2;

                    for (var n = 0; n < 4; n++)
                    {
                        if (pxElementIndex % 8 is 0)
                        {
                            if (state is State.Filling_With_Zeros && trailingZeroes is 8)
                            {
                                if ((pxElementIndex - 1) % 4 < 2)
                                    bitmap.SetPixel(j, i, Color.FromArgb(A, R, G, B));
                                return bitmap;
                            }
                            if (charIndex >= msg.Length)
                                state = State.Filling_With_Zeros;
                            else
                                charAsValue = msg[charIndex++];
                        }

                        switch (pxElementIndex % 4)
                        {
                            case 0:
                                if (state is State.Hiding)
                                {
                                    A += charAsValue % 2;
                                    charAsValue >>= 1;
                                }
                                break;
                            case 1:
                                if (state is State.Hiding)
                                {
                                    R += charAsValue % 2;
                                    charAsValue >>= 1;
                                }
                                break;
                            case 2:
                                if (state is State.Hiding)
                                {
                                    G += charAsValue % 2;
                                    charAsValue >>= 1;
                                }
                                break;
                            case 3:
                                if (state is State.Hiding)
                                {
                                    B += charAsValue % 2;
                                    charAsValue >>= 1;
                                }
                                bitmap.SetPixel(j, i, Color.FromArgb(A, R, G, B));
                                break;
                            default:
                                break;
                        }

                        pxElementIndex++;
                        if (state is State.Filling_With_Zeros)
                            trailingZeroes++;
                    }
                }

            return bitmap;
        }

        public string MessageFromImage(Bitmap source)
        {
            var pxElementIndex = 0;
            var charAsValue = 0;
            StringBuilder msg = new(string.Empty);

            for (var i = 0; i < source.Height; i++)
                for (var j = 0; j < source.Width; j++)
                {
                    Color pixel = source.GetPixel(j, i);
                    for (var n = 0; n < 4; n++)
                    {
                        switch (pxElementIndex % 4)
                        {
                            case 0:
                                charAsValue = (charAsValue << 1) + pixel.A % 2;
                                break;
                            case 1:
                                charAsValue = (charAsValue << 1) + pixel.R % 2;
                                break;
                            case 2:
                                charAsValue = (charAsValue << 1) + pixel.G % 2;
                                break;
                            case 3:
                                charAsValue = (charAsValue << 1) + pixel.B % 2;
                                break;
                            default:
                                break;
                        }

                        pxElementIndex++;
                        if (pxElementIndex % 8 is 0)
                        {
                            var result = 0;
                            for (var k = 0; k < 8; k++)
                            {
                                result = (result * 2) + (charAsValue % 2);
                                charAsValue /= 2;
                            }

                            charAsValue = result;
                            if (charAsValue is 0)
                                return msg.ToString();

                            var c = (char)charAsValue;
                            msg.Append(c);
                        }
                    }
                }

            return msg.ToString();
        }

        public ICommand CommandOpenFromFile
        {
            get => new CommandHandler(() =>
            {
                OpenFileDialog openFileDialog = new();
                openFileDialog.Filter = "Image file (*.png)|*.png|Image file (*.bmp)|*.bmp";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                if (openFileDialog.ShowDialog() is false)
                    return;
                SourceImage = new(openFileDialog.FileName);
            }, () => true);
        }

        public ICommand CommandSaveToFile
        {
            get => new CommandHandler(() =>
            {
                SaveFileDialog saveFileDialog = new();
                saveFileDialog.Filter = "Image file (*.png)|*.png|Image file (*.bmp)|*.bmp";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                ImageFormat format = ImageFormat.Png;
                if (saveFileDialog.ShowDialog() is false)
                    return;
                string extension = Path.GetExtension(saveFileDialog.FileName);
                if (extension is ".bmp")
                    format = ImageFormat.Bmp;
                outputImage.Save(saveFileDialog.FileName, format);
            }, () => true);
        }

        public ICommand CommandHideMessage
        {
            get => new CommandHandler(() =>
            {
                OutputImage = MessageIntoImage(text, sourceImage);
            }, () => true);
        }

        public ICommand CommandExtractMessage
        {
            get => new CommandHandler(() =>
            {
                Text = MessageFromImage(sourceImage);
            }, () => true);
        }
    }
}
