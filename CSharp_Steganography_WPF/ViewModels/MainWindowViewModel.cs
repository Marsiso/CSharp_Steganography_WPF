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
        private long sourceImageSize;
        private long outputImageSize;

        public Bitmap SourceImage
        {
            get => sourceImage;
            set
            {
                OnPropertyChanged(ref sourceImage, value);
                MaxLength = (value.Width * value.Height) >> 1;
                using (var memoryStream = new MemoryStream(Convert.ToInt32(sourceImageSize)))
                {
                    value.Save(memoryStream, sourceImageFormat);
                    SourceImageSize = memoryStream.Length;
                }
            }
        }

        private ImageFormat sourceImageFormat;

        public ImageFormat SourceImageFormat { get => sourceImageFormat; set => OnPropertyChanged(ref sourceImageFormat, value); }

        private ImageFormat outputImageFormat;

        public ImageFormat OutputImageFormat { get => outputImageFormat; set => OnPropertyChanged(ref outputImageFormat, value); }

        private string sourceImagePath;

        public string SourceImagePath { get => sourceImagePath; set => OnPropertyChanged(ref sourceImagePath, value); }

        private string outputImagePath;

        public string OutputImagePath { get => outputImagePath; set => OnPropertyChanged(ref outputImagePath, value); }

        public long SourceImageSize { get => sourceImageSize; set => OnPropertyChanged(ref sourceImageSize, value); }

        public long OutputImageSize { get => outputImageSize; set => OnPropertyChanged(ref outputImageSize, value); }

        private Bitmap outputImage;

        public Bitmap OutputImage
        {
            get => outputImage; 
            set
            {
                OnPropertyChanged(ref outputImage, value);
                using (var memoryStream = new MemoryStream(Convert.ToInt32(sourceImageSize)))
                {
                    value.Save(memoryStream, sourceImageFormat);  
                    OutputImageSize = memoryStream.Length;
                }
            }
        }

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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MainWindowViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Text = string.Empty;
            SourceImageFormat = OutputImageFormat = ImageFormat.Png;
            SourceImagePath = OutputImagePath = "pack://application:,,,/Assets/Image_Transparent_Image.png";

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
                openFileDialog.Filter = "Image files (*.png)|*.png|Image files (*.bmp)|*.bmp";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                if (openFileDialog.ShowDialog() is false)
                    return;
                string extension = Path.GetExtension(openFileDialog.FileName);
                if (extension is ".png")
                    SourceImageFormat = ImageFormat.Png;
                else
                    SourceImageFormat = ImageFormat.Bmp;
                SourceImagePath = openFileDialog.FileName;
                SourceImage = new(SourceImagePath);
                SourceImageSize = new FileInfo(SourceImagePath).Length;
            }, () => true);
        }

        public ICommand CommandSaveToFile
        {
            get => new CommandHandler(() =>
            {
                SaveFileDialog saveFileDialog = new();
                saveFileDialog.Filter = "Image files (*.png)|*.png|Image files (*.bmp)|*.bmp";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                saveFileDialog.FilterIndex = 1;
                if (sourceImageFormat == ImageFormat.Bmp)
                {
                    saveFileDialog.FilterIndex++;
                }
                OutputImageFormat = ImageFormat.Png;
                if (saveFileDialog.ShowDialog() is false)
                    return;
                string extension = Path.GetExtension(saveFileDialog.FileName);
                if (extension is ".bmp")
                    OutputImageFormat = ImageFormat.Bmp;

                try
                {
                    OutputImagePath = saveFileDialog.FileName;
                    outputImage.Save(OutputImagePath, OutputImageFormat);
                }
                catch (Exception ex)
                {
                    Text = ex.Message;
                }
            }, () => true);
        }

        public ICommand CommandHideMessage
        {
            get => new CommandHandler(() =>
            {
                OutputImage = MessageIntoImage(text, sourceImage);
                OutputImagePath = string.Empty;
                OutputImageFormat = sourceImageFormat;
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
