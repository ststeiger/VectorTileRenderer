﻿
namespace RasterTileServer.Controllers
{


    public class TilesController 
        : Microsoft.AspNetCore.Mvc.Controller
    {


        public const string STYLE = "bright-style";
        public static string CACHE_DIR = GetCacheDir();
        

        public static string GetBasePath()
        {
            // return @"D:\username\Documents\Visual Studio 2017\Projects\VectorTileRenderer";

            string bd = System.AppDomain.CurrentDomain.BaseDirectory;
            bd = System.IO.Path.Combine(bd, "..", "..", "..");
            bd = System.IO.Path.GetFullPath(bd);
            bd = System.IO.Path.Combine(bd, "wwwroot");

            return bd;
        }


        public static string GetCacheDir()
        {
            // return @"D:\username\Documents\Visual Studio 2017\Projects\VectorTileRenderer";

            string bd = System.AppDomain.CurrentDomain.BaseDirectory;
            bd = System.IO.Path.Combine(bd, "..", "..", "..");
            bd = System.IO.Path.GetFullPath(bd);
            bd = System.IO.Path.Combine(bd, "Cache", STYLE);

            return bd;
        }



        public static VectorTileRenderer.Style GetInitialStyle()
        {
            string basePath = GetBasePath();

            // load style and fonts
            string bright = System.IO.Path.Combine(basePath, "styles", STYLE + ".json");
            VectorTileRenderer.Style style = new VectorTileRenderer.Style(bright);
            style.FontDirectory = System.IO.Path.Combine(basePath, "styles", "fonts");

            // set mbtiles as tile provider
            string mb = @"D:\username\Downloads\2017-07-03_planet_z0_z14.mbtiles";
            // mb = @"C:\Users\User\Downloads\2017-07-03_france_monaco.mbtiles";
            VectorTileRenderer.Sources.MbTilesSource provider = new VectorTileRenderer.Sources.MbTilesSource(mb);
            style.SetSourceProvider(0, provider);

            // SELECT * FROM tiles WHERE tile_column /*x*/ = 1439 AND tile_row /*y*/ = 1542 AND zoom_level /*z*/ = 13 LIMIT 1 

            // https://github.com/klokantech/tileserver-gl-data

            return style;
        }

        public static VectorTileRenderer.Style s_style = GetInitialStyle();


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
            // bd = System.IO.Path.Combine(bd, "meer.png");

            // byte[] ba = System.IO.File.ReadAllBytes(bd);
            // byte[] ba = ba = System.Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAQAAAAEAAQMAAABmvDolAAAAA1BMVEWq09/P7Lz1AAAAH0lEQVRoge3BAQ0AAADCoPdPbQ43oAAAAAAAAAAAvg0hAAABmmDh1QAAAABJRU5ErkJggg==");
            // return new System.IO.MemoryStream(ba);

            string fn = "de" // lang.ToLowerInvariant()  
                + "_" + x.ToString(System.Globalization.CultureInfo.InvariantCulture) 
                + "_" + y.ToString(System.Globalization.CultureInfo.InvariantCulture) 
                + "_" + z.ToString(System.Globalization.CultureInfo.InvariantCulture) + ".png";

            fn = System.IO.Path.Combine(CACHE_DIR, fn);

            // Cached  
            if (System.IO.File.Exists(fn))
                return System.IO.File.OpenRead(fn);

            // render it on a skia canvas
            VectorTileRenderer.SkiaCanvas canvas = new VectorTileRenderer.SkiaCanvas();

            byte[] bitmap = await VectorTileRenderer.Renderer.Render(s_style, canvas, x, y, z, 256, 256, 1);
            System.IO.File.WriteAllBytes(fn, bitmap);

            return new System.IO.MemoryStream(bitmap);
        } // End Function GetTileStream 


        // https://localhost:44305/tiles/1/2/3
        // https://localhost:44305/tiles/1/2/3.png
        // tiles/{x:int}/{y:int}/{z:int}
        [Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("tiles/{x:int}/{y:int}/{z:int}")]
        [Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("tiles/{x:int}/{y:int}/{z:int}.png")]
        public async System.Threading.Tasks.Task< Microsoft.AspNetCore.Mvc.FileStreamResult> GetTile(int x, int y, int z, string lang)
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