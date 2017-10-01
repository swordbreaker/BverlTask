using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Uebung2.Models
{
    public class FileHelper
    {

        [Serializable]
        struct HuffanFile
        {
            public readonly int Width;
            public readonly int Height;
            public readonly Node Tree;
            public readonly BitArray Data;

            public HuffanFile(int width, int height, Node tree, BitArray data)
            {
                Width = width;
                Height = height;
                Tree = tree;
                Data = data;
            }
        }

        public static void Write(string path, Image<Gray, byte> img, Node root, BitArray bitarray)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    stream.Position = 0;
                    var formatter = new BinaryFormatter();

                    var huffmanFile = new HuffanFile(img.Width, img.Height, root, bitarray);

                    formatter.Serialize(stream, huffmanFile);

                    stream.Flush();
                    stream.Position = 0;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Image<Gray, byte> Read(string path)
        {
            Image<Gray, byte> img;

            try
            {
                HuffanFile huffmanFile;
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    var formatter = new BinaryFormatter();

                    huffmanFile = (HuffanFile)formatter.Deserialize(stream);
                }

                // create output image
                img = new Image<Gray, byte>(huffmanFile.Width, huffmanFile.Height, new Gray(0));

                // fill in data
                int index = 0;

                var data = img.Data;

                for (int j = img.Cols - 1; j >= 0; j--)
                {
                    for (int i = img.Rows - 1; i >= 0; i--)
                    {
                        var node = huffmanFile.Tree;
                        while (node.IsInnernode)
                        {
                            node = node.DecodeBit(huffmanFile.Data[index++]);
                        }
                        data[i,j,0] = ((Leaf)node).Intensity;
                    }
                }

                //for (int i = 0; i < pixels.Length; i++)
                //{
                //    node = huffmanFile.Tree;
                //    while (node.IsInnernode)
                //    {
                //        node = node.DecodeBit(huffmanFile.Data.Get(index++));
                //    }
                //    pixels[i] = ((Leaf)node).Intensity;
                //}

                //img.Bytes = pixels;
            }
            catch (Exception e)
            {
                throw e;
            }

            return img;
        }

    }
}
