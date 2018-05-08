using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Packaging
{
    public class FileFormatException : Exception
    {
        public FileFormatException(string message): base(message)
        {
            
        }
    }
}
