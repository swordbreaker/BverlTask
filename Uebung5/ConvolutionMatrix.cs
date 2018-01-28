using System;
using System.Linq;
using System.Windows.Controls;
using Emgu.CV;
using PointI = System.Drawing.Point;


namespace Uebung5
{
    public class ConvolutionMatrix : Matrix<float>
    {
        public enum MorphType
        {
            Dilation, Erosion
        }

        public PointI Hotspot { get; set; }

        public override int NumberOfChannels => 1;

        public override Array ManagedArray
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public ConvolutionMatrix(Matrix<float> matrix, PointI hostpot) : this(matrix.Data, hostpot) { }

        public ConvolutionMatrix(float[,] matrix, PointI hostpot) : base(matrix)
        {
            Hotspot = hostpot;
        }

        public new float this[int row, int col] => base[row + Hotspot.Y, col + Hotspot.X];

        public ConvolutionMatrix Normalized => this.Sum == 0 ? this : new ConvolutionMatrix(this / Sum, Hotspot);

        public static ConvolutionMatrix Convolute(ConvolutionMatrix a, ConvolutionMatrix b, PointI hotspot)
        {
            (var h, var w) = (b.Height, b.Width);
            (var hi, var hj) = (a.Hotspot.Y, a.Hotspot.X);

            var result = new float[h, w];
            var data = b.Data;

            for (int v = 0; v < h; v++)
            {
                for (int u = 0; u < w; u++)
                {
                    var sum = 0f;

                    for (int i = -hi; i < hi + 1; i++)
                    {
                        for (int j = -hj; j < hj + 1; j++)
                        {
                            var x = u + j;
                            var y = v + i;

                            if (x < 0) x = Math.Abs(x);
                            if (y < 0) y = Math.Abs(y);
                            if (x >= w) x = (w - (x - w)) - 1;
                            if (y >= h) y = (h - (y - h)) - 1;

                            sum += data[y, x] * b[i, j];
                        }
                    }

                    result[v, u] = sum;
                }
            }

            return new ConvolutionMatrix(result, hotspot);
        }

        public static void Clamp(float[,,] m)
        {
            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 0; j < m.GetLength(1); j++)
                {
                    for (int k = 0; k < m.GetLength(2); k++)
                    {
                        if (m[i, j, k] < 0) m[i, j, k] = 0;
                        if (m[i, j, k] > 255) m[i, j, k] = 255;
                    }
                }
            }
        }

        public static float[,,] operator *(ConvolutionMatrix a, float[,,] b)
        {
            (var h, var w, var depht) = (b.GetLength(0), b.GetLength(1), b.GetLength(2));
            (var hi, var hj) = (a.Hotspot.Y, a.Hotspot.X);
            var m = a.Normalized;

            var result = new float[h, w, depht];

            for (int v = 0; v < h; v++)
            {
                for (int u = 0; u < w; u++)
                {
                    var sum = new float[depht];
                    for (int i = -hi; i < hi + 1; i++)
                    {
                        for (int j = -hj; j < hj + 1; j++)
                        {
                            var x = u + j;
                            var y = v + i;

                            if (x < 0) x = Math.Abs(x);
                            if (y < 0) y = Math.Abs(y);
                            if (x >= w) x = (w - (x - w)) - 1;
                            if (y >= h) y = (h - (y - h)) - 1;

                            for (int k = 0; k < depht; k++)
                            {
                                sum[k] += b[y, x, k] * m[i, j];
                            }
                        }
                    }

                    for (int k = 0; k < depht; k++)
                    {
                        result[v, u, k] = sum[k];
                    }
                }
            }

            return result;
        }

        public static float[,,] operator +(ConvolutionMatrix a, float[,,] b)
        {
            return Morph(a, b, MorphType.Erosion);
        }

        public static float[,,] operator -(ConvolutionMatrix a, float[,,] b)
        {
            return Morph(a, b, MorphType.Dilation);
        }

        private static float[,,] Morph(ConvolutionMatrix a, float[,,] b, MorphType morphType)
        {
            (var h, var w, var depht) = (b.GetLength(0), b.GetLength(1), b.GetLength(2));
            (var hi, var hj) = (a.Hotspot.Y, a.Hotspot.X);
            var m = a;

            var result = new float[h, w, depht];

            for (int v = 0; v < h; v++)
            {
                for (int u = 0; u < w; u++)
                {
                    float[][] values = new float[depht][];
                    int c = 0;

                    for (int i = 0; i < depht; i++)
                    {
                        values[i] = new float[m.Rows * m.Cols];
                    }

                    for (int i = -hi; i < hi + 1; i++)
                    {
                        for (int j = -hj; j < hj + 1; j++, c++)
                        {
                            var x = u + j;
                            var y = v + i;

                            if (x < 0) x = Math.Abs(x);
                            if (y < 0) y = Math.Abs(y);
                            if (x >= w) x = (w - (x - w)) - 1;
                            if (y >= h) y = (h - (y - h)) - 1;
                            for (int k = 0; k < depht; k++)
                            {
                                switch (morphType)
                                {
                                    case MorphType.Dilation:
                                        values[k][c] = float.IsPositiveInfinity(m[i, j])
                                            ? float.MinValue
                                            : b[y, x, k] + m[i, j];
                                        break;
                                    case MorphType.Erosion:
                                        values[k][c] = float.IsPositiveInfinity(m[i, j])
                                            ? float.MaxValue
                                            : b[y, x, k] - m[i, j];
                                        break;
                                }
                                
                            }
                        }
                    }

                    for (int k = 0; k < depht; k++)
                    {
                        switch (morphType)
                        {
                            case MorphType.Dilation:
                                result[v, u, k] = values[k].Max();
                                break;
                            case MorphType.Erosion:
                                result[v, u, k] = values[k].Min();
                                break;
                        }
                    }
                }
            }

            return result;
        }
    }
}
