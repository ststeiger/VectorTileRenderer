
namespace VectorTileRenderer
{


    public class FontInfo
    {
        public Typography.OpenFont.Typeface OpenFont;
        public SkiaSharp.SKTypeface Typeface;

        public string Path;
        public string FileName;
        public string FileNameWithoutExtension;


        public FontInfo()
        { }


        public FontInfo(Typography.OpenFont.Typeface font)
        {
            this.OpenFont = font;
            this.Path = font.FilePath;
            this.FileName = font.Filename;
            this.FileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(font.Filename);
            this.Typeface = SkiaSharp.SKTypeface.FromFile(this.Path);
        } // End Constuctor 


    } // End Class FontInfo 


} // End Namespace 
