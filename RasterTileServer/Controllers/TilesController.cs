
namespace RasterTileServer.Controllers
{


    public class TilesController 
        : Microsoft.AspNetCore.Mvc.Controller
    {


        protected const string STYLE = "bright-style";
        protected const string NOTHING_TO_SEE_HERE = "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";
        

        private static string GetBasePath()
        {
            // return @"D:\username\Documents\Visual Studio 2017\Projects\VectorTileRenderer";

            string bd = System.AppDomain.CurrentDomain.BaseDirectory;
            bd = System.IO.Path.Combine(bd, "..", "..", "..");
            bd = System.IO.Path.GetFullPath(bd);
            bd = System.IO.Path.Combine(bd, "wwwroot");

            return bd;
        }


        private static string GetCacheDir()
        {
            // return @"D:\username\Documents\Visual Studio 2017\Projects\VectorTileRenderer";

            string bd = System.AppDomain.CurrentDomain.BaseDirectory;
            bd = System.IO.Path.Combine(bd, "..", "..", "..");
            bd = System.IO.Path.GetFullPath(bd);

            bd = System.IO.Path.Combine(bd, "Cache");

            if (!System.IO.Directory.Exists(bd))
                System.IO.Directory.CreateDirectory(bd);

            bd = System.IO.Path.Combine(bd, STYLE);

            if (!System.IO.Directory.Exists(bd))
                System.IO.Directory.CreateDirectory(bd);

            return bd;
        }



        private static VectorTileRenderer.Style GetInitialStyle()
        {
            string basePath = GetBasePath();

            // load style and fonts
            string bright = System.IO.Path.Combine(basePath, "styles", STYLE + ".json");
            VectorTileRenderer.Style style = new VectorTileRenderer.Style(bright);
            style.FontDirectory = System.IO.Path.Combine(basePath, "styles", "fonts");

            // set mbtiles as tile provider
            string mb = @"C:\Users\User\Documents\OSM\2017-07-03_planet_z0_z14.mbtiles";
            if ("COR".Equals(System.Environment.UserDomainName, System.StringComparison.InvariantCultureIgnoreCase))
                mb = @"D:\username\Downloads\2017-07-03_planet_z0_z14.mbtiles";

            VectorTileRenderer.Sources.MbTilesSource provider = new VectorTileRenderer.Sources.MbTilesSource(mb);
            style.SetSourceProvider(0, provider);

            // SELECT * FROM tiles WHERE tile_column /*x*/ = 1439 AND tile_row /*y*/ = 1542 AND zoom_level /*z*/ = 13 LIMIT 1 

            // https://github.com/klokantech/tileserver-gl-data

            return style;
        }


        private static readonly string CACHE_DIR = GetCacheDir();
        private static readonly VectorTileRenderer.Style s_style = GetInitialStyle();
        private static readonly byte[] nothing_to_see_here = System.Convert.FromBase64String(NOTHING_TO_SEE_HERE);



        // https://alastaira.wordpress.com/2011/07/06/converting-tms-tile-coordinates-to-googlebingosm-tile-coordinates/
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int FromTmsY(int tmsY, int zoom)
        {
            return (1 << zoom) - tmsY - 1; // 2^zoom - tmsY - 1
        } // End Function FromTmsY 


        public static async System.Threading.Tasks.Task<System.IO.Stream> GetTileStream(int x, int y, int z)
        {
            // string bd = System.AppDomain.CurrentDomain.BaseDirectory;
            // bd = System.IO.Path.Combine(bd, "..", "..", "..");
            // bd = System.IO.Path.GetFullPath(bd);
            // bd = System.IO.Path.Combine(bd, "wwwroot");
            // bd = System.IO.Path.Combine(bd, "sea.png");

            // byte[] ba = System.IO.File.ReadAllBytes(bd);
            // byte[] ba = ba = System.Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAQAAAAEAAQMAAABmvDolAAAAA1BMVEWq09/P7Lz1AAAAH0lEQVRoge3BAQ0AAADCoPdPbQ43oAAAAAAAAAAAvg0hAAABmmDh1QAAAABJRU5ErkJggg==");
            // return new System.IO.MemoryStream(ba);

            string fileName = "de" // lang.ToLowerInvariant()  
                + "_" + x.ToString(System.Globalization.CultureInfo.InvariantCulture) 
                + "_" + y.ToString(System.Globalization.CultureInfo.InvariantCulture) 
                + "_" + z.ToString(System.Globalization.CultureInfo.InvariantCulture) + ".png";

            fileName = System.IO.Path.Combine(CACHE_DIR, fileName);

            // Cached  
            // if (System.IO.File.Exists(fileName)) return System.IO.File.OpenRead(fileName);

            byte[] bitmap = await VectorTileRenderer.Renderer.Render(s_style, x, y, z, 256, 256, 1);
            // System.IO.File.WriteAllBytes(fileName, bitmap);

            if(bitmap != null)
                return new System.IO.MemoryStream(bitmap);

            return new System.IO.MemoryStream(nothing_to_see_here);
        } // End Function GetTileStream 



        // Arabic test:
        // https://localhost:44305/tiles/63157/52354/17.png?lang=en&no_cache=1630490579563
        // https://localhost:44305/tiles/15919/13086/15.png?lang=en&no_cache=1630591921139
        // https://localhost:44305/tiles/15892/12946/15.png?lang=en&no_cache=1630672015619
        // https://localhost:44305/tiles/7940/6541/14.png?lang=en&no_cache=1630671597883

        // https://localhost:44305/tiles/1/2/3
        // https://localhost:44305/tiles/1/2/3.png


        /*         
https://localhost:44372/tiles/2917/1555/12.png?lang=en&no_cache=1631046405333

https://localhost:44372/tiles/11658/6219/14.png?lang=en&no_cache=1631046405333

https://localhost:44372/tiles/12287/6653/14.png?lang=en&no_cache=1631046405333

https://localhost:44372/tiles/12288/6653/14.png?lang=en&no_cache=1631046405333

https://localhost:44372/tiles/1671/750/11.png?lang=en&no_cache=1631046405333

https://localhost:44372/tiles/12287/6653/14.png?lang=en&no_cache=1631048235483
https://localhost:44372/tiles/12288/6653/14.png?lang=en&no_cache=1631048235483
https://localhost:44372/tiles/52115/29026/16.png?lang=en&no_cache=1631048552543

         */


        // tiles/{x:int}/{y:int}/{z:int}
        [Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("tiles/{x:int}/{y:int}/{z:int}")]
        [Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("tiles/{x:int}/{y:int}/{z:int}.png")]
        public async System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.FileStreamResult> GetTile(int x, int y, int z) //, [FromHeader] string lang)
        {
            y = FromTmsY(y, z);

            System.IO.Stream stream = await GetTileStream(x, y, z);
            if (stream == null)
            {
                Response.StatusCode = 404;
                return null;
            }


            Response.StatusCode = 200;
            Response.Headers["accept-ranges"] = "bytes";
            Response.Headers["access-control-allow-origin"] = "*";
            Response.Headers["cache-control"] = "public, max-age=86400, no-transform";

            Response.Headers["content-length"] = stream.Length.ToString(System.Globalization.CultureInfo.InvariantCulture);
            // Response.Headers["date"] = "Sat, 09 Feb 2019 11:06:10 GMT";
            // Response.Headers["expect-ct"] = "max-age=604800, report-uri="https://report-uri.cloudflare.com/cdn-cgi/beacon/expect-ct";
            // Response.Headers["last-modified"] = "Thu, 07 Feb 2019 11:44:10 GMT";
            Response.Headers["vary"] = "Accept-Encoding";

            return new Microsoft.AspNetCore.Mvc.FileStreamResult(stream, "image/png")
            {
                // EntityTag = 
                // LastModified = 
                // FileDownloadName = "test.txt"
            };
        } // End Function GetTile 


    } // End Class TilesController : Controller 


} // End Namespace RasterTileServer.Controllers 
