using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Uebung12
{
    class Program
    {
        struct PatternDiffrence : IComparable<PatternDiffrence>, IComparable, IComparer<PatternDiffrence>
        {
            public readonly Rectangle rect;
            public readonly double cl;

            public PatternDiffrence(Rectangle rect, double cl)
            {
                this.rect = rect;
                this.cl = cl;
            }

            public int Compare(PatternDiffrence x, PatternDiffrence y)
            {
                return x.CompareTo(y);
            }

            public int CompareTo(PatternDiffrence other)
            {
                return cl.CompareTo(other.cl);
            }

            public int CompareTo(object obj)
            {
                return CompareTo((PatternDiffrence)obj);
            }
        }

        static void Main(string[] args)
        {
            var img = new Image<Gray, byte>((Bitmap)Image.FromFile("woman_gray.BMP"));
            var pattern = img.Clone();
            pattern.ROI = new Rectangle(200, 310, 70, 50);
            FindPatern(img, pattern);

            Console.ReadLine();
        }

        static void FindPatern(Image<Gray, byte> img, Image<Gray, byte> pattern)
        {
            var imgData = img.Data;
            var patternData = pattern.Data;
            var k = pattern.Width * pattern.Height;

            var R = Sum(patternData);
            var RAvg = 1 / k * R;

            var omegaR = 0;

            for (int i = 0; i < pattern.Rows; i++)
            {
                for (int j = 0; j < pattern.Cols; j++)
                {
                    omegaR += imgData[i, j, 0] - RAvg;
                }
            }

            var results = new BoundedPQ<PatternDiffrence>(20, Comparer<PatternDiffrence>.Create((p1, p2) => p1.CompareTo(p2)));

            Parallel.For(0, img.Rows - pattern.Rows, s =>
            {
                //for (int s = 0; s < img.Rows - pattern.Rows; s++)
                //{
                for (int r = 0; r < img.Cols - pattern.Cols; r++)
                {
                    var I = 0;
                    var I1 = 0;

                    for (int i = 0; i < pattern.Rows; i++)
                    {
                        for (int j = 0; j < pattern.Cols; j++)
                        {
                            I += imgData[i + s, j + r, 0];
                            I1 += imgData[i + s, j + r, 0] * patternData[i, j, 0];
                        }
                    }

                    var IAvg = I/k;
                    var cl = (I1 - k * IAvg * RAvg) / (Math.Sqrt(I * I - k * IAvg * IAvg) * Math.Sqrt(k) * omegaR);

                    results.Add(new PatternDiffrence(new Rectangle(r, s, pattern.Width, pattern.Height), cl));
                }
            });

            var resultImg = img.Convert<Rgb, byte>();

            foreach (var item in results)
            {
                Console.WriteLine(item.cl);
                resultImg.Draw(item.rect, new Rgb(255, 0, 0), 1);
                resultImg.Draw(item.cl.ToString("F2"), new Point(item.rect.Left, item.rect.Bottom + 30), Emgu.CV.CvEnum.FontFace.HersheyComplex, 1, new Rgb(255, 0, 0));
            }

            //var last = results.Last();
            //resultImg.Draw(last.rect, new Rgb(255, 0, 0), 1);
            //resultImg.Draw(last.cl.ToString("F2"), new Point(last.rect.Left, last.rect.Bottom + 30), Emgu.CV.CvEnum.FontFace.HersheyComplex, 1, new Rgb(255, 0, 0));

            CvInvoke.Imshow("result", resultImg);
            CvInvoke.WaitKey();
        }

        static int Sum(byte[,,] pattern)
        {
            var sum = 0;
            for (int i = 0; i < pattern.GetLength(1); i++)
            {
                for (int j = 0; j < pattern.GetLength(0); j++)
                {
                    sum += pattern[j, i, 0];
                }
            }

            return sum;
        }
    }
}
