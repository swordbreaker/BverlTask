using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Uebung2.Models
{
    public class Huffman
    {
        public static (Node root, Leaf[] codes) Calc(int[] hist, int size)
        {
            var pq = new PriorityQueue<Node>(Comparer<Node>.Default);
            var codes = new List<Leaf>();

            var entropie = 0d;
            var codeLenghtMiddle = 0d;

            for (var i = 0; i < hist.Length; i++)
            {
                var p = (double)hist[i] / size;
                entropie += (p != 0) ? p *  Math.Log(p, 2) : 0;
                codeLenghtMiddle += p * hist[i];

                var l = new Leaf(p, (byte) i);
                codes.Add(l);
                pq.Enqueue(l);
            }

            entropie = -entropie;

            // Mittlere Codel�nge und Speicherbedarf absch�tzen (Unter- und Obergrenze)
            // Verwenden Sie f�r die Ausgabe z.B. einen MessageDialog
            Debug.WriteLine($"Code länge zwischen {entropie} und {entropie+1}");

            // Mittlere Codel�nge und Speicherbedarf berechnen und ausgeben
            Debug.WriteLine($"mittlere code lenge {codeLenghtMiddle} ");

            while (pq.Count > 1)
            {
                pq.Enqueue(new Node(pq.Deque(), pq.Deque()));
            }

            var root = pq.Deque();

            Debug.Assert(pq.IsEmpty, "pq.IsEmpty");

            // Wurzelknoten holen und rekursiv alle Codes erzeugen
            root.SetCode(0,0);

            return (root, codes.ToArray());
        }

        public static BitArray EncodeImage(Image<Gray, byte> img, Leaf[] codes)
        {
            var data = img.Data;
            var bits = new List<bool>();

            for (int j = img.Cols - 1; j >= 0; j--)
            {
                for (int i = img.Rows - 1; i >= 0; i--)
                {
                    var code = codes[data[i, j, 0]];

                    for (int l = code.CodeLenght - 1; l >= 0; l--)
                    {
                        bits.Add((code.Code >> l & 1) == 1);
                    }
                }
            }

            return new BitArray(bits.ToArray());
        }
    }
}
