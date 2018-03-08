using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    public class EbFont
    {
        public EbFont() { }

        public string Font { get; set; } = "Times-Roman";
        public int Size { get; set; } = 14;
        public FontStyle Style { get; set; }
        public string color { get; set; }
        public bool Caps { get; set; }
        public bool Strikethrough { get; set; }
        public bool Underline { get; set; }
    }
    //JS OBJ:  {"Font":"Abhaya Libre","Fontsize":"16","Fontstyle":"normal","FontWeight":"bold","Fontcolor":"#000000","Caps":"none","Strikethrough":"line-through","Underline":"none"}

    public enum FontStyle
    {
        Regular,//NORMAL = 0;
        Italic,//ITALIC = 2
        Bold,//BOLD = 1;
        ItalicBold//BOLDITALIC = 3;
    }
}
