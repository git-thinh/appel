using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace app_sys
{
    public enum oNodeType
    {
        NONE,
        TEXT,
        PDF,
        DOC,
        DOCX,
        PPT,
        PPTX,
        XLS,
        XLSX,
        FOLDER,
    }

    public class oNode
    {
        public string name { set; get; }
        public string text { set; get; }

        string _path = string.Empty;
        public string path
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _path = value.ToLower().Trim();
                    if (_path.EndsWith(".txt"))
                        type = oNodeType.TEXT;
                    else if (_path.EndsWith(".pdf"))
                        type = oNodeType.PDF;
                    else if (_path.EndsWith(".docx"))
                        type = oNodeType.DOCX;
                    else if (_path.EndsWith(".doc"))
                        type = oNodeType.DOC;
                    else if (_path.EndsWith(".XLSX"))
                        type = oNodeType.XLSX;
                    else if (_path.EndsWith(".XLS"))
                        type = oNodeType.XLS;
                    else if (_path.EndsWith(".pptx"))
                        type = oNodeType.PPTX;
                    else if (_path.EndsWith(".ppt"))
                        type = oNodeType.PPT;
                    //else if (Directory.Exists(_path))
                    //    type = oNodeType.FOLDER;
                }
            }
            get
            {
                return _path;
            }
        }

        public bool anylatic { set; get; }
        public bool root { set; get; }

        public oNodeType type { get; set; }
    } 
}
