using System;
using System.Drawing;
using System.Linq;
namespace RagoAlgo
{
    public class Rago
    {
        public static Bitmap histogram(Bitmap image, int[] magnitudes)
        {
            int maxMagnitude = magnitudes.Max();
            Bitmap histogram = new Bitmap(256, maxMagnitude);
            using (Graphics g = Graphics.FromImage(histogram))
            {
                g.Clear(Color.White);
            }

            Color barColor = Color.Gray;
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < magnitudes[x]; y++)
                {
                    histogram.SetPixel(x, histogram.Height - 1 - y, barColor);
                }
            }

            return histogram;
        }
    }
}