
namespace VectorTileRenderer111
{


    using VectorTileRenderer;


    public class FormatConvertedBitmap
        : BitmapSource
    {
        public FormatConvertedBitmap(BitmapSource bitmap, PixelFormat destinationFormat, BitmapPalette destinationPalette, double alphaThreshold)
        { }

        public void CopyPixels(Int32Rect rect, System.IntPtr pixels, int bytesSize, int rowBytes)
        {

        }

    }


    public class BitmapPalette
    { }

    public class WriteableBitmap
        : BitmapSource
    {
        public int BackBufferStride;
        public System.IntPtr BackBuffer;

        public void Lock()
        { }

        public void Unlock()
        { }

        public void Freeze()
        { }


        public void AddDirtyRect(Int32Rect rect)
        {
        }


        public WriteableBitmap(
           int pixelWidth,
           int pixelHeight,
           double dpiX,
           double dpiY,
           PixelFormat pixelFormat,
           BitmapPalette palette
           )
        //: base(true) // Use base class virtuals
        {
        }
    }



    public class BitmapImage
        : BitmapSource
    {

        public int DecodePixelWidth;
        public int DecodePixelHeight;

        public void BeginInit()
        { }

        public System.IO.Stream StreamSource;

        public BitmapCacheOption CacheOption;
        public void EndInit() { }
        public void Freeze() { }
    }

    public class BitmapFrame
    {

        public static BitmapFrame Create(System.Windows.Media.Imaging.BitmapSource bms)
        {
            return null;
        }
    }


    public abstract class BitmapEncoder
    {

        public System.Collections.Generic.List<BitmapFrame> Frames;

        public virtual void Save(System.IO.Stream stream)
        { }
    }

    public class PngBitmapEncoder
        : BitmapEncoder
    { }


    public struct PixelFormat
    {
        public PixelFormat(PixelFormatEnum nu)
        {
        }


    }



    public static class PixelFormats
    {
        /// <summary>
        /// Pbgra32: 32 bpp SRGB format
        /// </summary>
        public static PixelFormat Pbgra32
        {
            get
            {
                return new PixelFormat(PixelFormatEnum.Pbgra32);
            }
        }
    }




    public class BitmapSource
    {
        public System.Windows.Media.Imaging.BitmapSource WindowsShit;

        public int PixelWidth;
        public int PixelHeight;


        //public static BitmapSource2 MoreWindowsShit(BitmapImage foo)
        //{
        //    return null;
        //}



    }


}
