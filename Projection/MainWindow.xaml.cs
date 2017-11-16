using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Emgu.CV.UI;
using Image = System.Drawing.Image;
using Point = System.Drawing.Point;

namespace Projection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BitmapImage Img { get; set; }
        public BitmapImage HImg { get; set; }
        public BitmapImage VImg { get; set; }


        public MainWindow()
        {
            var img = new Image<Gray, byte>((Bitmap) Image.FromFile("example.png"));

            var data = img.Data;

            var m = img.Height;
            var n = img.Width;

            var hProj = new int[n];
            var vProj = new int[m];

            for (int v = 0; v < n; v++)
            {
                for (int u = 0; u < m; u++)
                {
                    hProj[v] += data[u, v, 0];
                    vProj[u] += data[u, v, 0];
                }
            }

            var hMax = hProj.Max() / 100;
            var vMax = vProj.Max() / 100;
            var hImg = new Image<Gray, byte>(hProj.Length, hMax);
            var vImg = new Image<Gray, byte>(vMax, vProj.Length);

            hImg.SetValue(new Gray(0));
            vImg.SetValue(new Gray(0));

            for (int i = 0; i < vProj.Length; i++)
            {
                CvInvoke.Line(vImg, new Point(0, i), new Point(vProj[i] / 100, i), new Gray(255).MCvScalar);
            }

            for (int i = 0; i < hProj.Length; i++)
            {
                CvInvoke.Line(hImg, new Point(i, 0), new Point(i, hProj[i] / 100), new Gray(255).MCvScalar);
            }

            Img = ToBitmapImage(img.Bitmap);

            //ImageViewer.Show(hImg);
            HImg = ToBitmapImage(hImg.Bitmap);
            VImg = ToBitmapImage(vImg.Bitmap);

            InitializeComponent();
        }

        public BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}
