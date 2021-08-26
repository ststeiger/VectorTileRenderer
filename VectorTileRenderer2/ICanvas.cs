
namespace VectorTileRenderer
{


    public interface ICanvas
    {
        bool ClipOverflow { get; set; }

        void StartDrawing(double sizeX, double sizeY);

        void DrawBackground(Brush style);

        void DrawLineString(System.Collections.Generic.List<Point> geometry, Brush style);

        void DrawPolygon(System.Collections.Generic.List<Point> geometry, Brush style);

        void DrawPoint(Point geometry, Brush style);

        void DrawText(Point geometry, Brush style);

        void DrawTextOnPath(System.Collections.Generic.List<Point> geometry, Brush style);

        void DrawImage(System.IO.Stream imageStream, Brush style);

        void DrawUnknown(System.Collections.Generic.List<System.Collections.Generic.List<Point>> geometry, Brush style);

        byte[] FinishDrawing();
    }


}
