using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

#pragma warning disable 1591

namespace PdfiumViewer
{
    public class PdfRectangle : IEquatable<PdfRectangle>
    {
        public static readonly PdfRectangle Empty = new PdfRectangle();

        // _page is offset by 1 so that Empty returns an invalid rectangle.
        private readonly int _page;

        public int Page
        {
            get { return _page - 1; }
        }

        public RectangleF Bounds { get; set; }

        public bool IsValid
        {
            get { return _page != 0; }
        }

        public PdfRectangle() { }

        public PdfRectangle(int page, RectangleF bounds)
        {
            _page = page + 1;
            this.Bounds = bounds;
        }

        public bool Equals(PdfRectangle other)
        {
            return
                Page == other.Page &&
                this.Bounds == other.Bounds;
        }

        public override bool Equals(object obj)
        {
            return
                obj is PdfRectangle &&
                Equals((PdfRectangle)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Page * 397) ^ Bounds.GetHashCode();
            }
        }

        public static bool operator ==(PdfRectangle left, PdfRectangle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PdfRectangle left, PdfRectangle right)
        {
            return !left.Equals(right);
        }
    }
}
