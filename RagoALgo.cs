using System;

namespace RagoAlgo
{
    public class Rago
    {
        public static Bitmap histogram(Bitmap image, int[] magnitudes)
        {
            int maxMagnitude = magnitudes.Max();
            Bitmap histogram = new Bitmap(image.256, maxMagnitude);
            histogram.color = Color.White;

            Color grey = Color.FromArgb(128, 128, 128);
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < magnitudes[x]; y++)
                {
                    histogram.SetPixel(x, histogram.Height - 1 - y, grey);
                }
            }

            return histogram;
        }
    }
}