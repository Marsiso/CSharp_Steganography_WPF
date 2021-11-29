﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                MaxLength = value.Width * value.Height * 3;
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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MainWindowViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            string? path = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName + @"\Assets\Image_Transparent_Image.png";
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            Bitmap bitmap = new(path);
            SourceImage = OutputImage = bitmap;
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
            int R = 0, G = 0, B = 0;

            for (int i = 0; i < bitmap.Height; i++)
                for (int j = 0; j < bitmap.Width; j++)
                {
                    Color pixel = bitmap.GetPixel(j, i);
                    R = pixel.R - pixel.R % 2;
                    G = pixel.G - pixel.G % 2;
                    B = pixel.B - pixel.B % 2;
                    for (var n = 0; n < 3; n++)
                    {
                        if (pxElementIndex % 8 is 0)
                        {
                            if (state is State.Filling_With_Zeros && trailingZeroes is 8)
                            {
                                if ((pxElementIndex - 1) % 3 < 2)
                                    bitmap.SetPixel(j, i, Color.FromArgb(R, G, B));
                                return bitmap;
                            }
                            if (charIndex >= msg.Length)
                                state = State.Filling_With_Zeros;
                            else
                                charAsValue = msg[charIndex++];
                        }

                        switch (pxElementIndex % 3)
                        {
                            case 0:
                                if (state is State.Hiding)
                                {
                                    R += charAsValue % 2;
                                    charAsValue >>= 1;
                                }
                                break;
                            case 1:
                                if (state is State.Hiding)
                                {
                                    G += charAsValue % 2;
                                    charAsValue >>= 1;
                                }
                                break;
                            case 2:
                                if (state is State.Hiding)
                                {
                                    B += charAsValue % 2;
                                    charAsValue >>= 1;
                                }
                                bitmap.SetPixel(j, i, Color.FromArgb(R, G, B));
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
                    for (var n = 0; n < 3; n++)
                    {
                        switch (pxElementIndex % 3)
                        {
                            case 0:
                                charAsValue = (charAsValue << 1) + pixel.R % 2;
                                break;
                            case 1:
                                charAsValue = (charAsValue << 1) + pixel.G % 2;
                                break;
                            case 2:
                                charAsValue = (charAsValue << 1) + pixel.B % 2;
                                break;
                            default:
                                break;
                        }

                        pxElementIndex++;
                        if (pxElementIndex % 8 is 0)
                        {
                            charAsValue = BitReversal(charAsValue);
                            if (charAsValue is 0)
                                return msg.ToString();

                            var c = (char)charAsValue;
                            msg.Append(c);
                        }
                    }
                }

            return msg.ToString();
        }

        public int BitReversal(int number)
        {
            var result = 0;
            for (var i = 0; i < 8; i++)
            {
                result = (result * 2) + (number % 2);
                number /= 2;
            }
            return result;
        }
    }
}