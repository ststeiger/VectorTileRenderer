
namespace VectorTileRenderer
{


    public class VectorTile
    {
        public bool IsOverZoomed { get; set; } = false;

        public System.Collections.Generic.List<VectorTileLayer> Layers = 
            new System.Collections.Generic.List<VectorTileLayer>();


        public VectorTile ApplyExtent(Rect extent)
        {
            VectorTile newTile = new VectorTile();
            newTile.IsOverZoomed = this.IsOverZoomed;
            
            foreach(VectorTileLayer layer in Layers)
            {
                VectorTileLayer vectorLayer = new VectorTileLayer();
                vectorLayer.Name = layer.Name;

                foreach (VectorTileFeature feature in layer.Features)
                {
                    VectorTileFeature vectorFeature = new VectorTileFeature();
                    vectorFeature.Attributes = new System.Collections.Generic.Dictionary<string, object>(feature.Attributes);
                    vectorFeature.Extent = feature.Extent;
                    vectorFeature.GeometryType = feature.GeometryType;

                    System.Collections.Generic.List<System.Collections.Generic.List<Point>> vectorGeometry = new System.Collections.Generic.List<System.Collections.Generic.List<Point>>();
                    foreach (System.Collections.Generic.List<Point> geometry in feature.Geometry)
                    {
                        System.Collections.Generic.List<Point> vectorPoints = new System.Collections.Generic.List<Point>();

                        foreach (Point point in geometry)
                        {
                            double newX = Utils.ConvertRange(point.X, extent.Left, extent.Right, 0, vectorFeature.Extent);
                            double newY = Utils.ConvertRange(point.Y, extent.Top, extent.Bottom, 0, vectorFeature.Extent);

                            vectorPoints.Add(new Point(newX, newY));
                        } // Next point 

                        vectorGeometry.Add(vectorPoints);
                    } // Next geometry 

                    vectorFeature.Geometry = vectorGeometry;
                    vectorLayer.Features.Add(vectorFeature);
                } // Next feature 

                newTile.Layers.Add(vectorLayer);
            } // Next layer 

            return newTile;
        } // End Function ApplyExtent 


    } // End Class VectorTile 


    public class VectorTileLayer
    {
        public string Name { get; set; }

        public System.Collections.Generic.List<VectorTileFeature> Features = 
            new System.Collections.Generic.List<VectorTileFeature>();
    } // End Class VectorTileLayer 


    public class VectorTileFeature
    {
        public double Extent { get; set; }
        public string GeometryType { get; set; }

        public System.Collections.Generic.Dictionary<string, object> Attributes = 
            new System.Collections.Generic.Dictionary<string, object>();

        public System.Collections.Generic.List<System.Collections.Generic.List<Point>> Geometry = 
            new System.Collections.Generic.List<System.Collections.Generic.List<Point>>();
    } // End Class VectorTileFeature 


} // End Namespace 
