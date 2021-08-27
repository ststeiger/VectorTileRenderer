
namespace VectorTileRenderer
{

    using System.Linq;


    public class Renderer
    {
        // TODO make it instance based... maybe
        private static object cacheLock = new object();

        enum VisualLayerType
        {
            Vector,
            Raster,
        }

        class VisualLayer
        {
            public VisualLayerType Type { get; set; }

            public System.IO.Stream RasterStream { get; set; } = null;

            public VectorTileFeature VectorTileFeature { get; set; } = null;

            public System.Collections.Generic.List<System.Collections.Generic.List<Point>> Geometry {get;set;} = null;

            public Brush Brush { get; set; } = null;
        }


        /*
        public async static System.Threading.Tasks.Task<BitmapSource> RenderCached(
            string cachePath, 
            Style style, 
            ICanvas canvas, 
            int x, 
            int y, 
            double zoom, 
            double sizeX = 512, 
            double sizeY = 512, 
            double scale = 1)
        {
            var bundle = new
            {
                style.Hash,
                sizeX,
                sizeY,
                scale,
            };

            lock (cacheLock)
            {
                if (!System.IO.Directory.Exists(cachePath))
                {
                    System.IO.Directory.CreateDirectory(cachePath);
                }
            }

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(bundle);
            string hash = Utils.Sha256(json).Substring(0, 12); // get 12 digits to avoid fs length issues

            string fileName = x + "x" + y + "-" + zoom + "-" + hash + ".png";
            string path = System.IO.Path.Combine(cachePath, fileName);
            
            lock(cacheLock)
            {
                if (System.IO.File.Exists(path))
                {
                    return LoadBitmap(path);
                }
            }

            BitmapSource bitmap = await Render(style, canvas, x, y, zoom, sizeX, sizeY, scale);

            if(bitmap != null)
            {
                try
                {
                    lock (cacheLock)
                    {
                        if (System.IO.File.Exists(path))
                        {
                            return LoadBitmap(path);
                        } // End if (System.IO.File.Exists(path)) 

                        using (System.IO.FileStream fileStream = new System.IO.FileStream(path, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite))
                        {
                            BitmapEncoder encoder = new PngBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(bitmap.WindowsShit));
                            encoder.Save(fileStream);
                        } // End Using fileStream 

                    } // End lock (cacheLock) 
                }
                catch (System.Exception e)
                {
                    return null;
                }
            } // End if(bitmap != null) 

            return bitmap;
        } // End Function RenderCached 



        private static BitmapSource LoadBitmap(string path)
        {
            using (System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
            {
                BitmapImage fsBitmap = new BitmapImage();
                fsBitmap.BeginInit();
                fsBitmap.StreamSource = stream;
                fsBitmap.CacheOption = BitmapCacheOption.OnLoad;
                fsBitmap.EndInit();
                fsBitmap.Freeze();

                return fsBitmap;
            }
        } // End Function LoadBitmap 
        */


        public static async System.Threading.Tasks.Task<byte[]> Render(
            Style style, 
            int x,
            int y,
            double zoom,
            double sizeX = 512,
            double sizeY = 512,
            double scale = 1)
        {
            byte[] bitmap = null;

            using (SkiaCanvas canvas = new SkiaCanvas())
            {
                bitmap = await Render(style, canvas, x, y, zoom, sizeX, sizeY, scale);
            }
            
            return bitmap;
        }

        internal async static System.Threading.Tasks.Task<byte[]> Render(
            Style style, 
            ICanvas canvas, 
            int x, 
            int y, 
            double zoom, 
            double sizeX = 512, 
            double sizeY = 512, 
            double scale = 1)
        {
            System.Collections.Generic.Dictionary<Source, System.IO.Stream> rasterTileCache = 
                new System.Collections.Generic.Dictionary<Source, System.IO.Stream>();

            System.Collections.Generic.Dictionary<Source, VectorTile> vectorTileCache = 
                new System.Collections.Generic.Dictionary<Source, VectorTile>();

            System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<VectorTileLayer>> categorizedLayers = 
                new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<VectorTileLayer>>();

            double actualZoom = zoom;

            if (sizeX < 1024)
            {
                double ratio = 1024 / sizeX;
                double zoomDelta = System.Math.Log(ratio, 2);

                actualZoom = zoom - zoomDelta;
            } // End if (sizeX < 1024) 

            sizeX *= scale;
            sizeY *= scale;
            
            canvas.StartDrawing(sizeX, sizeY);

            // TODO refactor tuples to struct
            System.Collections.Generic.List<VisualLayer> visualLayers = 
                new System.Collections.Generic.List<VisualLayer>();

            // TODO refactor this messy block
            foreach (Layer layer in style.Layers)
            {
                if (layer.Source != null)
                {
                    if (layer.Source.Type == "vector")
                    {
                        if (!vectorTileCache.ContainsKey(layer.Source))
                        {
                            if (layer.Source.Provider is Sources.IVectorTileSource)
                            {
                                VectorTile tile = await (layer.Source.Provider as Sources.IVectorTileSource).GetVectorTile(x, y, (int)zoom);

                                if (tile == null)
                                {
                                    return null;
                                    // throwing exceptions screws up the performance
                                    throw new System.IO.FileNotFoundException("Could not load tile : " + x + "," + y + "," + zoom + " of " + layer.SourceName);
                                }

                                // magic sauce! :p
                                if (tile.IsOverZoomed)
                                {
                                    canvas.ClipOverflow = true;
                                }

                                //canvas.ClipOverflow = true;

                                vectorTileCache[layer.Source] = tile;

                                // normalize the points from 0 to size
                                foreach (VectorTileLayer vectorLayer in tile.Layers)
                                {
                                    foreach (VectorTileFeature feature in vectorLayer.Features)
                                    {
                                        foreach (System.Collections.Generic.List<Point> geometry in feature.Geometry)
                                        {
                                            for (int i = 0; i < geometry.Count; i++)
                                            {
                                                Point point = geometry[i];
                                                geometry[i] = new Point(point.X / feature.Extent * sizeX, point.Y / feature.Extent * sizeY);
                                            }
                                        }
                                    }
                                }

                                foreach (VectorTileLayer tileLayer in tile.Layers)
                                {
                                    if (!categorizedLayers.ContainsKey(tileLayer.Name))
                                    {
                                        categorizedLayers[tileLayer.Name] = new System.Collections.Generic.List<VectorTileLayer>();
                                    }
                                    categorizedLayers[tileLayer.Name].Add(tileLayer);
                                }
                            }
                        }
                    }
                    else if (layer.Source.Type == "raster")
                    {
                        if (!rasterTileCache.ContainsKey(layer.Source))
                        {
                            if(layer.Source.Provider != null)
                            {
                                if (layer.Source.Provider is Sources.ITileSource)
                                {
                                    System.IO.Stream tile = await layer.Source.Provider.GetTile(x, y, (int)zoom);

                                    if (tile == null)
                                    {
                                        return null;
                                        // throwing exceptions screws up the performance
                                        throw new System.IO.FileNotFoundException("Could not load tile : " + x + "," + y + "," + zoom + " of " + layer.SourceName);
                                    }

                                    rasterTileCache[layer.Source] = tile;
                                }
                            }
                        }

                        if(rasterTileCache.ContainsKey(layer.Source))
                        {
                            Brush brush = style.ParseStyle(layer, scale, new System.Collections.Generic.Dictionary<string, object>());

                            visualLayers.Add(new VisualLayer()
                            {
                                Type = VisualLayerType.Raster,
                                RasterStream = rasterTileCache[layer.Source],
                                Brush = brush,
                            });
                        }
                    }

                    if (categorizedLayers.ContainsKey(layer.SourceLayer))
                    {
                        System.Collections.Generic.List<VectorTileLayer> tileLayers = categorizedLayers[layer.SourceLayer];

                        foreach (VectorTileLayer tileLayer in tileLayers)
                        {
                            foreach (VectorTileFeature feature in tileLayer.Features)
                            {
                                //List<List<Point>> geometry = LocalizeGeometry(feature.Geometry, sizeX, sizeY, feature.Extent);
                                System.Collections.Generic.Dictionary<string, object> attributes = 
                                    new System.Collections.Generic.Dictionary<string, object>(feature.Attributes);

                                attributes["$type"] = feature.GeometryType;
                                attributes["$id"] = layer.ID;
                                attributes["$zoom"] = actualZoom;

                                if (style.ValidateLayer(layer, actualZoom, attributes))
                                {
                                    Brush brush = style.ParseStyle(layer, scale, attributes);

                                    if (!brush.Paint.Visibility)
                                    {
                                        continue;
                                    }
                                    
                                    visualLayers.Add(new VisualLayer()
                                    {
                                        Type = VisualLayerType.Vector,
                                        VectorTileFeature = feature,
                                        Geometry = feature.Geometry,
                                        Brush = brush,
                                    });
                                }
                            }
                        }
                    }

                }
                else if (layer.Type == "background")
                {
                    Brush[] brushes = style.GetStyleByType("background", actualZoom, scale);
                    foreach(Brush brush in brushes)
                    {
                        canvas.DrawBackground(brush);
                    }
                }
            }

            // defered rendering to preserve text drawing order
            foreach (VisualLayer layer in visualLayers.OrderBy(item => item.Brush.ZIndex))
            {
                if(layer.Type == VisualLayerType.Vector)
                {
                    VectorTileFeature feature = layer.VectorTileFeature;
                    System.Collections.Generic.List<System.Collections.Generic.List<Point>> geometry = layer.Geometry;
                    Brush brush = layer.Brush;

                    System.Collections.Generic.Dictionary<string, object> attributesDict = feature.Attributes.ToDictionary(key => key.Key, value => value.Value);

                    if (!brush.Paint.Visibility)
                    {
                        continue;
                    }

                    if (feature.GeometryType == "Point")
                    {
                        foreach (System.Collections.Generic.List<Point> point in geometry)
                        {
                            canvas.DrawPoint(point.First(), brush);
                        }
                    }
                    else if (feature.GeometryType == "LineString")
                    {
                        foreach (System.Collections.Generic.List<Point> line in geometry)
                        {
                            canvas.DrawLineString(line, brush);
                        }
                    }
                    else if (feature.GeometryType == "Polygon")
                    {
                        foreach (System.Collections.Generic.List<Point> polygon in geometry)
                        {
                            canvas.DrawPolygon(polygon, brush);
                        }
                    }
                    else if (feature.GeometryType == "Unknown")
                    {
                        canvas.DrawUnknown(geometry, brush);
                    }
                }
                else if(layer.Type == VisualLayerType.Raster)
                {
                    canvas.DrawImage(layer.RasterStream, layer.Brush);
                    layer.RasterStream.Close();
                }
            }

            foreach (VisualLayer layer in visualLayers.OrderBy(item => item.Brush.ZIndex).Reverse())
            {
                if (layer.Type == VisualLayerType.Vector)
                {
                    VectorTileFeature feature = layer.VectorTileFeature;
                    System.Collections.Generic.List<System.Collections.Generic.List<Point>> geometry = layer.Geometry;
                    Brush brush = layer.Brush;

                    System.Collections.Generic.Dictionary<string, object> attributesDict = 
                        feature.Attributes.ToDictionary(key => key.Key, value => value.Value);

                    if (!brush.Paint.Visibility)
                    {
                        continue;
                    } // End if (!brush.Paint.Visibility) 

                    if (feature.GeometryType == "Point")
                    {
                        foreach (System.Collections.Generic.List<Point> point in geometry)
                        {
                            if (brush.Text != null)
                            {
                                canvas.DrawText(point.First(), brush);
                            }
                        }
                    }
                    else if (feature.GeometryType == "LineString")
                    {
                        foreach (System.Collections.Generic.List<Point> line in geometry)
                        {
                            if (brush.Text != null)
                            {
                                canvas.DrawTextOnPath(line, brush);
                            } // End if (brush.Text != null) 

                        } // Next line 
                    } 

                } // End if (layer.Type == VisualLayerType.Vector) 

            } // Next layer 

            return canvas.FinishDrawing();
        } // End Task Render 
        

        private static System.Collections.Generic.List<System.Collections.Generic.List<Point>> LocalizeGeometry(
            System.Collections.Generic.List<System.Collections.Generic.List<Point>> coordinates, 
            double sizeX, 
            double sizeY, 
            double extent)
        {
            return coordinates.Select(list =>
            {
                return list.Select(point =>
                {
                    Point newPoint = new Point(0, 0);

                    double x = Utils.ConvertRange(point.X, 0, extent, 0, sizeX, false);
                    double y = Utils.ConvertRange(point.Y, 0, extent, 0, sizeY, false);

                    newPoint.X = x;
                    newPoint.Y = y;

                    return newPoint;
                }).ToList();
            }).ToList();
        } // End Function LocalizeGeometry 


    } // End Class Renderer 


} // End Namespace VectorTileRenderer 
