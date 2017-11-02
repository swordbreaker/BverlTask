using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using Uebung2.Commands;
using Uebung5.Annotations;
using System.Diagnostics;
using System.Collections.Generic;

namespace Uebung5
{
    /**
0 0 -1 0 0
0 -1 -2 -1 0
-1 -2 16 -2 -1
0 -1 -2 -1 0
0 0 -1 0 0

0 1 2 1 0
1 3 5 3 1
2 5 9 5 2
1 3 5 3 1
0 1 2 1 0
     */

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private BitmapImage _newImage;
        private BitmapImage _oldImage;
        private string _imgPath = @"saltandpepper.BMP";

        public string MatString { get; set; }

        public float Omega { get; set; } = 1;

        public Matrix<int> Matrix { get; set; }

        public ICommand UpdateMatrixCommand { get; }
        public ICommand MeadianFilterCommand { get; }
        public ICommand GaussFilterCommand { get; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand SobelCommand { get; set; }
        public ICommand EdgeColorCommand { get; set; }

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
            MeadianFilterCommand = new SimpleCommand(MedianFilter);
            GaussFilterCommand = new SimpleCommand(Gauss);
            OpenFileCommand = new SimpleCommand(OpenFile);
            SobelCommand = new SimpleCommand(Sobel);
            EdgeColorCommand = new SimpleCommand(EdgeColor);

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

        private void Sobel()
        {
            //var m1 = new ConvolutionMatrix(new float[,]
            //{
            //    {-3 , 0, -3 },
            //    {-10, 0, -10 },
            //    {-3 , 0, -3 }

            //}, new Point(1,1));

            //var m2 = new ConvolutionMatrix(new float[,]
            //{
            //    {-3 , -10, -3 },
            //    {0, 0, 0 },
            //    {3 , 10, 3 }

            //}, new Point(1, 1));

            //var imgData = m1 * img.Data;
            //imgData = m2 * imgData;

            //ConvolutionMatrix.Clamp(imgData);

            var img = new Image<Rgb, float>((Bitmap)Image.FromFile(_imgPath));
            var imgData = Sobel(img.Data);

            var resultImg = new Image<Rgb, float>(imgData);

            NewImage = ToBitmapImage(resultImg.Bitmap);
            OldImage = ToBitmapImage(img.Bitmap);
        }

        private void EdgeColor()
        {
            var img = new Image<Rgb, float>((Bitmap)Image.FromFile(_imgPath));
            var imgData = Sobel(img.Data);

            var resultImg = new Image<Hsv,byte>(img.Size);
            var data = resultImg.Data;

            for (int i = 0; i < imgData.GetLength(0); i++)
            {
                for (int j = 0; j < imgData.GetLength(1); j++)
                {
                    data[i, j, 0] = (byte)imgData[i, j, 0];
                    data[i, j, 1] = 255;
                    data[i, j, 2] = 255;
                }
            }

            NewImage = ToBitmapImage(resultImg.Bitmap);
            OldImage = ToBitmapImage(img.Bitmap);
        }

        private float[,,] Sobel(float[,,] img)
        {
            var m1 = new ConvolutionMatrix(new float[,]
            {
                {-3 , 0, -3 },
                {-10, 0, -10 },
                {-3 , 0, -3 }

            }, new Point(1, 1));

            var m2 = new ConvolutionMatrix(new float[,]
            {
                {-3 , -10, -3 },
                {0, 0, 0 },
                {3 , 10, 3 }

            }, new Point(1, 1));

            var imgData = m1 * img;
            imgData = m2 * imgData;

            ConvolutionMatrix.Clamp(imgData);
            return imgData;
        }

        private void UpdateMatrix()
        {
            var stopwatch = Stopwatch.StartNew();

            Matrix = MatString.ToMatrix();
            (var hi, var hj) = (Matrix.Rows / 2, Matrix.Cols / 2);
            var m = new ConvolutionMatrix(Matrix.Convert<float>(), new Point(hj, hi));

            var img = new Image<Rgb, float>((Bitmap)Image.FromFile(_imgPath));

            var result = m * img.Data;

            var resultImg = new Image<Rgb, float>(result);

            Debug.WriteLine($"Linear Filter: {stopwatch.Elapsed.TotalMilliseconds}ms");

            NewImage = ToBitmapImage(resultImg.Bitmap);
            OldImage = ToBitmapImage(img.Bitmap);
        }

        private void MedianFilter()
        {
            var stopwatch = Stopwatch.StartNew();

            Matrix = MatString.ToMatrix();
            (var hi, var hj) = (Matrix.Rows / 2, Matrix.Cols / 2);
            var m = new ConvolutionMatrix(Matrix.Convert<float>(), new Point(hj, hi));

            var img = new Image<Rgb, float>((Bitmap)Image.FromFile(_imgPath));

            (var w, var h, var depht) = (img.Width, img.Height, 3);

            var result = new float[h, w, depht];
            var b = img.Data;

            for (int v = 0; v < h; v++)
            {
                for (int u = 0; u < w; u++)
                {
                    var values = new List<float>[depht];

                    for (int k = 0; k < depht; k++) { values[k] = new List<float>(); }

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
                                var ig = b[y, x, k];
                                var ma = m[i, j];

                                for (int l = 0; l < ma; l++)
                                {
                                    values[k].Add(ig);
                                }

                            }
                        }
                    }

                    for (int k = 0; k < depht; k++)
                    {
                        values[k].Sort();
                        result[v, u, k] = values[k][values[k].Count / 2];
                    }
                }
            }

            var resultImg = new Image<Rgb, float>(result);

            Debug.WriteLine($"Median Filter: {stopwatch.Elapsed.TotalMilliseconds}ms");

            NewImage = ToBitmapImage(resultImg.Bitmap);
            OldImage = ToBitmapImage(img.Bitmap);
        }


        public void Gauss()
        {
            var stopwatch = Stopwatch.StartNew();

            var width = 5;
            var mid = width / 2;

            var data = new float[1, width];

            for (int i = -mid; i < mid+1; i++)
            {
                data[0, i+ mid] = (float)Math.Exp(-((i * i) / (2 * Omega * Omega)) );
            }

            (var hi, var hj) = (0, width/2);
            var m1 = new ConvolutionMatrix(data, new Point(hj, hi));
            var m2 = new ConvolutionMatrix(m1.Transpose(), new Point(hi, hj));

            var img = new Image<Rgb, float>((Bitmap)Image.FromFile(_imgPath));

            var result = m1 * img.Data;
            result = m2 * result;
            
            var resultImg = new Image<Rgb, float>(result);

            Debug.WriteLine($"Linear Filter: {stopwatch.Elapsed.TotalMilliseconds}ms");

            NewImage = ToBitmapImage(resultImg.Bitmap);
            OldImage = ToBitmapImage(img.Bitmap);

        }

        private void OpenFile()
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "Image Files(*.BMP;*.JPG;*.GIF;*PNG)|*.BMP;*.JPG;*.GIF;*PNG"
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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
