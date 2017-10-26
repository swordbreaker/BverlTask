using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Uebung2.Commands;
using Uebung5.Annotations;

namespace Uebung5
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private BitmapImage _newImage;
        private BitmapImage _oldImage;
        private const string _imgPath = @"Lenna.png";

        public string MatString { get; set; }

        public Matrix<int> Matrix { get; set; }

        public ICommand UpdateMatrixCommand { get; set; }

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
            UpdateMatrixCommand = new SimpleCommand(UpdateMatrix);

            var mat = new Matrix<int>(3, 3)
            {
                Data = new[,]
                {
                    {1, 1, 1},
                    {1, 1, 1},
                    {1, 1, 1},
                }
            };
            mat = mat.Transpose();

            MatString = mat.CovertToString();
        }

        private void UpdateMatrix()
        {
            Matrix = MatString.ToMatrix();

            var newImg = new Image<Rgb,byte>((Bitmap)System.Drawing.Image.FromFile(_imgPath));
            (var hi, var hj) = (Matrix.Rows / 2, Matrix.Cols / 2);
            var img = new Image<Rgb, byte>(new System.Drawing.Size(newImg.Width + 2*hj, newImg.Height + 2*hi));
            
            CvInvoke.CopyMakeBorder(newImg, img, hj, hj, hi, hi, BorderType.Reflect);

            var nMatrix = (Math.Abs(Matrix.Sum) > 0.0000001f)? Matrix.Convert<float>() / Matrix.Sum : Matrix.Convert<float>();

            var kernel = new ConvolutionKernelF(nMatrix, new System.Drawing.Point(hi, hj));
            var newImg2 = img.Convolution(kernel);

            var dataOld = img.Data;
            var dataNew = newImg.Data;
            for (int v = 0; v < newImg.Rows; v++)
            {
                for (int u = 0; u < newImg.Cols; u++)
                {
                    var sumR = 0f;
                    var sumG = 0f;
                    var sumB = 0f;
                    for (int i = -hi; i < hi; i++)
                    {
                        for (int j = -hj; j < hj; j++)
                        {
                            sumR += dataOld[v+j+hj, u+i+hi, 0] * nMatrix[i + hi, j + hj];
                            sumG += dataOld[v+j+hj, u+i+hi, 1] * nMatrix[i + hi, j + hj];
                            sumB += dataOld[v+j+hj, u+i+hi, 2] * nMatrix[i + hi, j + hj];
                        }
                    }
                    dataNew[v, u, 0] = (byte)sumR.Clamp(0, 255);
                    dataNew[v, u, 1] = (byte)sumG.Clamp(0, 255);
                    dataNew[v, u, 2] = (byte)sumB.Clamp(0, 255);
                }
            }

            NewImage = ToBitmapImage(newImg2.Bitmap);
            OldImage = ToBitmapImage(img.Bitmap);
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
    }
}
