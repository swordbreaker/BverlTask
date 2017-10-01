using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uebung2.Models
{
    [Serializable]
    public class Node : IComparable<Node>
    {
        protected Node Left;
        protected Node Right;

        [NonSerialized]
        private long _code;
        [NonSerialized]
        private byte _codeLenght;
        [NonSerialized]
        private readonly double _p;

        public long Code
        {
            get => _code;
            protected set => _code = value;
        }

        public double P => _p;

        public byte CodeLenght
        {
            get => _codeLenght;
            protected set => _codeLenght = value;
        }

        public bool IsInnernode => Left != null;

        public Node(double p)
        {
            _p = p;
        }

        public Node(Node left, Node right)
        {
            Left = left;
            Right = right;
            _p = Left.P + Right.P;
        }

        public void SetCode(long code, int codeLen)
        {
            Code = code;
            CodeLenght = (byte)codeLen;

            Left?.SetCode(code << 1, codeLen + 1);
            Right?.SetCode((code << 1) + 1, codeLen + 1);
        }

        public int CompareTo(Node other)
        {
            return P.CompareTo(other.P);
        }

        public Node DecodeBit(bool bit)
        {
            return (bit) ? Right : Left;
        }

        public override string ToString()
        {
            return $"Node code: {Code} p: {P}";
        }
    }

    [Serializable]
    public class Leaf : Node
    {
        public byte Intensity { get; } // pixel intensity used during decoding

        public Leaf(double p, byte intensity) : base(p)
        {
            Intensity = intensity;
        }
    }
}
