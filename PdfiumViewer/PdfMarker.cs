using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

#pragma warning disable 1591

namespace PdfiumViewer
{
    public class PdfMarker : IPdfMarker
    {
        public int Page { get; set; }
        public RectangleF Bounds { get; set; }
        public Color Color { get; set; }
        public Color BorderColor { get; set; }
        public float BorderWidth { get; set; }

        public PdfMarker(int page, RectangleF bounds, Color color)
            : this(page, bounds, color, Color.Transparent, 0)
        {
        }

        public PdfMarker(int page, RectangleF bounds, Color color, Color borderColor, float borderWidth)
        {
            Page = page;
            Bounds = bounds;
            Color = color;
            BorderColor = borderColor;
            BorderWidth = borderWidth;
        }

        public void Draw(PdfRenderer renderer, Graphics graphics)
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer");
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            var bounds = renderer.BoundsFromPdf(new PdfRectangle(Page, Bounds));

            using (var brush = new SolidBrush(Color))
            {
                graphics.FillRectangle(brush, bounds);
            }

            if (BorderWidth > 0)
            {
                using (var pen = new Pen(BorderColor, BorderWidth))
                {
                    graphics.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width, bounds.Height);
                }
            }
        }
    }
}
