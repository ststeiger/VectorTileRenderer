
namespace TestRenderer
{
    public static class TmsHelper
    {



        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int FromTmsY(int tmsY, int zoom)
        {
            return (1 << zoom) - tmsY - 1; // 2^zoom - tmsY - 1
        } // End Function FromTmsY 


        // https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#VB.Net
        public static VectorTileRenderer.Point Wgs84ToTms(float lat, float lon, int zoom)
        {
            VectorTileRenderer.Point ret = new VectorTileRenderer.Point();

            // ret.X = System.Convert.ToInt32(System.Math.Floor((lon + 180) / (double)360 * System.Math.Pow(2, zoom)));
            // ret.Y = System.Convert.ToInt32(System.Math.Floor((1 - System.Math.Log(System.Math.Tan(lat * System.Math.PI / 180) + 1 / System.Math.Cos(lat * System.Math.PI / 180)) / System.Math.PI) / 2 * System.Math.Pow(2, zoom)));

            ret.X = System.Convert.ToInt32(System.Math.Floor((lon + 180) / (double)360 * (1 << zoom)));
            ret.Y = System.Convert.ToInt32(System.Math.Floor((1 - System.Math.Log(System.Math.Tan(lat * System.Math.PI / 180) + 1 / System.Math.Cos(lat * System.Math.PI / 180)) / System.Math.PI) / 2 * (1 << zoom)));

            return ret;
        } // End Function Wgs84ToTileXY 


    }
}
