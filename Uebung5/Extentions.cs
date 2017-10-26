using System;
using System.Linq;
using System.Text;
using Emgu.CV;

namespace Uebung5
{
    public static class Extentions
    {
        public static Matrix<int> ToMatrix(this string s)
        {
            var lines = s.Split('\n');

            var cols = lines.Where(s1 => !string.IsNullOrEmpty(s1.Trim())).ToArray().Length;
            var rows = lines[0].Split(' ').Count(s1 => !string.IsNullOrEmpty(s1.Trim()));

            var m = new Matrix<int>(rows, cols);

            for (var i = 0; i < lines.Length; i++)
            {
                var values = lines[i].Split(' ').Where(s1 => !string.IsNullOrEmpty(s1.Trim())).ToArray();
                for (var j = 0; j < values.Length; j++)
                {
                    m.Data[i, j] = int.Parse(values[j]);
                }
            }

            return m;
        }

        public static string CovertToString(this Matrix<int> m)
        {
            var sb = new StringBuilder();

            for (int j = 0; j < m.Cols; j++)
            {
                for (int i = 0; i < m.Rows; i++)
                {
                    sb.Append(m.Data[i, j]);
                    sb.Append(" ");
                }
                sb.Append("\n");
            }

            return sb.ToString();
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }
}
