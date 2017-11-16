using System.Collections.Generic;
using System.Windows.Media;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Uebung8
{
    public static class FloodFillAlg
    {
        private static Image<Rgb, byte> _newImg;
        private static byte[,,] _data;
        private static byte[,,] _newData;

        private static Color[] _colors =
        {
            Colors.Blue, Colors.Red, Colors.Green, Colors.Yellow, Colors.Orange, Colors.Violet, Colors.Pink,
            Colors.YellowGreen, Colors.Aqua
        };

        public static Image<Rgb, byte> Run(Image<Gray, byte> img)
        {
            byte m = 2;

            _newImg = new Image<Rgb, byte>(img.Width, img.Height);
            _data = img.Data;
            _newData = _newImg.Data;

            for (int u = 0; u < img.Width; u++)
            {
                for (int v = 0; v < img.Height; v++)
                {
                    if (_data[v, u, 0] == 1)
                    {
                        FloodFill(u, v, m);
                        checked
                        {
                            m++;
                        }
                    }
                }
            }

            return _newImg;
        }

        private static void FloodFill(int u, int v, byte label)
        {
            var (w, h) = (_newImg.Width, _newImg.Height);
            var queue = new Queue<(int u, int v)>();
            queue.Enqueue((u,v));

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();

                if (x > 0 && x < w & y > 0 && y < h && _data[y, x, 0] == 1)
                {
                    _data[y, x, 0] = label;
                    var i = label % _colors.Length;

                    _newData[y, x, 0] = _colors[i].R;
                    _newData[y, x, 1] = _colors[i].G;
                    _newData[y, x, 2] = _colors[i].B;

                    queue.Enqueue((x + 1, y));
                    queue.Enqueue((x, y + 1));
                    queue.Enqueue((x, y - 1));
                    queue.Enqueue((x - 1, y));
                }
            }
        }
    }
}
