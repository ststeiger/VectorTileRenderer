
namespace VectorTileRenderer
{

    using System.Linq;


    public class Brush
    {
        public int ZIndex { get; set; } = 0;
        public Paint Paint { get; set; }
        public string TextField { get; set; }
        public string Text { get; set; }
        public string GlyphsDirectory { get; set; } = null;
        public Layer _layer { get; set; }
    }


    public enum SymbolPlacement
    {
        Point,
        Line
    }


    public enum TextTransform
    {
        None,
        Uppercase,
        Lowercase
    }


    public class Paint
    {
        public Color BackgroundColor { get; set; }
        public string BackgroundPattern { get; set; }
        public double BackgroundOpacity { get; set; } = 1;

        public Color FillColor { get; set; }
        public string FillPattern { get; set; }
        public Point FillTranslate { get; set; } = new Point();
        public double FillOpacity { get; set; } = 1;

        public Color LineColor { get; set; }
        public string LinePattern { get; set; }
        public Point LineTranslate { get; set; } = new Point();
        public PenLineCap LineCap { get; set; } = PenLineCap.Flat;
        public double LineWidth { get; set; } = 1;
        public double LineOffset { get; set; } = 0;
        public double LineBlur { get; set; } = 0;
        public double[] LineDashArray { get; set; } = new double[0];
        public double LineOpacity { get; set; } = 1;

        public SymbolPlacement SymbolPlacement { get; set; } = SymbolPlacement.Point;
        public double IconScale { get; set; } = 1;
        public string IconImage { get; set; }
        public double IconRotate { get; set; } = 0;
        public Point IconOffset { get; set; } = new Point();
        public double IconOpacity { get; set; } = 1;

        public Color TextColor { get; set; }
        public string[] TextFont { get; set; } = new string[] { "Open Sans Regular", "Arial Unicode MS Regular" };
        public double TextSize { get; set; } = 16;
        public double TextMaxWidth { get; set; } = 10;
        public TextAlignment TextJustify { get; set; } = TextAlignment.Center;
        public double TextRotate { get; set; } = 0;
        public Point TextOffset { get; set; } = new Point();
        public Color TextStrokeColor { get; set; }
        public double TextStrokeWidth { get; set; } = 0;
        public double TextStrokeBlur { get; set; } = 0;
        public bool TextOptional { get; set; } = false;
        public TextTransform TextTransform { get; set; } = TextTransform.None;
        public double TextOpacity { get; set; } = 1;

        public bool Visibility { get; set; } = true; // visibility
    }


    //class ComparableColor:IComparable
    //{
    //    private long numericColor;

    //    public ComparableColor(string encodedColor)
    //    {

    //    }

    //    public int CompareTo(object obj)
    //    {
    //        if(obj.GetType() != typeof(ComparableColor))
    //        {
    //            return -1;
    //        }

    //        return numericColor.CompareTo((ComparableColor)obj);
    //    }
    //}


    public class Layer
    {
        public int Index { get; set; } = -1;
        public string ID { get; set; } = "";
        public string Type { get; set; } = "";
        public string SourceName { get; set; } = "";
        public Source Source { get; set; } = null;
        public string SourceLayer { get; set; } = "";
        public System.Collections.Generic.Dictionary<string, object> Paint { get; set; } =
            new System.Collections.Generic.Dictionary<string, object>();

        public System.Collections.Generic.Dictionary<string, object> Layout { get; set; } =
            new System.Collections.Generic.Dictionary<string, object>();

        public object[] Filter { get; set; } = new object[0];
        public double? MinZoom { get; set; } = null;
        public double? MaxZoom { get; set; } = null;
    }


    public class Source
    {
        public string URL { get; set; } = "";
        public string Type { get; set; } = "";
        public string Name { get; set; } = "";
        public Sources.ITileSource Provider { get; set; } = null;
    }


    public class Style
    {
        public readonly string Hash = "";
        public System.Collections.Generic.List<Layer> Layers =
            new System.Collections.Generic.List<Layer>();

        public System.Collections.Generic.Dictionary<string, Source> Sources =
            new System.Collections.Generic.Dictionary<string, Source>();

        // double screenScale = 0.2;// = 0.3;
        // double emToPx = 16;

        System.Collections.Concurrent.ConcurrentDictionary<string, Brush[]> brushesCache =
            new System.Collections.Concurrent.ConcurrentDictionary<string, Brush[]>();

        public string FontDirectory { get; set; } = null;


        public Style(string path, double scale = 1)
        {
            string json = System.IO.File.ReadAllText(path);
            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(json);
            

            Newtonsoft.Json.Linq.JProperty srcs = jObject.Property("sources");
            if (srcs != null && srcs.Type == Newtonsoft.Json.Linq.JTokenType.Property && srcs.Value.Type == Newtonsoft.Json.Linq.JTokenType.Object)
            {
                foreach (Newtonsoft.Json.Linq.JProperty jSource in ((Newtonsoft.Json.Linq.JObject)srcs.Value).Properties())
                {
                    Source source = new Source();

                    System.Collections.Generic.IDictionary<string, Newtonsoft.Json.Linq.JToken> sourceDict =
                        jSource.Value as Newtonsoft.Json.Linq.JObject;

                    source.Name = jSource.Name;

                    if (sourceDict.ContainsKey("url"))
                    {
                        source.URL = PlainifyJson(sourceDict["url"]) as string;
                    }

                    if (sourceDict.ContainsKey("type"))
                    {
                        source.Type = PlainifyJson(sourceDict["type"]) as string;
                    }

                    Sources[jSource.Name] = source;
                } // Next jSource 

            } // End if (srcs != null && srcs.Type == JTokenType.Property && srcs.Value.Type == JTokenType.Object) 


            Newtonsoft.Json.Linq.JProperty layers = jObject.Property("layers");

            if (layers != null && layers.Value != null && layers.Value.Type == Newtonsoft.Json.Linq.JTokenType.Array)
            {
                int i = 0;
                foreach (Newtonsoft.Json.Linq.JObject jLayer in layers.Value)
                {
                    Layer layer = new Layer();
                    layer.Index = i;

                    System.Collections.Generic.IDictionary<string, Newtonsoft.Json.Linq.JToken> layerDict = jLayer;


                    if (layerDict.ContainsKey("minzoom"))
                    {
                        layer.MinZoom = System.Convert.ToDouble(PlainifyJson(layerDict["minzoom"]));
                    }

                    if (layerDict.ContainsKey("maxzoom"))
                    {
                        layer.MaxZoom = System.Convert.ToDouble(PlainifyJson(layerDict["maxzoom"]));
                    }

                    if (layerDict.ContainsKey("id"))
                    {
                        layer.ID = PlainifyJson(layerDict["id"]) as string;
                    }

                    if (layerDict.ContainsKey("type"))
                    {
                        layer.Type = PlainifyJson(layerDict["type"]) as string;
                    }

                    if (layerDict.ContainsKey("source"))
                    {
                        layer.SourceName = PlainifyJson(layerDict["source"]) as string;
                        layer.Source = Sources[layer.SourceName];
                    }

                    if (layerDict.ContainsKey("source-layer"))
                    {
                        layer.SourceLayer = PlainifyJson(layerDict["source-layer"]) as string;
                    }

                    if (layerDict.ContainsKey("paint"))
                    {
                        layer.Paint = PlainifyJson(layerDict["paint"]) as System.Collections.Generic.Dictionary<string, object>;
                    }

                    if (layerDict.ContainsKey("layout"))
                    {
                        layer.Layout = PlainifyJson(layerDict["layout"]) as System.Collections.Generic.Dictionary<string, object>;
                    }

                    if (layerDict.ContainsKey("filter"))
                    {
                        Newtonsoft.Json.Linq.JArray filterArray = layerDict["filter"] as Newtonsoft.Json.Linq.JArray;
                        layer.Filter = PlainifyJson(filterArray) as object[];
                    }

                    Layers.Add(layer);

                    i++;
                } // Next jLayer 

            } // End if (layers != null && layers.Value != null && layers.Value.Type == JTokenType.Array) 

            Hash = Utils.Sha256(json);
        } // End Function Style 


        public void SetSourceProvider(int index, Sources.ITileSource provider)
        {
            int i = 0;
            foreach (System.Collections.Generic.KeyValuePair<string, Source> pair in Sources)
            {
                if (index == i)
                {
                    pair.Value.Provider = provider;
                    return;
                }
                i++;
            } // Next pair 
        } // End Function SetSourceProvider 


        public void SetSourceProvider(string name, Sources.ITileSource provider)
        {
            Sources[name].Provider = provider;
        }


        protected object PlainifyJson(Newtonsoft.Json.Linq.JToken token)
        {
            if (token.Type == Newtonsoft.Json.Linq.JTokenType.Object)
            {
                System.Collections.Generic.IDictionary<string, Newtonsoft.Json.Linq.JToken> dict =
                    token as Newtonsoft.Json.Linq.JObject;

                return dict.Select(pair => new System.Collections.Generic.KeyValuePair<string, object>(pair.Key, PlainifyJson(pair.Value)))
                        .ToDictionary(key => key.Key, value => value.Value);
            }
            else if (token.Type == Newtonsoft.Json.Linq.JTokenType.Array)
            {
                Newtonsoft.Json.Linq.JArray array = token as Newtonsoft.Json.Linq.JArray;
                return array.Select(item => PlainifyJson(item)).ToArray();
            }
            else
            {
                return token.ToObject<object>();
            }

            return null;
        } // End Function PlainifyJson 


        public Brush[] GetStyleByType(string type, double zoom, double scale = 1)
        {
            System.Collections.Generic.List<Brush> results = new System.Collections.Generic.List<Brush>();

            int i = 0;
            foreach (Layer layer in Layers)
            {
                if (layer.Type == type)
                {
                    System.Collections.Generic.Dictionary<string, object> attributes = new System.Collections.Generic.Dictionary<string, object>();
                    attributes["$type"] = "";
                    attributes["$id"] = "";
                    attributes["$zoom"] = zoom;

                    results.Add(ParseStyle(layer, scale, attributes));
                }
                i++;
            } // Next layer 

            return results.ToArray();
        } // End Function GetStyleByType 


        public Color GetBackgroundColor(double zoom)
        {
            Brush[] brushes = GetStyleByType("background", zoom, 1);

            foreach (Brush brush in brushes)
            {
                Color newColor = Color.FromArgb((byte)System.Math.Max(0, System.Math.Min(255, brush.Paint.BackgroundOpacity * brush.Paint.BackgroundColor.A)), brush.Paint.BackgroundColor.R, brush.Paint.BackgroundColor.G, brush.Paint.BackgroundColor.B);
                return newColor;
            }

            return Colors.White;
        } // End Function GetBackgroundColor 


        //public Brush[] GetBrushesCached(
        //    double zoom, 
        //    double scale, 
        //    string type, 
        //    string id, 
        //    System.Collections.Generic.Dictionary<string, object> attributes)
        //{
        //    // check if the brush is cached or not
        //    // uses a cache key and stores brushes
        //    // 200ms saved on 3x3 512x512px tiles
        //    System.Text.StringBuilder builder = new System.Text.StringBuilder();

        //    builder.Append(zoom);
        //    builder.Append(',');
        //    builder.Append(scale);
        //    builder.Append(',');
        //    builder.Append(type);
        //    builder.Append(',');
        //    builder.Append(id);
        //    builder.Append(',');

        //    foreach (System.Collections.Generic.KeyValuePair<string, object> attribute in attributes)
        //    {
        //        builder.Append(attribute.Key);
        //        builder.Append(';');
        //        builder.Append(attribute.Value);
        //        builder.Append('%');
        //    } // Next attribute 

        //    string key = builder.ToString();

        //    if (brushesCache.ContainsKey(key))
        //    {
        //        return brushesCache[key];
        //    } // End if (brushesCache.ContainsKey(key)) 

        //    Brush[] brushes = GetBrushes(zoom, scale, type, id, attributes);
        //    brushesCache[key] = brushes;

        //    return brushes;
        //} // End Function GetBrushesCached 


        //public Brush[] GetBrushes(
        //    double zoom, 
        //    double scale, 
        //    string type, 
        //    string id, 
        //    System.Collections.Generic.Dictionary<string, object> attributes)
        //{
        //    attributes["$type"] = type;
        //    attributes["$id"] = id;
        //    attributes["$zoom"] = zoom;

        //    Layer[] layers = FindLayers(zoom, attributes);

        //    Brush[] result = new Brush[layers.Count()];

        //    int i = 0;
        //    foreach (Layer layer in layers)
        //    {
        //        result[i] = ParseStyle(layer, scale, attributes);
        //        i++;
        //    } // Next layer 

        //    return result;
        //} // End Function GetBrushesCached 


        public Brush ParseStyle(Layer layer, double scale, System.Collections.Generic.Dictionary<string, object> attributes)
        {
            System.Collections.Generic.Dictionary<string, object> paintData = layer.Paint;
            System.Collections.Generic.Dictionary<string, object> layoutData = layer.Layout;
            int index = layer.Index;

            Brush brush = new Brush();
            brush.ZIndex = index;
            brush._layer = layer;
            brush.GlyphsDirectory = this.FontDirectory;

            Paint paint = new Paint();
            brush.Paint = paint;

            if (layer.ID == "country_label")
            {

            }

            if (paintData != null)
            {
                // --

                if (paintData.ContainsKey("fill-color"))
                {
                    paint.FillColor = ParseColor(GetValue(paintData["fill-color"], attributes));
                }

                if (paintData.ContainsKey("background-color"))
                {
                    paint.BackgroundColor = ParseColor(GetValue(paintData["background-color"], attributes));
                }

                if (paintData.ContainsKey("text-color"))
                {
                    paint.TextColor = ParseColor(GetValue(paintData["text-color"], attributes));
                }

                if (paintData.ContainsKey("line-color"))
                {
                    paint.LineColor = ParseColor(GetValue(paintData["line-color"], attributes));
                }

                // --

                if (paintData.ContainsKey("line-pattern"))
                {
                    paint.LinePattern = (string)GetValue(paintData["line-pattern"], attributes);
                }

                if (paintData.ContainsKey("background-pattern"))
                {
                    paint.BackgroundPattern = (string)GetValue(paintData["background-pattern"], attributes);
                }

                if (paintData.ContainsKey("fill-pattern"))
                {
                    paint.FillPattern = (string)GetValue(paintData["fill-pattern"], attributes);
                }

                // --

                if (paintData.ContainsKey("text-opacity"))
                {
                    paint.TextOpacity = System.Convert.ToDouble(GetValue(paintData["text-opacity"], attributes));
                }

                if (paintData.ContainsKey("icon-opacity"))
                {
                    paint.IconOpacity = System.Convert.ToDouble(GetValue(paintData["icon-opacity"], attributes));
                }

                if (paintData.ContainsKey("line-opacity"))
                {
                    paint.LineOpacity = System.Convert.ToDouble(GetValue(paintData["line-opacity"], attributes));
                }

                if (paintData.ContainsKey("fill-opacity"))
                {
                    paint.FillOpacity = System.Convert.ToDouble(GetValue(paintData["fill-opacity"], attributes));
                }

                if (paintData.ContainsKey("background-opacity"))
                {
                    paint.BackgroundOpacity = System.Convert.ToDouble(GetValue(paintData["background-opacity"], attributes));
                }

                // --

                if (paintData.ContainsKey("line-width"))
                {
                    paint.LineWidth = System.Convert.ToDouble(GetValue(paintData["line-width"], attributes)) * scale; // * screenScale;
                }

                if (paintData.ContainsKey("line-offset"))
                {
                    paint.LineOffset = System.Convert.ToDouble(GetValue(paintData["line-offset"], attributes)) * scale;// * screenScale;
                }

                if (paintData.ContainsKey("line-dasharray"))
                {
                    object[] array = (GetValue(paintData["line-dasharray"], attributes) as object[]);
                    paint.LineDashArray = array.Select(item => System.Convert.ToDouble(item) * scale).ToArray();
                }

                // --

                if (paintData.ContainsKey("text-halo-color"))
                {
                    paint.TextStrokeColor = ParseColor(GetValue(paintData["text-halo-color"], attributes));
                }

                if (paintData.ContainsKey("text-halo-width"))
                {
                    paint.TextStrokeWidth = System.Convert.ToDouble(GetValue(paintData["text-halo-width"], attributes)) * scale;
                }

                if (paintData.ContainsKey("text-halo-blur"))
                {
                    paint.TextStrokeBlur = System.Convert.ToDouble(GetValue(paintData["text-halo-blur"], attributes)) * scale;
                }

                // --

                //Console.WriteLine("paint");
                //Console.WriteLine(paintData.ToString());
                //foreach (string keyName in ((JObject)paintData).Properties().Select(p => p.Name))
                //{
                //    Console.WriteLine(keyName);
                //}

            } // End if (paintData != null) 

            if (layoutData != null)
            {
                if (layoutData.ContainsKey("line-cap"))
                {
                    string value = (string)GetValue(layoutData["line-cap"], attributes);
                    if (value == "butt")
                    {
                        paint.LineCap = PenLineCap.Flat;
                    }
                    else if (value == "round")
                    {
                        paint.LineCap = PenLineCap.Round;
                    }
                    else if (value == "square")
                    {
                        paint.LineCap = PenLineCap.Square;
                    }
                } // End if (layoutData.ContainsKey("line-cap")) 

                if (layoutData.ContainsKey("visibility"))
                {
                    paint.Visibility = ((string)GetValue(layoutData["visibility"], attributes)) == "visible";
                }

                if (layoutData.ContainsKey("text-field"))
                {
                    brush.TextField = (string)GetValue(layoutData["text-field"], attributes);

                    // TODO check performance implications of Regex.Replace
                    brush.Text = System.Text.RegularExpressions.Regex.Replace(brush.TextField, @"\{([A-Za-z0-9\-\:_]+)\}",
                        delegate (System.Text.RegularExpressions.Match m)
                        {
                            string key = StripBraces(m.Value);
                            if (attributes.ContainsKey(key))
                            {
                                return attributes[key].ToString();
                            }

                            return "";
                        }
                    ).Trim();
                }

                if (layoutData.ContainsKey("text-font"))
                {
                    paint.TextFont = ((object[])GetValue(layoutData["text-font"], attributes)).Select(item => (string)item).ToArray();
                }

                if (layoutData.ContainsKey("text-size"))
                {
                    paint.TextSize = System.Convert.ToDouble(GetValue(layoutData["text-size"], attributes)) * scale;
                }

                if (layoutData.ContainsKey("text-max-width"))
                {
                    paint.TextMaxWidth = System.Convert.ToDouble(GetValue(layoutData["text-max-width"], attributes)) * scale;// * screenScale;
                }

                if (layoutData.ContainsKey("text-offset"))
                {
                    object[] value = (object[])GetValue(layoutData["text-offset"], attributes);
                    paint.TextOffset = new Point(System.Convert.ToDouble(value[0]) * scale, System.Convert.ToDouble(value[1]) * scale);
                }

                if (layoutData.ContainsKey("text-optional"))
                {
                    paint.TextOptional = (bool)(GetValue(layoutData["text-optional"], attributes));
                }

                if (layoutData.ContainsKey("text-transform"))
                {
                    string value = (string)GetValue(layoutData["text-transform"], attributes);
                    if (value == "none")
                    {
                        paint.TextTransform = TextTransform.None;
                    }
                    else if (value == "uppercase")
                    {
                        paint.TextTransform = TextTransform.Uppercase;
                    }
                    else if (value == "lowercase")
                    {
                        paint.TextTransform = TextTransform.Lowercase;
                    }
                }

                if (layoutData.ContainsKey("icon-size"))
                {
                    paint.IconScale = System.Convert.ToDouble(GetValue(layoutData["icon-size"], attributes)) * scale;
                }

                if (layoutData.ContainsKey("icon-image"))
                {
                    paint.IconImage = (string)GetValue(layoutData["icon-image"], attributes);
                }

                //Console.WriteLine("layout");
                //Console.WriteLine(layoutData.ToString());
            } // End if (layoutData != null) 

            return brush;
        } // End Function ParseStyle 


        private string StripBraces(string s)
        {
            string ret = null;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            int len = s.Length;
            for (int i = 0; i < len; ++i)
            {
                char c = s[i];
                switch (c)
                {
                    case '{':
                    case '}':
                        continue;
                    default:
                        sb.Append(c);
                        break;
                } // End Switch 

            } // Next i 

            ret = sb.ToString();
            sb.Clear();
            sb = null;

            return ret;
        } // End Function StripBraces 


        private Color ParseColor(object iColor)
        {
            if (iColor.GetType() == typeof(Color))
            {
                return (Color)iColor;
            }

            if (iColor.GetType() != typeof(string))
            {

            }

            string colorString = (string)iColor;

            if (colorString[0] == '#')
            {
                return (Color)ColorConverter.ConvertFromString(colorString);
            }

            if (colorString.StartsWith("hsl("))
            {
                string[] segments = colorString.Replace('%', '\0').Split(',', '(', ')');
                double h = double.Parse(segments[1], System.Globalization.CultureInfo.InvariantCulture);
                double s = double.Parse(segments[2], System.Globalization.CultureInfo.InvariantCulture);
                double l = double.Parse(segments[3], System.Globalization.CultureInfo.InvariantCulture);

                ColorMine.ColorSpaces.IRgb color = (new ColorMine.ColorSpaces.Hsl()
                {
                    H = h,
                    S = s,
                    L = l,
                }).ToRgb();

                return Color.FromRgb((byte)color.R, (byte)color.G, (byte)color.B);
            } // End if (colorString.StartsWith("hsl(")) 

            if (colorString.StartsWith("hsla("))
            {
                string[] segments = colorString.Replace('%', '\0').Split(',', '(', ')');
                double h = double.Parse(segments[1], System.Globalization.CultureInfo.InvariantCulture);
                double s = double.Parse(segments[2], System.Globalization.CultureInfo.InvariantCulture);
                double l = double.Parse(segments[3], System.Globalization.CultureInfo.InvariantCulture);
                double a = double.Parse(segments[4], System.Globalization.CultureInfo.InvariantCulture) * 255;

                ColorMine.ColorSpaces.IRgb color = (new ColorMine.ColorSpaces.Hsl()
                {
                    H = h,
                    S = s,
                    L = l,
                }).ToRgb();

                return Color.FromArgb((byte)(a), (byte)color.R, (byte)color.G, (byte)color.B);
            } // End if (colorString.StartsWith("hsla(")) 


            if (colorString.StartsWith("rgba("))
            {
                string[] segments = colorString.Replace('%', '\0').Split(',', '(', ')');
                double r = double.Parse(segments[1], System.Globalization.CultureInfo.InvariantCulture);
                double g = double.Parse(segments[2], System.Globalization.CultureInfo.InvariantCulture);
                double b = double.Parse(segments[3], System.Globalization.CultureInfo.InvariantCulture);
                double a = double.Parse(segments[4], System.Globalization.CultureInfo.InvariantCulture) * 255;

                return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
            } // End if (colorString.StartsWith("rgba(")) 

            if (colorString.StartsWith("rgb("))
            {
                string[] segments = colorString.Replace('%', '\0').Split(',', '(', ')');
                double r = double.Parse(segments[1], System.Globalization.CultureInfo.InvariantCulture);
                double g = double.Parse(segments[2], System.Globalization.CultureInfo.InvariantCulture);
                double b = double.Parse(segments[3], System.Globalization.CultureInfo.InvariantCulture);

                return Color.FromRgb((byte)r, (byte)g, (byte)b);
            } // End if (colorString.StartsWith("rgb(")) 

            try
            {
                return ColorConverter.ConvertFromString(colorString);
            }
            catch (System.Exception e)
            {
                throw new System.NotImplementedException("Not implemented color format: " + colorString);
            }

            // return Colors.Violet;
        } // End Function ParseColor 


        public bool ValidateLayer(Layer layer, double zoom, System.Collections.Generic.Dictionary<string, object> attributes)
        {

            if (layer.MinZoom != null)
            {
                if (zoom < layer.MinZoom.Value)
                {
                    return false;
                }
            }

            if (layer.MaxZoom != null)
            {
                if (zoom > layer.MaxZoom.Value)
                {
                    return false;
                }
            }

            if (layer.Filter.Length > 0)
            {
                if (!ValidateUsingFilter(layer.Filter, attributes))
                {
                    return false;
                }
            } // End if (layer.Filter.Length > 0) 

            return true;
        } // End Function ValidateLayer 


        private Layer[] FindLayers(double zoom, string layerName, System.Collections.Generic.Dictionary<string, object> attributes)
        {
            ////Console.WriteLine(layerName);
            System.Collections.Generic.List<Layer> result = new System.Collections.Generic.List<Layer>();

            foreach (Layer layer in Layers)
            {
                //if (attributes.ContainsKey("class"))
                //{
                //    if (id == "highway-trunk" && (string)attributes["class"] == "primary")
                //    {

                //    }
                //}

                if (layer.SourceLayer == layerName)
                {
                    bool valid = true;

                    if (layer.Filter.Length > 0)
                    {
                        if (!ValidateUsingFilter(layer.Filter, attributes))
                        {
                            valid = false;
                        }
                    }

                    if (layer.MinZoom != null)
                    {
                        if (zoom < layer.MinZoom.Value)
                        {
                            valid = false;
                        }
                    }

                    if (layer.MaxZoom != null)
                    {
                        if (zoom > layer.MaxZoom.Value)
                        {
                            valid = false;
                        }
                    }

                    if (valid)
                    {
                        //return layer;
                        result.Add(layer);
                    }

                } // End if (layer.SourceLayer == layerName) 

            } // Next layer 

            return result.ToArray();
        } // End Function FindLayers 


        private bool ValidateUsingFilter(object[] filterArray, System.Collections.Generic.Dictionary<string, object> attributes)
        {
            if (filterArray.Length == 0)
            {
            }
            string operation = filterArray[0] as string;
            bool result;

            if (operation == "all")
            {
                foreach (object[] subFilter in filterArray.Skip(1))
                {
                    if (!ValidateUsingFilter(subFilter, attributes))
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (operation == "any")
            {
                foreach (object[] subFilter in filterArray.Skip(1))
                {
                    if (ValidateUsingFilter(subFilter, attributes))
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (operation == "none")
            {
                result = false;
                foreach (object[] subFilter in filterArray.Skip(1))
                {
                    if (ValidateUsingFilter(subFilter, attributes))
                    {
                        result = true;
                    }
                }
                return !result;
            }

            switch (operation)
            {
                case "==":
                case "!=":
                case ">":
                case ">=":
                case "<":
                case "<=":

                    string key = (string)filterArray[1];

                    if (operation == "==")
                    {
                        if (!attributes.ContainsKey(key))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // special case, comparing inequality with non existent attribute
                        if (!attributes.ContainsKey(key))
                        {
                            return true;
                        }
                    }

                    if (!(attributes[key] is System.IComparable))
                    {
                        throw new System.NotImplementedException("Comparing colors probably");
                        return false;
                    }

                    System.IComparable valueA = (System.IComparable)attributes[key];
                    object valueB = GetValue(filterArray[2], attributes);

                    if (IsNumber(valueA) && IsNumber(valueB))
                    {
                        valueA = System.Convert.ToDouble(valueA);
                        valueB = System.Convert.ToDouble(valueB);
                    }

                    if (key is string)
                    {
                        if (key == "capital")
                        {

                        }
                    } // End if (key is string) 

                    if (valueA.GetType() != valueB.GetType())
                    {
                        return false;
                    } // End if (valueA.GetType() != valueB.GetType()) 

                    int comparison = valueA.CompareTo(valueB);

                    if (operation == "==")
                    {
                        return comparison == 0;
                    }
                    else if (operation == "!=")
                    {
                        return comparison != 0;
                    }
                    else if (operation == ">")
                    {
                        return comparison > 0;
                    }
                    else if (operation == "<")
                    {
                        return comparison < 0;
                    }
                    else if (operation == ">=")
                    {
                        return comparison >= 0;
                    }
                    else if (operation == "<=")
                    {
                        return comparison <= 0;
                    }

                    break;
            } // End switch (operation) 

            if (operation == "has")
            {
                return attributes.ContainsKey(filterArray[1] as string);
            }
            else if (operation == "!has")
            {
                return !attributes.ContainsKey(filterArray[1] as string);
            }


            if (operation == "in")
            {
                string key = filterArray[1] as string;
                if (!attributes.ContainsKey(key))
                {
                    return false;
                }

                object value = attributes[key];

                foreach (object item in filterArray.Skip(2))
                {
                    if (GetValue(item, attributes).Equals(value))
                    {
                        return true;
                    } // End if (GetValue(item, attributes).Equals(value)) 

                } // Next item 

                return false;
            } // End if (operation == "in") 
            else if (operation == "!in")
            {
                string key = filterArray[1] as string;
                if (!attributes.ContainsKey(key))
                {
                    return true;
                }

                object value = attributes[key];

                foreach (object item in filterArray.Skip(2))
                {
                    if (GetValue(item, attributes).Equals(value))
                    {
                        return false;
                    } // End if (GetValue(item, attributes).Equals(value)) 

                } // Next item 

                return true;
            } // End else if (operation == "!in")

            return false;
        } // End Function ValidateUsingFilter


        protected object GetValue(object token, System.Collections.Generic.Dictionary<string, object> attributes = null)
        {

            if (token is string && attributes != null)
            {
                string value = token as string;

                if (value.Length == 0)
                {
                    return "";
                }

                if (value[0] == '$')
                {
                    return GetValue(attributes[value]);
                }

            } // End if (token is string && attributes != null) 

            if (token.GetType().IsArray)
            {
                object[] array = token as object[];
                System.Collections.Generic.List<object> result = new System.Collections.Generic.List<object>();

                foreach (object item in array)
                {
                    object obj = GetValue(item, attributes);
                    result.Add(obj);
                } // Next item 

                return result.ToArray();
            }
            else if (token is System.Collections.Generic.Dictionary<string, object>)
            {
                System.Collections.Generic.Dictionary<string, object> dict = token as System.Collections.Generic.Dictionary<string, object>;
                if (dict.ContainsKey("stops"))
                {
                    object[] stops = dict["stops"] as object[];
                    // if it has stops, it's interpolation domain now :P
                    // System.Collections.Generic.List<System.Tuple<double, JToken>> pointStops = stops.Select(item => new System.Tuple<double, JToken>((item as JArray)[0].Value<double>(), (item as JArray)[1])).ToList();
                    System.Collections.Generic.List<System.Tuple<double, object>> pointStops = stops.Select(item => new System.Tuple<double, object>(System.Convert.ToDouble((item as object[])[0]), (item as object[])[1])).ToList();

                    double zoom = (double)attributes["$zoom"];
                    double minZoom = pointStops.First().Item1;
                    double maxZoom = pointStops.Last().Item1;
                    double power = 1;

                    if (minZoom == 5 && maxZoom == 10)
                    {

                    }

                    double zoomA = minZoom;
                    double zoomB = maxZoom;
                    int zoomAIndex = 0;

                    int zoomBIndex = pointStops.Count - 1;

                    // get min max zoom bounds from array
                    if (zoom <= minZoom)
                    {
                        //zoomA = minZoom;
                        //zoomB = pointStops[1].Item1;
                        return pointStops.First().Item2;
                    }
                    else if (zoom >= maxZoom)
                    {
                        //zoomA = pointStops[pointStops.Count - 2].Item1;
                        //zoomB = maxZoom;
                        return pointStops.Last().Item2;
                    }
                    else
                    {
                        // checking for consecutive values
                        for (int i = 1; i < pointStops.Count; i++)
                        {
                            double previousZoom = pointStops[i - 1].Item1;
                            double thisZoom = pointStops[i].Item1;

                            if (zoom >= previousZoom && zoom <= thisZoom)
                            {
                                zoomA = previousZoom;
                                zoomB = thisZoom;

                                zoomAIndex = i - 1;
                                zoomBIndex = i;
                                break;
                            } // End if (zoom >= previousZoom && zoom <= thisZoom) 

                        } // Next i 
                    } // End Else 


                    if (dict.ContainsKey("base"))
                    {
                        power = System.Convert.ToDouble(GetValue(dict["base"], attributes));
                    } // End if (dict.ContainsKey("base")) 

                    // object referenceElement = (stops[0] as object[])[1];

                    return InterpolateValues(pointStops[zoomAIndex].Item2, pointStops[zoomBIndex].Item2, zoomA, zoomB, zoom, power, false);
                } // End if (dict.ContainsKey("stops")) 

            } // End else if (token is System.Collections.Generic.Dictionary<string, object>) 


            //if (token is string)
            //{
            //    return token as string;
            //}
            //else if (token is bool)
            //{
            //    return (bool)token;
            //}
            //else if (token is float)
            //{
            //    return token as float;
            //}
            //else if (token.Type == JTokenType.Integer)
            //{
            //    return token.Value<int>();
            //}
            //else if (token.Type == JTokenType.None || token.Type == JTokenType.Null)
            //{
            //    return null;
            //}


            return token;
        } // End Function GetValue 


        protected bool IsNumber(object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        } // End Function IsNumber 


        private object InterpolateValues(object startValue, object endValue, double zoomA, double zoomB, double zoom, double power, bool clamp = false)
        {
            if (startValue is string)
            {
                // TODO implement color mappings
                // Color minValue = ParseColor(startValue.Value<string>());
                // Color maxValue = ParseColor(endValue.Value<string>());


                // double newR = Utils.ConvertRange(zoom, zoomA, zoomB, minValue.ScR, maxValue.ScR, power, false);
                // double newG = Utils.ConvertRange(zoom, zoomA, zoomB, minValue.ScG, maxValue.ScG, power, false);
                // double newB = Utils.ConvertRange(zoom, zoomA, zoomB, minValue.ScB, maxValue.ScB, power, false);
                // double newA = Utils.ConvertRange(zoom, zoomA, zoomB, minValue.ScA, maxValue.ScA, power, false);

                // return Color.FromScRgb((float)newA, (float)newR, (float)newG, (float)newB);

                string minValue = startValue as string;
                string maxValue = endValue as string;

                if (System.Math.Abs(zoomA - zoom) <= System.Math.Abs(zoomB - zoom))
                {
                    return minValue;
                }
                else
                {
                    return maxValue;
                }

            }
            else if (startValue.GetType().IsArray)
            {
                System.Collections.Generic.List<object> result =
                    new System.Collections.Generic.List<object>();

                object[] startArray = startValue as object[];
                object[] endArray = endValue as object[];

                for (int i = 0; i < startArray.Length; i++)
                {
                    object minValue = startArray[i];
                    object maxValue = endArray[i];

                    object value = InterpolateValues(minValue, maxValue, zoomA, zoomB, zoom, power, clamp);

                    result.Add(value);
                }

                return result.ToArray();
            }
            else if (IsNumber(startValue))
            {
                double minValue = System.Convert.ToDouble(startValue);
                double maxValue = System.Convert.ToDouble(endValue);

                return InterpolateRange(zoom, zoomA, zoomB, minValue, maxValue, power, clamp);
            }
            else
            {
                throw new System.NotImplementedException("Unimplemented interpolation");
            }

        } // End Function InterpolateValues 


        private double InterpolateRange(
              double oldValue
            , double oldMin
            , double oldMax
            , double newMin
            , double newMax
            , double power
            , bool clamp = false)
        {
            double difference = oldMax - oldMin;
            double progress = oldValue - oldMin;

            double normalized = 0;

            if (difference == 0)
            {
                normalized = 0;
            }
            else if (power == 1)
            {
                normalized = progress / difference;
            }
            else
            {
                normalized = (System.Math.Pow(power, progress) - 1f) / (System.Math.Pow(power, difference) - 1f);
            }

            double result = (normalized * (newMax - newMin)) + newMin;


            return result;
        } // End Function InterpolateRange 


    } // End Class Style 


} // End Namespace 
