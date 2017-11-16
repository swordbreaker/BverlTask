using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using Uebung8.Commands;

namespace Uebung8
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private BitmapImage _newImage;
        private BitmapImage _oldImage;
        private string _imgPath = @"coins.png";


        public ICommand OpenFileCommand { get; set; }
        public ICommand FloodFillCommand { get; set; }

        public BitmapImage NewImage
        {
            get => _newImage;
            set
            {
                if (Equals(value, _newImage)) return;
                _newImage = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage OldImage
        {
            get => _oldImage;
            set
            {
                if (Equals(value, _oldImage)) return;
                _oldImage = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            FloodFillCommand = new SimpleCommand(FloodFill);
        }

        public void FloodFill()
        {
            var img = new Image<Gray, byte>((Bitmap)Image.FromFile(_imgPath));
            var binbaryImage = img.ThresholdBinary(new Gray(90), new Gray(1));
            CvInvoke.MedianBlur(binbaryImage, binbaryImage, 3);
            
            var labelImage = FloodFillAlg.Run(binbaryImage);

            NewImage = ToBitmapImage(labelImage.Bitmap);
            OldImage = ToBitmapImage(img.Bitmap);
        }

        private void OpenFile()
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "Image Files(*.BMP;*.JPG;*.GIF;*PNG;*TIF)|*.BMP;*.JPG;*.GIF;*PNG;*TIF"
            };
            var result = fileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    _imgPath = fileDialog.FileName;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    return;
            }

            OldImage = ToBitmapImage((Bitmap)Image.FromFile(_imgPath));
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
