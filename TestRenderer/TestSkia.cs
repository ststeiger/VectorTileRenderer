
using SkiaSharp;


namespace TestRenderer
{


    class TestSkia
    {


        public static byte[] DrawWithoutSurface()
        {
            byte[] pngBytes = null;

            // SKImageInfo nfo = new SKImageInfo();
            // SKBitmap bmp = new SKBitmap(300, 300, SKColorType.Rgba8888, SKAlphaType.Opaque);
            using (SKBitmap bmp = new SKBitmap(300, 300, SKColorType.Bgra8888, SKAlphaType.Premul))
            {
                using (SKCanvas canvas = new SKCanvas(bmp))
                {
                    canvas.DrawColor(SKColors.White); // Clear 

                    using (SKPaint paint = new SKPaint())
                    {

                        // paint.ImageFilter = SKImageFilter.CreateBlur(5, 5); // Dispose !
                        paint.IsAntialias = true;
                        // paint.Color = new SKColor(0xff, 0x00, 0xff);
                        paint.Color = new SKColor(0x2c, 0x3e, 0x50);
                        paint.StrokeCap = SKStrokeCap.Round;


                        paint.Typeface = SKTypeface.FromFamilyName("Linux Libertine G", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
                        paint.Typeface = SKTypeface.FromFamilyName("Segoe Script", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Italic);

                        paint.TextSize = 10;

                        canvas.DrawText("This is a test", 20, 20, paint);

                        paint.Typeface = SKTypeface.FromFamilyName("fadfasdjf", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
                        canvas.DrawText("This is a test with an unknown font", 20, 60, paint);
                    } // End Using paint 

                } // End Using canvas 



                using (SKImage skImg = SKImage.FromBitmap(bmp))
                {
                    using (SKData pngData = skImg.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        System.IO.File.WriteAllBytes(@"D:\TestSkia.png", pngData.ToArray());
                        pngBytes = pngData.ToArray();
                    }

                    using (SKData jpgData = skImg.Encode(SKEncodedImageFormat.Jpeg, 100))
                    {
                        System.IO.File.WriteAllBytes(@"D:\TestSkia.jpg", jpgData.ToArray());
                    }

                } // End Using skImg 



                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(pngBytes))
                {
                    using (System.Drawing.Image img = System.Drawing.Image.FromStream(ms))
                    {
                        img.Save(@"D:\TestSkia.tif", System.Drawing.Imaging.ImageFormat.Tiff);
                    }
                } // End Using ms 

            } // End Using bmp 

            return pngBytes;
        } // End Sub DrawWithoutSurface 


    }
}
