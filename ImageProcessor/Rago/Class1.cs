using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageProcessor.Rago
{
    public static class PixelFilters
    {
        // Delegate to define a per-pixel filter
        public delegate void PixelFilter(ref byte r, ref byte g, ref byte b);

        /// <summary>
        /// Applies a pixel-level filter to a bitmap using fast memory access (LockBits).
        /// </summary>
        /// 

        public enum ConvolutionFilter
        {
            Laplascian,
            GaussianBlur,
            Sharpen,
            EdgeDetect
        }

        public static void ApplyFiter(ref Bitmap source, PixelFilter filter)
        {
            int width = source.Width;
            int height = source.Height;

            Rectangle rect = new Rectangle(0, 0, width, height);

            BitmapData srcData = source.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = srcData.Stride;

            IntPtr srcScan0 = srcData.Scan0;

            
            unsafe
            {
                byte* pSrc = (byte*)srcScan0;
                for(int y = 0; y< height; y++)
                {
                    for(int x = 0; x < width; x++)
                    {
                        int idx = y * stride + x * 3;

                        byte b = pSrc[idx];
                        byte g = pSrc[idx + 1];
                        byte r = pSrc[idx + 2];

                        filter(ref r, ref g, ref b);
                        pSrc[idx] = b;
                        pSrc[idx + 1] = g;
                        pSrc[idx + 2] = r;
                    }
                }
            }
            source.UnlockBits(srcData);
        }

        public static void ApplyConvolutionFilter(Bitmap source, ConvolutionFilter filterType)
        {
            switch (filterType)
            {
                case ConvolutionFilter.Laplascian:
                    Laplascian(source);
                    break;
                // Implement other cases as needed
                default:
                    throw new NotImplementedException($"Filter {filterType} not implemented.");
            }
        }
         
        public static void ApplyConvolutionFilters(Bitmap source, PixelFilter filter) 
        {

        }

        // Example Filters:

        public static void Grayscale(ref byte r, ref byte g, ref byte b)
        {
            byte gray = (byte)((r + g + b) / 3);
            r = g = b = gray;
        }

        public static void Invert(ref byte r, ref byte g, ref byte b)
        {
            r = (byte)(255 - r);
            g = (byte)(255 - g);
            b = (byte)(255 - b);
        }

        public static void Sepia(ref byte r, ref byte g, ref byte b)
        {
            byte originalR = r, originalG = g, originalB = b;

            r = Clamp((int)(0.393 * originalR + 0.769 * originalG + 0.189 * originalB));
            g = Clamp((int)(0.349 * originalR + 0.686 * originalG + 0.168 * originalB));
            b = Clamp((int)(0.272 * originalR + 0.534 * originalG + 0.131 * originalB));
        }

        public static void Laplascian(Bitmap image)
        {
            AliacAlgo.AliacAlgo.LaplascianEmboss(image);
        }

        private static byte Clamp(int value)
        {
            return (byte)(value > 255 ? 255 : (value < 0 ? 0 : value));
        }
    }
}
