using System;
using System.CodeDom;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Uebung2.Commands;
using Uebung2.Models;
using Image = System.Drawing.Image;

namespace Uebung2.ViewModels
{
    public class MainWindowViewModel
    {
        public ICommand DecodeCommand { get; set; }
        public ICommand EncodeCommand { get; set; }

        private const string HuffFile = "Test.huf";

        public MainWindowViewModel()
        {
            EncodeCommand = new SimpleCommand(Encode);
            DecodeCommand = new SimpleCommand(Decode);
        }

        public void Encode()
        {
            var bmp = (Bitmap)Image.FromFile("bridge.gif");
            var img = new Image<Gray, byte>(bmp);

            var hist = HistogramHelper.Calc(img);

            (var root, var codes) = Huffman.Calc(hist.Select(f => (int)f).ToArray(), img.Height * img.Width);

            var data = Huffman.EncodeImage(img, codes);

            FileHelper.Write(HuffFile, img, root, data);
            //Write(HuffFile, img, root, data);
        }

        public void Decode()
        {
            var img = FileHelper.Read(HuffFile);

            ImageViewer.Show(img);
        }

        public void Write(string path, Image<Gray, byte> img, Node root, BitArray bitarray)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    var formatter = new BinaryFormatter();

                    //Write header
                    formatter.Serialize(stream, img.Width);
                    formatter.Serialize(stream, img.Height);

                    //Write code tree
                    formatter.Serialize(stream, root);

                    //Write compressed data
                    formatter.Serialize(stream, bitarray);

                    stream.Flush();
                    stream.Position = 0;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
