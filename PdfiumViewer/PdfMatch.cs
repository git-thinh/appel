using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable 1591

namespace PdfiumViewer
{
    public class PdfMatch
    {
        public string Text { get; set; }
        public PdfTextSpan TextSpan { get; set; }
        public int Page { get; set; }

        public PdfMatch(string text, PdfTextSpan textSpan, int page)
        {
            Text = text;
            TextSpan = textSpan;
            Page = page;
        }
    }
}
