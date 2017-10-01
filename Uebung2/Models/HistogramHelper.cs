using Emgu.CV;
using Emgu.CV.Structure;

namespace Uebung2.Models
{
    public class HistogramHelper
    {
        public static int[] Calc(Image<Gray, byte> img)
        {
            var hist = new int[265];

            var data = img.Data;

            for (int i = img.Rows - 1; i >= 0; i--)
            {
                for (int j = img.Cols - 1; j >= 0; j--)
                {
                    hist[data[i, j, 0]]++;
                }
            }

            return hist;
        }
    }
}
