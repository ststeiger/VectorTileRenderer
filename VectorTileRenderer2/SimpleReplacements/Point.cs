
namespace VectorTileRenderer
{

    public struct Point
    {
        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double Y { get; set; }
        public double X { get; set; }


        public static bool operator !=(Point point1, Point point2)
        {
            return !point1.Equals(point2);
        }

        public static bool operator ==(Point point1, Point point2)
        {
            return point1.Equals(point2);
        }

    }

}
