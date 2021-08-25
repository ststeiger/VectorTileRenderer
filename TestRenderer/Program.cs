
namespace TestRenderer
{


    static class Program
    {


        private static System.Drawing.Bitmap BitmapFromSource(System.Windows.Media.Imaging.BitmapSource bitmapsource)
        {
            System.Drawing.Bitmap bitmap;
            using (System.IO.MemoryStream outStream = new System.IO.MemoryStream())
            {
                System.Windows.Media.Imaging.BitmapEncoder enc = new System.Windows.Media.Imaging.BmpBitmapEncoder();
                enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            } // End Using outStream 

            return bitmap;
        } // End Function BitmapFromSource 


        public static System.Drawing.Bitmap BitmapFromSource2(System.Windows.Media.Imaging.BitmapSource bitmapsource)
        {
            // Convert image format
            System.Windows.Media.Imaging.FormatConvertedBitmap src = new System.Windows.Media.Imaging.FormatConvertedBitmap();
            src.BeginInit();
            src.Source = bitmapsource;
            src.DestinationFormat = System.Windows.Media.PixelFormats.Bgra32;
            src.EndInit();

            // Copy to bitmap
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(src.PixelWidth, src.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            src.CopyPixels(System.Windows.Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bitmap.UnlockBits(data);

            return bitmap;
        } // End Function BitmapFromSource2 



        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int FromTmsY(int tmsY, int zoom)
        {
            return (1 << zoom) - tmsY - 1; // 2^zoom - tmsY - 1
        } // End Function FromTmsY 


        // https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#VB.Net
        private static System.Drawing.Point Wgs84ToTileXY(float lat, float lon, int zoom)
        {
            System.Drawing.Point ret = new System.Drawing.Point();

            // ret.X = System.Convert.ToInt32(System.Math.Floor((lon + 180) / (double)360 * System.Math.Pow(2, zoom)));
            // ret.Y = System.Convert.ToInt32(System.Math.Floor((1 - System.Math.Log(System.Math.Tan(lat * System.Math.PI / 180) + 1 / System.Math.Cos(lat * System.Math.PI / 180)) / System.Math.PI) / 2 * System.Math.Pow(2, zoom)));

            ret.X = System.Convert.ToInt32(System.Math.Floor((lon + 180) / (double)360 * (1 << zoom)));
            ret.Y = System.Convert.ToInt32(System.Math.Floor((1 - System.Math.Log(System.Math.Tan(lat * System.Math.PI / 180) + 1 / System.Math.Cos(lat * System.Math.PI / 180)) / System.Math.PI) / 2 * (1 << zoom)));

            return ret;
        } // End Function Wgs84ToTileXY 


        public static async System.Threading.Tasks.Task FromPBF()
        {
            // load style and fonts
            VectorTileRenderer.Style style = new VectorTileRenderer.Style("basic-style.json");
            style.FontDirectory = "styles/fonts/";

            // set pbf as tile provider
            VectorTileRenderer.Sources.PbfTileSource provider = new VectorTileRenderer.Sources.PbfTileSource("tile.pbf");
            style.SetSourceProvider(0, provider);

            // render it on a skia canvas
            int zoom = 13;
            VectorTileRenderer.SkiaCanvas canvas = new VectorTileRenderer.SkiaCanvas();
            System.Windows.Media.Imaging.BitmapSource bitmap = await VectorTileRenderer.Renderer.Render(style, canvas, 0, 0, zoom, 512, 512, 1);
            
            using (System.Drawing.Bitmap bmp = BitmapFromSource2(bitmap))
            {
                bmp.Save(@"D:\TileFromPBF.png", System.Drawing.Imaging.ImageFormat.Png);
            } // End Using bmp 

        } // End Task FromPBF 


        // https://github.com/AliFlux/VectorTileRenderer
        public static async System.Threading.Tasks.Task FromMbTiles()
        {
            // D:\Stefan.Steiger\Downloads\2017-07-03_planet_z0_z14.mbtiles

            string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            basePath = System.IO.Path.Combine(basePath, "..", "..", "..");
            basePath = System.IO.Path.GetFullPath(basePath);

            try
            {
                // load style and fonts
                string bright = System.IO.Path.Combine(basePath, "styles", "bright-style.json");
                VectorTileRenderer.Style style = new VectorTileRenderer.Style(bright);
                style.FontDirectory = System.IO.Path.Combine(basePath, "styles", "fonts");



                // set mbtiles as tile provider
                string mb = @"D:\username\Downloads\2017-07-03_planet_z0_z14.mbtiles";
                VectorTileRenderer.Sources.MbTilesSource provider = new VectorTileRenderer.Sources.MbTilesSource(mb);
                style.SetSourceProvider(0, provider);

                // 47.385004471044475 8.516975793094163
                float latitude = 47.385004471044475f;
                float longitude = 8.516975793094163f;


                int zoom = 13; // zoom_level = z
                int x = 1439; // tile_column = x
                int y = 1542; // tile_row = y

                zoom = 16;
                System.Drawing.Point p = Wgs84ToTileXY(latitude, longitude, zoom);
                x = (int) p.X;
                y = (int)p.Y;
                y = FromTmsY(y, zoom);

                string sql = string.Format("SELECT * FROM tiles WHERE tile_column = {0} AND tile_row = {1} AND zoom_level = {2}", x, y, zoom);
                System.Console.WriteLine(sql);

                /*
                SELECT * FROM tiles 
                WHERE tile_column = 1439 -- x
                AND tile_row = 1542  -- y 
                AND zoom_level = 13 -- z
                LIMIT 1 
                */


                // https://github.com/klokantech/tileserver-gl-data

                // render it on a skia canvas
                VectorTileRenderer.SkiaCanvas canvas = new VectorTileRenderer.SkiaCanvas();
                System.Windows.Media.Imaging.BitmapSource bitmap = await VectorTileRenderer.Renderer.Render(style, canvas, x, y, zoom, 512, 512, 1);

                using (System.Drawing.Bitmap bmp = BitmapFromSource2(bitmap))
                {
                    bmp.Save(@"D:\TileFromMbTiles.png", System.Drawing.Imaging.ImageFormat.Png);
                } // End Using bmp 

            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine(ex.StackTrace);
            }
        } // End Task FromMbTiles 


        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [System.STAThread]
        static void Main()
        {
#if false
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
#endif
            TestSkia.DrawWithoutSurface();

            System.Threading.Tasks.Task renderTask = System.Threading.Tasks.Task.Run(
                async () =>
                {
                    await FromMbTiles();
                }
            );

            renderTask.Wait();
        } // End Sub Main 


    } // End Class Program 


} // End Namespace TestRenderer 
