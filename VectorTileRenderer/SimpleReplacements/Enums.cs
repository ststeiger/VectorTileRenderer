
namespace VectorTileRenderer
{
    public enum PenLineCap
    {
        Flat = 0,
        Square = 1,
        Round = 2,
        Triangle = 3,
    }



    public enum TextAlignment
    {

        Left = 0, // Default 
        Right = 1,
        Center = 2,
        Justify = 3
    }




    public enum BitmapCacheOption
    {
        Default = 0,
        OnDemand = Default,
        OnLoad = 1,
        None = 2
    }



    /// <summary>
    /// PixelFormatEnum represents the format of the bits of an image or surface.
    /// </summary>
    public enum PixelFormatEnum
    {
        /// <summary>
        /// Default: (DontCare) the format is not important
        /// </summary>
        Default = 0,

        /// <summary>
        /// Extended: the pixel format is 3rd party - we don't know anything about it.
        /// </summary>
        Extended = Default,

        /// <summary>
        /// Indexed1: Paletted image with 2 colors.
        /// </summary>
        Indexed1 = 0x1,

        /// <summary>
        /// Indexed2: Paletted image with 4 colors.
        /// </summary>
        Indexed2 = 0x2,

        /// <summary>
        /// Indexed4: Paletted image with 16 colors.
        /// </summary>
        Indexed4 = 0x3,

        /// <summary>
        /// Indexed8: Paletted image with 256 colors.
        /// </summary>
        Indexed8 = 0x4,

        /// <summary>
        /// BlackWhite: Monochrome, 2-color image, black and white only.
        /// </summary>
        BlackWhite = 0x5,

        /// <summary>
        /// Gray2: Image with 4 shades of gray
        /// </summary>
        Gray2 = 0x6,

        /// <summary>
        /// Gray4: Image with 16 shades of gray
        /// </summary>
        Gray4 = 0x7,

        /// <summary>
        /// Gray8: Image with 256 shades of gray
        /// </summary>
        Gray8 = 0x8,

        /// <summary>
        /// Bgr555: 16 bpp SRGB format
        /// </summary>
        Bgr555 = 0x9,

        /// <summary>
        /// Bgr565: 16 bpp SRGB format
        /// </summary>
        Bgr565 = 0xA,

        /// <summary>
        /// Gray16: 16 bpp Gray format
        /// </summary>
        Gray16 = 0xB,

        /// <summary>
        /// Bgr24: 24 bpp SRGB format
        /// </summary>
        Bgr24 = 0xC,

        /// <summary>
        /// BGR24: 24 bpp SRGB format
        /// </summary>
        Rgb24 = 0xD,

        /// <summary>
        /// Bgr32: 32 bpp SRGB format
        /// </summary>
        Bgr32 = 0xE,

        /// <summary>
        /// Bgra32: 32 bpp SRGB format
        /// </summary>
        Bgra32 = 0xF,

        /// <summary>
        /// Pbgra32: 32 bpp SRGB format
        /// </summary>
        Pbgra32 = 0x10,

        /// <summary>
        /// Gray32Float: 32 bpp Gray format, gamma is 1.0
        /// </summary>
        Gray32Float = 0x11,

        /// <summary>
        /// Bgr101010: 32 bpp Gray fixed point format
        /// </summary>
        Bgr101010 = 0x14,

        /// <summary>
        /// Rgb48: 48 bpp RGB format
        /// </summary>
        Rgb48 = 0x15,

        /// <summary>
        /// Rgba64: 64 bpp extended format; Gamma is 1.0
        /// </summary>
        Rgba64 = 0x16,

        /// <summary>
        /// Prgba64: 64 bpp extended format; Gamma is 1.0
        /// </summary>
        Prgba64 = 0x17,

        /// <summary>
        /// Rgba128Float: 128 bpp extended format; Gamma is 1.0
        /// </summary>
        Rgba128Float = 0x19,

        /// <summary>
        /// Prgba128Float: 128 bpp extended format; Gamma is 1.0
        /// </summary>
        Prgba128Float = 0x1A,

        /// <summary>
        /// PABGR128Float: 128 bpp extended format; Gamma is 1.0
        /// </summary>
        Rgb128Float = 0x1B,

        /// <summary>
        /// CMYK32: 32 bpp CMYK format.
        /// </summary>
        Cmyk32 = 0x1C
    }


}
