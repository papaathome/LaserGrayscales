using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using Image = System.Drawing.Image;

namespace As.Applications.Images
{
    internal class Picture
    {
        /// <summary>
        /// Bitmap image manager class
        /// </summary>
        /// <param name="image"></param>
        public Picture(Bitmap image)
        {
            Image = image;

            Width = image.Width; // pixels
            Height = image.Height; // pixels
            Resolution = image.HorizontalResolution;

            Sharp = true;
            Inverted = false;
            OneBitColor = false;

            Brightness = 50; // %
            Contrast = 50; // %
            Gamma = 50; // %
            WhiteOffset = 50; // %

            Preview = Interpolation();
        }

        public Bitmap Image { get; private set; }

        public Bitmap Preview { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public float Resolution { get; private set; }

        public bool Sharp { get; private set; }

        public bool Inverted { get; private set; }

        public bool OneBitColor { get; private set; }

        public int Brightness { get; private set; }

        public int Contrast { get; private set; }

        public int Gamma { get; private set; }

        public int WhiteOffset { get; private set; }

        public Bitmap Interpolation()
        {
            var num = (Image.Height < Image.Width)
                ? ((double)Width / Image.Width)
                : ((double)Height / Image.Height);
            var new_height = Convert.ToInt16(Convert.ToDouble(Image.Height) * num);
            var new_width = Convert.ToInt16(Convert.ToDouble(Image.Width) * num);
            var image = (Bitmap)Resize(Image, new_width, new_height, Sharp);
            image.SetResolution(Resolution, Resolution);

            return ApplyThresholdColor(
                Correct(
                    (Inverted) ? Invert(image) : image,
                    Brightness / 100f,
                    Contrast / 100f,
                    Gamma / 100f),
                WhiteOffset,
                OneBitColor);
        }

        static public Image Resize(Image image, int new_width, int new_height, bool sharp)
        {
            var bitmap = new Bitmap(new_width, new_height);
            var graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = (sharp)
                ? InterpolationMode.NearestNeighbor
                : InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(image, 0, 0, new_width, new_height);
            return bitmap;
        }

        static private Bitmap Invert(Bitmap image)
        {
            var bitmap = (Bitmap)image.Clone();
            var graphics = Graphics.FromImage(bitmap);
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var scan = bitmapData.Scan0;
            var num = Math.Abs(bitmapData.Stride) * bitmap.Height;
            var array = new byte[num];
            Marshal.Copy(scan, array, 0, num);
            for (int i = 0; i < array.Length; i += 4)
            {
                array[i] = (byte)(255 - array[i]);
                array[i + 1] = (byte)(255 - array[i + 1]);
                array[i + 2] = (byte)(255 - array[i + 2]);
            }
            Marshal.Copy(array, 0, scan, num);
            bitmap.UnlockBits(bitmapData);
            graphics.DrawImage(bitmap, image.Width, image.Height);
            graphics.Dispose();
            return bitmap;
        }

        static private Bitmap Correct(
            Bitmap image,
            float brightness,
            float contrast,
            float gamma)
        {
            Bitmap bitmap = new Bitmap(
                image.Width,
                image.Height) ?? throw new Exception($"Bitmap: cannot create a ({image.Width} x {image.Height}) bitmap");
            bitmap.SetResolution(
                image.HorizontalResolution,
                image.VerticalResolution);
            var graphics = Graphics.FromImage(bitmap);
            var num = brightness - 1f;
            float[][] color_matrix =
            [
                [contrast, 0f, 0f, 0f, 0f],
                [0f, contrast, 0f, 0f, 0f],
                [0f, 0f, contrast, 0f, 0f],
                [0f, 0f, 0f, 1f, 0f],
                [num, num, num, 0f, 1f]
            ];
            var imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(
                new ColorMatrix(color_matrix),
                ColorMatrixFlag.Default,
                ColorAdjustType.Bitmap);
            imageAttributes.SetGamma(
                gamma,
                ColorAdjustType.Bitmap);
            graphics.DrawImage(
                image,
                new Rectangle(
                    0, 0,
                    image.Width, image.Height),
                0, 0,
                image.Width, image.Height,
                GraphicsUnit.Pixel,
                imageAttributes);
            graphics.Dispose();
            //Refresh();
            return bitmap;
        }

        static private Bitmap ApplyThresholdColor(
            Bitmap image,
            int offset,
            bool one_bit_color)
        {
            var bitmap = (Bitmap)image.Clone();
            var graphics = Graphics.FromImage(bitmap);
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var scan = bitmapData.Scan0;
            var num = Math.Abs(bitmapData.Stride) * bitmap.Height;
            var array = new byte[num];
            Marshal.Copy(scan, array, 0, num);
            for (var i = 0; i < array.Length; i += 4)
            {
                var num2 = (array[i] + array[i + 1] + array[i + 2]) / 3f;
                if (offset >= 255)
                {
                    continue;
                }
                if (num2 >= offset || array[i + 3] < byte.MaxValue)
                {
                    array[i] = 250;
                    array[i + 1] = 206;
                    array[i + 2] = 135;
                    array[i + 3] = byte.MaxValue;
                }
                else if (one_bit_color)
                {
                    array[i] = 0;
                    array[i + 1] = 0;
                    array[i + 2] = 0;
                    if (array[i + 3] < byte.MaxValue)
                    {
                        array[i] = 250;
                        array[i + 1] = 206;
                        array[i + 2] = 135;
                        array[i + 3] = byte.MaxValue;
                    }
                }
            }
            Marshal.Copy(array, 0, scan, num);
            bitmap.UnlockBits(bitmapData);
            graphics.DrawImage(bitmap, image.Width, image.Height);
            graphics.Dispose();
            return bitmap;
        }
    }
}
