using Emgu.CV;
using Emgu.CV.Structure;

namespace Uebung3
{
    public static class HistAusgleich
    {
        public static Image<Gray, byte> Calc(Image<Gray, byte> old)
        {
            var hist = GetHitogram(old);
            var table = CreateTable(hist, old.Width * old.Height);

            return Lut.ApplyLut(old, table).ToImage<Gray, byte>();
        }

        public static int[] GetHitogram(Image<Gray, byte> img)
        {
            var hist = new int[256];

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

        public static byte[] CreateTable(int[] a, int N)
        {
            var b = new byte[256];
            var sum = 0;
            for (var i = 0; i < a.Length; i++)
            {
                sum += a[i];
                b[i] = (byte)(sum * (255f / N));
            }

            return b;
        }
    }
}
