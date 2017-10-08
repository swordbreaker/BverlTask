using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace Uebung3
{
    public static class Lut
    {
        public static Mat ApplyLut(Image<Gray, byte> img, byte[] lut)
        {
            Contract.Assert(lut.Length == 256, "lut.Length == 256");

            var handle = GCHandle.Alloc(lut, GCHandleType.Pinned);
            var histMat = new Mat(1, 256, DepthType.Cv8U, 1, handle.AddrOfPinnedObject(), 256);

            var newImg = new Mat();
            CvInvoke.LUT(img, histMat, newImg);

            return newImg;
        }
    }
}
