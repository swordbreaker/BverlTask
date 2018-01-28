using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Uebung1
{
    public class BayerMask
    {
        const int R = 0;
        const int G = 1;
        const int B = 2;

        public static Image<Rgb, byte> Simple(Image<Gray, byte> raw)
        {
            var w = raw.Width;
            var h = raw.Height;

            var image = new Image<Rgb, byte>(w, h);

            byte[] rgb = new byte[3];

            // um die Aufgabe zu vereinfachen, f�hren wir keine Randbehandlung durch und lassen am Rand 2 Pixel schwarz
            for (int v = 2; v < h - 2; v++)
            {
                for (int u = 2; u < w - 2; u++)
                {
                    if (v % 2 == 0)
                    {
                        if (u % 2 == 0)
                        {
                            // b ist gegeben
                            rgb[B] = raw.Data[v, u, 0];

                            rgb[G] = Interpolate(
                                raw.Data[v - 1, u, 0],
                                raw.Data[v + 1, u, 0],
                                raw.Data[v, u - 1, 0],
                                raw.Data[v, u - 1, 0]
                            );

                            rgb[R] = Interpolate(
                                raw.Data[v - 1, u - 1, 0],
                                raw.Data[v + 1, u + 1, 0],
                                raw.Data[v + 1, u - 1, 0],
                                raw.Data[v - 1, u - 1, 0]
                            );
                        }
                        else
                        {
                            // g ist gegeben
                            rgb[G] = raw.Data[v, u, 0];

                            rgb[R] = Interpolate(
                                raw.Data[v - 1, u, 0],
                                raw.Data[v + 1, u, 0]
                            );

                            rgb[B] = Interpolate(
                                raw.Data[v, u + 1, 0],
                                raw.Data[v, u - 1, 0]
                            );
                        }
                    }
                    else
                    {
                        if (u % 2 == 0)
                        {
                            // g ist gegeben
                            rgb[G] = raw.Data[v, u, 0];

                            rgb[B] = Interpolate(
                                raw.Data[v - 1, u, 0],
                                raw.Data[v + 1, u, 0]
                            );

                            rgb[R] = Interpolate(
                                raw.Data[v, u + 1, 0],
                                raw.Data[v, u - 1, 0]
                            );
                        }
                        else
                        {
                            // r ist gegeben
                            rgb[R] = raw.Data[v, u, 0];

                            rgb[B] = Interpolate(
                                raw.Data[v + 1, u + 1, 0],
                                raw.Data[v + 1, u - 1, 0],
                                raw.Data[v - 1, u + 1, 0],
                                raw.Data[v - 1, u - 1, 0]
                            );

                            rgb[G] = Interpolate(
                                raw.Data[v - 1, u, 0],
                                raw.Data[v + 1, u, 0],
                                raw.Data[v    , u - 1, 0],
                                raw.Data[v    , u - 1, 0]
                            );
                        }
                    }
                    // rgb-Komponenten des neuen Bildes setzen
                    image.Data[v, u, 0] = rgb[0];
                    image.Data[v, u, 1] = rgb[1];
                    image.Data[v, u, 2] = rgb[2];
                }
            }

            return image;
        }

        public static Image<Rgb, byte> NotSoSimple(Image<Gray, byte> raw)
        {
            var w = raw.Width;
            var h = raw.Height;

            var image = new Image<Rgb, byte>(w, h);
            var rgb = new byte[3];

            // um die Aufgabe zu vereinfachen, f�hren wir keine Randbehandlung durch und lassen am Rand 2 Pixel schwarz
            for (int v = 2; v < h - 2; v++)
            {
                for (int u = 2; u < w - 2; u++)
                {
                    if (v % 2 == 0)
                    {
                        if (u % 2 == 0)
                        {
                            // b ist gegeben
                            rgb[B] = raw.Data[v, u, 0];

                            rgb[G] = Interpolate(
                                raw.Data[v - 1, u, 0],
                                raw.Data[v + 1, u, 0],
                                raw.Data[v, u - 1, 0],
                                raw.Data[v, u - 1, 0]
                            );

                            rgb[R] = Interpolate(
                                raw.Data[v - 1, u - 1, 0],
                                raw.Data[v + 1, u + 1, 0],
                                raw.Data[v + 1, u - 1, 0],
                                raw.Data[v - 1, u - 1, 0]
                            );
                        }
                        else
                        {
                            // g ist gegeben
                            rgb[G] = raw.Data[v, u, 0];

                            rgb[R] = Interpolate(
                                (raw.Data[v - 1, u, 0], 1),
                                (raw.Data[v + 1, u, 0], 1),
                                (raw.Data[v + 1, u - 2, 0], 0.2774f),
                                (raw.Data[v + 1, u + 2, 0], 0.2774f),
                                (raw.Data[v - 1, u - 2, 0], 0.2774f),
                                (raw.Data[v - 1, u + 2, 0], 0.2774f)
                            );

                            rgb[B] = Interpolate(
                                (raw.Data[v, u + 1, 0], 1),
                                (raw.Data[v, u - 1, 0], 1),
                                (raw.Data[v + 2, u - 1, 0], 0.2774f),
                                (raw.Data[v + 2, u + 1, 0], 0.2774f),
                                (raw.Data[v - 2, u - 1, 0], 0.2774f),
                                (raw.Data[v - 2, u + 1, 0], 0.2774f)
                            );
                        }
                    }
                    else
                    {
                        if (u % 2 == 0)
                        {
                            // g ist gegeben
                            rgb[G] = raw.Data[v, u, 0];

                            rgb[B] = Interpolate(
                                (raw.Data[v - 1, u, 0], 1),
                                (raw.Data[v + 1, u, 0], 1),
                                (raw.Data[v + 1, u - 2, 0], 0.2774f),
                                (raw.Data[v + 1, u + 2, 0], 0.2774f),
                                (raw.Data[v - 1, u - 2, 0], 0.2774f),
                                (raw.Data[v - 1, u + 2, 0], 0.2774f)
                            );

                            rgb[R] = Interpolate(
                                (raw.Data[v, u + 1, 0], 1),
                                (raw.Data[v, u - 1, 0], 1),
                                (raw.Data[v + 2, u - 1, 0], 0.2774f),
                                (raw.Data[v + 2, u + 1, 0], 0.2774f),
                                (raw.Data[v - 2, u - 1, 0], 0.2774f),
                                (raw.Data[v - 2, u + 1, 0], 0.2774f)
                            );
                        }
                        else
                        {
                            // r ist gegeben
                            rgb[R] = raw.Data[v, u, 0];

                            rgb[B] = Interpolate(
                                raw.Data[v + 1, u + 1, 0],
                                raw.Data[v + 1, u - 1, 0],
                                raw.Data[v - 1, u + 1, 0],
                                raw.Data[v - 1, u - 1, 0]
                            );

                            rgb[G] = Interpolate(
                                raw.Data[v - 1, u, 0],
                                raw.Data[v + 1, u, 0],
                                raw.Data[v, u - 1, 0],
                                raw.Data[v, u - 1, 0]
                            );
                        }
                    }
                    // rgb-Komponenten des neuen Bildes setzen
                    image.Data[v, u, 0] = rgb[0];
                    image.Data[v, u, 1] = rgb[1];
                    image.Data[v, u, 2] = rgb[2];
                }
            }

            return image;
        }

        public static Image<Rgb, byte> NotSoSimple2(Image<Gray, byte> raw)
        {
            var w = raw.Width;
            var h = raw.Height;

            var image = new Image<Rgb, byte>(w, h);
            var rgb = new byte[3];

            // um die Aufgabe zu vereinfachen, f�hren wir keine Randbehandlung durch und lassen am Rand 2 Pixel schwarz
            for (int v = 2; v < h - 2; v++)
            {
                for (int u = 2; u < w - 2; u++)
                {
                    if (v % 2 == 0)
                    {
                        if (u % 2 == 0)
                        {
                            // b ist gegeben
                            rgb[B] = raw.Data[v, u, 0];

                            rgb[G] = Interpolate(
                                raw.Data[v - 1, u, 0],
                                raw.Data[v + 1, u, 0],
                                raw.Data[v, u - 1, 0],
                                raw.Data[v, u - 1, 0]
                            );

                            rgb[R] = Interpolate(
                                raw.Data[v - 1, u - 1, 0],
                                raw.Data[v + 1, u + 1, 0],
                                raw.Data[v + 1, u - 1, 0],
                                raw.Data[v - 1, u - 1, 0]
                            );
                        }
                        else
                        {
                            // g ist gegeben
                            rgb[G] = raw.Data[v, u, 0];

                            rgb[R] = Interpolate(
                                raw.Data[v + 1, u - 2, 0],
                                raw.Data[v + 1, u + 2, 0],
                                raw.Data[v - 1, u - 2, 0],
                                raw.Data[v - 1, u + 2, 0]
                            );

                            rgb[B] = Interpolate(
                                raw.Data[v + 2, u - 1, 0],
                                raw.Data[v + 2, u + 1, 0],
                                raw.Data[v - 2, u - 1, 0],
                                raw.Data[v - 2, u + 1, 0]
                            );
                        }
                    }
                    else
                    {
                        if (u % 2 == 0)
                        {
                            // g ist gegeben
                            rgb[G] = raw.Data[v, u, 0];

                            rgb[B] = Interpolate(
                                raw.Data[v + 1, u - 2, 0],
                                raw.Data[v + 1, u + 2, 0],
                                raw.Data[v - 1, u - 2, 0],
                                raw.Data[v - 1, u + 2, 0]
                            );

                            rgb[R] = Interpolate(
                                (raw.Data[v + 2, u - 1, 0], 0.2774f),
                                (raw.Data[v + 2, u + 1, 0], 0.2774f),
                                (raw.Data[v - 2, u - 1, 0], 0.2774f),
                                (raw.Data[v - 2, u + 1, 0], 0.2774f)
                            );
                        }
                        else
                        {
                            // r ist gegeben
                            rgb[R] = raw.Data[v, u, 0];

                            rgb[B] = Interpolate(
                                raw.Data[v + 1, u + 1, 0],
                                raw.Data[v + 1, u - 1, 0],
                                raw.Data[v - 1, u + 1, 0],
                                raw.Data[v - 1, u - 1, 0]
                            );

                            rgb[G] = Interpolate(
                                raw.Data[v - 1, u, 0],
                                raw.Data[v + 1, u, 0],
                                raw.Data[v, u - 1, 0],
                                raw.Data[v, u - 1, 0]
                            );
                        }
                    }
                    // rgb-Komponenten des neuen Bildes setzen
                    image.Data[v, u, 0] = rgb[0];
                    image.Data[v, u, 1] = rgb[1];
                    image.Data[v, u, 2] = rgb[2];
                }
            }

            return image;
        }

        public static byte Interpolate(params (byte v, float m)[] tuples)
        {
            return (byte)(tuples.Sum(t => t.v * t.m) / tuples.Sum(t => t.m));
        }

        public static byte Interpolate(params byte[] vals)
        {
            return (byte)(vals.Sum(b1 => b1) / vals.Length);
        }
    }
}
