using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable 1591

namespace PdfiumViewer
{
    public class PdfTextSpan : IEquatable<PdfTextSpan>
    {
        public int Page { get; set; }
        public int Offset { get; set; }
        public int Length { get; set; }

        public PdfTextSpan(int page, int offset, int length)
        {
            Page = page;
            Offset = offset;
            Length = length;
        }

        public bool Equals(PdfTextSpan other)
        {
            return
                Page == other.Page &&
                Offset == other.Offset &&
                Length == other.Length;
        }

        public override bool Equals(object obj)
        {
            return
                obj is PdfTextSpan &&
                Equals((PdfTextSpan)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Page;
                hashCode = (hashCode * 397) ^ Offset;
                hashCode = (hashCode * 397) ^ Length;
                return hashCode;
            }
        }

        public static bool operator ==(PdfTextSpan left, PdfTextSpan right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PdfTextSpan left, PdfTextSpan right)
        {
            return !left.Equals(right);
        }
    }
}
