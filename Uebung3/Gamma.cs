using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Uebung3
{
    public class Gamma
    {
        public static Image<Gray, byte> Calc(Image<Gray, byte> old, double gamma, double x0)
        {
            var s = gamma / (x0 * (gamma - 1) + Math.Pow(x0, 1 - gamma));
            var d = 1 / (Math.Pow(x0, gamma) * (gamma - 1) + 1) - 1;

            var table = new byte[256];

            for (int i = 0; i < 256; i++)
            {
                var x = i / 255d;
                if (x < x0)
                {
                    table[i] = (byte) (s * x * 255);
                }
                else
                {
                    table[i] = (byte)(((1 + d) * Math.Pow(x, gamma) - d)*255);
                }
            }

            var mat = Lut.ApplyLut(old, table);
            return mat.ToImage<Gray, byte>();
        }
    }
}
