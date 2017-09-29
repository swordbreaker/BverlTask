using System.Drawing;
using System.Windows.Controls;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace Uebung2.ViewModels
{
    public class MainWindowViewModel
    {
        public HistogramViewer HistViewer { get; private set; }


        public MainWindowViewModel()
        {
            Run();
        }

        public void Run()
        {
            var bmp = (Bitmap)Bitmap.FromFile("bridge.gif");
            var img = new Image<Gray, byte>(bmp);
            //var img = CvInvoke.Imread("bridge.gif").ToImage<Gray, byte>();

            
            var hist = new DenseHistogram(256, new RangeF(0, 255));
            hist.Calculate(new[]{img}, true, null);
            var vals = hist.GetBinValues();

            HistViewer = new HistogramViewer();
            HistViewer.HistogramCtrl.GenerateHistograms(img, 256);
            HistViewer.HistogramCtrl.Refresh();
            HistViewer.Show();


            //HistogramViewer.Show(img);
        }

    }
}
