using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    public class FontStyle
    {
        public FontStyle() { }

        public string Font { get; set; }
        public string Color { get; set; }
        public string Case { get; set; }
        public string Style { get; set; }
        public int Size { get; set; }
        public bool IsStrikeThrough { get; set; }
        public bool IsUnderlined { get; set; }
    }


    //public enum FontStyles
    //{
    //    Regular,
    //    Italic,
    //    Bold,
    //    ItalicBold
    //}
}
