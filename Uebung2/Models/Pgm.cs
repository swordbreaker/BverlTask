using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.Util;

namespace Uebung2.Models
{
    public class Pgm
    {
        public Pgm(string path)
        {
            
        }

        public void Parse(string path)
        {
            using (var lines = File.ReadLines(path).GetEnumerator())
            {
                do
                {
                    if (!lines.MoveNext()) throw new Exception("No more lines");
                } while (lines.Current[0] == '#');

                ParseFormat(lines.Current.Trim().ToLowerInvariant());

                do
                {
                    if (!lines.MoveNext()) throw new Exception("No more lines");
                } while (lines.Current[0] == '#');

                ParseSize(lines.Current);

                do
                {
                    if (!lines.MoveNext()) throw new Exception("No more lines");
                } while (lines.Current[0] == '#');

                ParseMax(lines.Current);

            }


            
        }

        private void ParseFormat(string format)
        {
            
        }

        private void ParseSize(string line)
        {
            
        }

        private void ParseMax(string line)
        {
            
        }
    }
}
