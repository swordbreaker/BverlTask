using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace Uebung1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var bytes = File.ReadAllBytes("Schlittentest.raw").Skip(1024).ToArray();

            var raw = new Image<Gray, byte>(800, 600);
            raw.Bytes = bytes;

            Task.Run(() => ImageViewer.Show(raw, "Raw"));

            CalculateAndShow(raw, BayerMask.Simple);
            CalculateAndShow(raw, BayerMask.NotSoSimple);
            CalculateAndShow(raw, BayerMask.NotSoSimple2);

            Console.ReadKey();
        }

        private static void CalculateAndShow(Image<Gray, byte> raw, Func<Image<Gray, byte>, Image<Rgb, byte>> algorithmn)
        {
            var img = algorithmn.Invoke(raw);

            Task.Run(() => ImageViewer.Show(img, algorithmn.Method.Name));
        }
    }
}
