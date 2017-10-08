using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Uebung3.Annotations;

namespace Uebung3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private BitmapImage _old;
        private BitmapImage _new;

        public BitmapImage Old
        {
            get => _old;
            set
            {
                _old = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage New
        {
            get => _new;
            set
            {
                _new = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            //var mat = CvInvoke.Imread("Lena128.bmp");
            //var img = mat.ToImage<Gray, byte>();

            ////var newImg = HistAusgleich.Calc(img);
            //var newImg = Gamma.Calc(img, 1/2.4, 0.00304);

            //Old = ToBitmapImage(img.ToBitmap());
            //New = ToBitmapImage(newImg.ToBitmap());
        }

        public async Task ModifyImage(Func<Image<Gray, byte>, Image<Gray, byte>> func)
        {
            var mat = CvInvoke.Imread("Lena128.bmp");
            var img = mat.ToImage<Gray, byte>();

            var newImg = func.Invoke(img);
            //var newImg = HistAusgleich.Calc(img);
            //var newImg = Gamma.Calc(img, 1 / 2.4, 0.00304);

            Old = ToBitmapImage(img.ToBitmap());
            New = ToBitmapImage(newImg.ToBitmap());
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void HistButtonClick(object sender, RoutedEventArgs e)
        {
            await ModifyImage(HistAusgleich.Calc);
        }

        private async void GammaButtonClick(object sender, RoutedEventArgs e)
        {
            await ModifyImage(image => Gamma.Calc(image, 1 / 2.4, 0.00304));
        }

        private void GammaInvButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
