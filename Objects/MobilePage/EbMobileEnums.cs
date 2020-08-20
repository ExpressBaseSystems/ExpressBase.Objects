using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    public enum NetworkMode
    {
        Online,
        Offline,
        Mixed
    }

    public enum NumericBoxTypes
    {
        TextType = 0,
        ButtonType = 1
    }

    public enum SortOrder
    {
        Ascending = 0,
        Descending = 1
    }

    public enum RenderStyle
    {
        Flat = 1,
        Tile = 2
    }

    public enum DataColumnRenderType
    {
        Text = 1,
        Image = 2,
        MobileNumber = 3,
        Map = 4,
        Email = 5
    }

    public enum MobileHorrizontalAlign
    {
        Left,
        Center,
        Right,
        Fill
    }

    public enum MobileVerticalAlign
    {
        Left,
        Center,
        Right,
        Fill
    }
}
