
namespace TestRenderer
{


    public static class Program
    {



        // https://ftp.fau.de/osm-planet/pbf/
        // https://planet.openstreetmap.org/
        // http://ftp.snt.utwente.nl/pub/misc/openstreetmap/
        // https://free.nchc.org.tw/osm.planet/planet/
        public static async System.Threading.Tasks.Task FromPBF(int x, int y, int zoom)
        {
            string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            basePath = System.IO.Path.Combine(basePath, "..", "..", "..");
            basePath = System.IO.Path.GetFullPath(basePath);


            string basic = System.IO.Path.Combine(basePath, "styles", "basic-style.json");

            // load style and fonts
            VectorTileRenderer.Style style = new VectorTileRenderer.Style(basic);
            style.FontDirectory = System.IO.Path.Combine(basePath, "styles", "fonts");

            // set pbf as tile provider
            string pbfFile = @"D:\username\Downloads\tile.pbf";
            VectorTileRenderer.Sources.PbfTileSource provider = new VectorTileRenderer.Sources.PbfTileSource(pbfFile);
            style.SetSourceProvider(0, provider);

            // render it on a skia canvas
            VectorTileRenderer.SkiaCanvas canvas = new VectorTileRenderer.SkiaCanvas();
            byte[] bitmap = await VectorTileRenderer.Renderer.Render(style, canvas, x, y, zoom, 512, 512, 1);

            //using (System.Drawing.Bitmap bmp = WindowsMediaConversion.BitmapFromSource2(bitmap))
            //{
            //    bmp.Save(@"D:\TileFromPBF.png", System.Drawing.Imaging.ImageFormat.Png);
            //} // End Using bmp 

        } // End Task FromPBF 


        public static string[] GetTileTables()
        {
            string[] tiles = new string[15];
            for (int i = 0; i < tiles.Length; ++i)
            {
                tiles[i] = "tiles_" + i.ToString(System.Globalization.CultureInfo.InvariantCulture).PadLeft(2, '0');
            } // Next i 

            return tiles;
        } // End Function GetTileTables 


        public static string[] GetPlanetFiles(string basePath)
        {
            string[] files = new string[15];

            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = System.IO.Path.Combine(basePath, "planet_" + i.ToString(System.Globalization.CultureInfo.InvariantCulture).PadLeft(2, '0') + ".db3");
            } // Next i 

            return files;
        } // End Function GetPlanetFiles 


        // https://github.com/AliFlux/VectorTileRenderer
        public static async System.Threading.Tasks.Task FromMbTiles(int x, int y, int zoom)
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
                // mb = @"C:\Users\User\Downloads\2017-07-03_france_monaco.mbtiles";
                VectorTileRenderer.Sources.MbTilesSource provider = new VectorTileRenderer.Sources.MbTilesSource(mb);
                style.SetSourceProvider(0, provider);

                // SELECT * FROM tiles WHERE tile_column /*x*/ = 1439 AND tile_row /*y*/ = 1542 AND zoom_level /*z*/ = 13 LIMIT 1 

                // https://github.com/klokantech/tileserver-gl-data

                // render it on a skia canvas
                VectorTileRenderer.SkiaCanvas canvas = new VectorTileRenderer.SkiaCanvas();
                 byte[] bitmap = await VectorTileRenderer.Renderer.Render(style, canvas, x, y, zoom, 512, 512, 1);


                System.IO.File.WriteAllBytes(@"D:\TileFromMbTiles.png", bitmap);


                //using (System.IO.Stream ms = new System.IO.MemoryStream(bitmap))
                //{
                //    using (System.Drawing.Image img = System.Drawing.Image.FromStream(ms))
                //    {
                //        img.Save(@"E:\TileFromMbTiles.png", System.Drawing.Imaging.ImageFormat.Png);
                //    }
                //}
                



                //using (System.Drawing.Bitmap bmp = WindowsMediaConversion.BitmapFromSource2(bitmap))
                //{
                //    bmp.Save(@"D:\TileFromMbTiles.png", System.Drawing.Imaging.ImageFormat.Png);
                //} // End Using bmp 

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

            // TestSkia.DrawWithoutSurface();


            // ZH: 47.385004471044475 8.516975793094163
            // float latitude = 47.385004471044475f;
            // float longitude = 8.516975793094163f;

            // MC: 43.73437528735218 7.420633512081709
            float latitude = 43.73437528735218f;
            float longitude = 7.420633512081709f;
            
            int zoom = 13; // zoom_level = z
            VectorTileRenderer.Point p = TmsHelper.Wgs84ToTms(latitude, longitude, zoom);
            int x = (int)p.X; // tile_column = x
            int y = (int)p.Y; // tile_row = y
            y = TmsHelper.FromTmsY(y, zoom);

            string sql = string.Format("SELECT * FROM tiles WHERE tile_column = {0} AND tile_row = {1} AND zoom_level = {2}", x, y, zoom);
            System.Console.WriteLine(sql);


            System.Threading.Tasks.Task renderTask = System.Threading.Tasks.Task.Run(
                async () =>
                {
                    await FromMbTiles(x,y,zoom);
                    // await FromPBF(x, y, zoom);
                }
            );

            renderTask.Wait();
        } // End Sub Main 


    } // End Class Program 


} // End Namespace TestRenderer 
