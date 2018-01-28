using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uebung11
{
    class Program
    {
        static void Main(string[] args)
        {
            var v = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            Fwt1D(v);
        }


        public static void Fwt1D(int[] v)
        {
            var m = new Matrix<float>(new float[,]
            {
                     { 6,     4,    -2,     0,     0 ,    0 ,    0 ,    0 },
                     {-1   ,  2   ,  6,     2 ,   -1 ,    0   ,  0 ,    0},
                     {0  ,   0 ,   -1  ,   2,     6   ,  2  ,  -1  ,   0},
                     { 0 ,    0  ,   0  ,   0 ,   -1  ,   2 ,    5 ,    2},
                     {-2 ,    4  ,  -2  ,   0 ,    0   ,  0 ,    0 ,    0},
                     {0   ,  0  ,  -2  ,   4 ,   -2  ,   0 ,    0 ,    0},
                     {0 ,    0   ,  0  ,   0  ,  -2  ,   4 ,   -2  ,   0},
                     {0  ,   0  ,   0   ,  0  ,   0   ,  0  ,  -4  ,   4},
            });

            var g = new Matrix<float>(new float[,] { { 1 }, { 2 }, { 3 }, { 4 }, { 5 }, { 6 }, { 7 }, { 8 } });

            var result = m * g;

            var rm = new Matrix<float>(new float[,]
            {
                {4     ,0     ,0     ,0     ,-4    ,0     ,0     ,0},
                {2     ,2     ,0     ,0     ,5     ,-1    ,0     ,0},
                {0     ,4     ,0     ,0     ,-2    ,-2    ,0     ,0},
                {0     ,2     ,2     ,0     ,-1    ,6     ,-1    ,0},
                {0     ,0     ,4     ,0     ,0     ,-2    ,-2    ,0},
                {0     ,0     ,2     ,2     ,0     ,-1    ,6     ,-1},
                {0     ,0     ,0     ,4     ,0     ,0     ,-2    ,-2},
                {0     ,0     ,0     ,4     ,0     ,0     ,-2    ,6},
            });


            var inv = (1/32f) * rm * result;
        }


        public static void Fwt(byte[,,] data)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                for (int i = 0; i < data.GetLength(0); i++)
                {

                }
            }
        }

        public static void IFwt(byte[,,] data)
        {
            const int ld8 = 3; // ld of factor 8
            var w = data.GetLength(1);
            var h = data.GetLength(0);

            Contract.Assert(w % 2 == 0);
            Contract.Assert(h % 2 == 0);

            for (int u = 0; u < w; u++)
            {
                byte p0 = data[u, 0, 0], p1 = data[u, 1, 0], p2 = data[u, 2, 0];
                // top border handling
                data[u, 0, 0] = (byte)((p0 - p1) >> ld8);
                data[u, 1, 0] = (byte)((2 * p0 + 5 * p1 + 2 * p2 - data[u, 3, 0]) >> (ld8 + 2));
                p0 = p1; p1 = p2;
                // middle part
                for (int v = 2; v < h - 2; v += 2)
                {
                    p2 = data[u, v + 1, 0];
                    data[u, v, 0] = (byte)((-p0 + 2 * p1 - p2) >> (ld8 + 1));
                    data[u, v + 1, 0] = (byte)((-p0 + 2 * p1 + 6 * p2 + 2 * data[u, v + 2, 0] - data[u, v + 3, 0]) >> (ld8 + 2));
                    p0 = p2;
                    p1 = data[u, v + 2, 0];
                }
                // bottom border handling
                p2 = data[u, h - 1, 0];
                data[u, h - 2, 0] = (byte)((-p0 + 2 * p1 - p2) >> (ld8 + 1));
                data[u, h - 1, 0] = (byte)((-p0 + 2 * p1 + 3 * p2) >> (ld8 + 1));
            }

        }


    }
}
