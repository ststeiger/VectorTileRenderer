
namespace VectorTileRenderer
{

    using System.Collections.Generic;
    // using System.Linq;
    using ClipperLib;
    using SkiaSharp;



    public class SkiaCanvas
        : ICanvas, System.IDisposable
    {
        protected int width;
        protected int height;

        protected SKBitmap bitmap;

        // SKSurface surface;
        protected SKCanvas canvas;

        private GRContext grContext;
        private GRBackendRenderTargetDesc renderTarget;

        public bool ClipOverflow { get; set; } = false;
        private Rect clipRectangle;
        protected List<IntPoint> clipRectanglePath;

        protected Dictionary<string, SKTypeface> fontPairs = new Dictionary<string, SKTypeface>();
        protected List<Rect> textRectangles = new List<Rect>();
        private bool disposedValue;

        public void StartDrawing(double width, double height)
        {
            this.width = (int)width;
            this.height = (int)height;

            SKImageInfo info = new SKImageInfo(this.width, this.height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            this.bitmap = new SKBitmap(info);


            // GRGlInterface glInterface = GRGlInterface.CreateNativeGlInterface();
            // grContext = GRContext.Create(GRBackend.OpenGL, glInterface);

            // renderTarget = SkiaGL.CreateRenderTarget();
            // renderTarget.Width = this.width;
            // renderTarget.Height = this.height;


            // surface = SKSurface.Create(info, bitmap.BackBuffer, bitmap.BackBufferStride);
            // surface = SKSurface.Create(grContext, renderTarget);
            // canvas = surface.Canvas;

            canvas = new SKCanvas(this.bitmap);


            double padding = -5;
            clipRectangle = new Rect(padding, padding, this.width - padding * 2, this.height - padding * 2);

            clipRectanglePath = new List<IntPoint>();
            clipRectanglePath.Add(new IntPoint((int)clipRectangle.Top, (int)clipRectangle.Left));
            clipRectanglePath.Add(new IntPoint((int)clipRectangle.Top, (int)clipRectangle.Right));
            clipRectanglePath.Add(new IntPoint((int)clipRectangle.Bottom, (int)clipRectangle.Right));
            clipRectanglePath.Add(new IntPoint((int)clipRectangle.Bottom, (int)clipRectangle.Left));
        } // End Sub StartDrawing 


        public void DrawBackground(Brush style)
        {
            canvas.Clear(new SKColor(style.Paint.BackgroundColor.R, style.Paint.BackgroundColor.G, style.Paint.BackgroundColor.B, style.Paint.BackgroundColor.A));
        } // End Sub DrawBackground 


        protected SKStrokeCap ConvertCap(PenLineCap cap)
        {
            if (cap == PenLineCap.Flat)
            {
                return SKStrokeCap.Butt;
            }
            else if (cap == PenLineCap.Round)
            {
                return SKStrokeCap.Round;
            }

            return SKStrokeCap.Square;
        } // End Function ConvertCap 


        //protected double GetAngle(double x1, double y1, double x2, double y2)
        //{
        //    double degrees;

        //    if (x2 - x1 == 0)
        //    {
        //        if (y2 > y1)
        //            degrees = 90;
        //        else
        //            degrees = 270;
        //    }
        //    else
        //    {
        //        // Calculate angle from offset.
        //        double riseoverrun = (y2 - y1) / (x2 - x1);
        //        double radians = Math.Atan(riseoverrun);
        //        degrees = radians * (180 / Math.PI);

        //        if ((x2 - x1) < 0 || (y2 - y1) < 0)
        //            degrees += 180;
        //        if ((x2 - x1) > 0 && (y2 - y1) < 0)
        //            degrees -= 180;
        //        if (degrees < 0)
        //            degrees += 360;
        //    }
        //    return degrees;
        //} // End Function GetAngle 


        //protected double GetAngleAverage(double a, double b)
        //{
        //    a = a % 360;
        //    b = b % 360;

        //    double sum = a + b;
        //    if (sum > 360 && sum < 540)
        //    {
        //        sum = sum % 180;
        //    }
        //    return sum / 2;
        //} // End Function GetAngleAverage 

        protected double Clamp(double number, double min = 0, double max = 1)
        {
            return System.Math.Max(min, System.Math.Min(max, number));
        } // End Function Clamp 


        protected List<Point> ClipPolygon(List<Point> geometry)
        {
            Clipper c = new Clipper();

            List<IntPoint> polygon = new List<IntPoint>();

            foreach (Point point in geometry)
            {
                polygon.Add(new IntPoint((int)point.X, (int)point.Y));
            } // Next point 

            // c.AddPolygon(polygon, PolyType.ptSubject);
            c.AddPath(polygon, PolyType.ptSubject, true);


            // c.AddPolygon(clipRectanglePath, PolyType.ptClip);
            c.AddPath(clipRectanglePath, PolyType.ptClip, true);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();

            bool success = c.Execute(ClipType.ctIntersection, solution, PolyFillType.pftNonZero, PolyFillType.pftEvenOdd);

            if (success && solution.Count > 0)
            {
                List<Point> result = solution.First().Map(item => new Point(item.X, item.Y)); //.ToList();
                return result;
            } // End if (success && solution.Count > 0) 

            return null;
        } // End Function ClipPolygon 


        protected List<Point> ClipLine(List<Point> geometry)
        {
            return LineClipper.ClipPolyline(geometry, clipRectangle);
        } // End Function ClipLine 


        protected SKPath GetPathFromGeometry(List<Point> geometry)
        {

            SKPath path = new SKPath
            {
                FillType = SKPathFillType.EvenOdd,
            };

            Point firstPoint = geometry[0];

            path.MoveTo((float)firstPoint.X, (float)firstPoint.Y);
            foreach (Point point in geometry.Skip(1))
            {
                SKPoint lastPoint = path.LastPoint;
                path.LineTo((float)point.X, (float)point.Y);
            } // Next point 

            return path;
        } // End Function GetPathFromGeometry 


        public void DrawLineString(List<Point> geometry, Brush style)
        {
            if (ClipOverflow)
            {
                geometry = ClipLine(geometry);
                if (geometry == null)
                {
                    return;
                } // End if (geometry == null) 

            } // End if (ClipOverflow) 

            SKPath path = GetPathFromGeometry(geometry);
            if (path == null)
            {
                return;
            } // End if(path == null) 


            if (style.Paint.LineColor == null)
                return;

            SKPaint fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeCap = ConvertCap(style.Paint.LineCap),
                StrokeWidth = (float)style.Paint.LineWidth,
                Color = new SKColor(style.Paint.LineColor.R, style.Paint.LineColor.G, style.Paint.LineColor.B, (byte)Clamp(style.Paint.LineColor.A * style.Paint.LineOpacity, 0, 255)),
                IsAntialias = true,
            };

            if (style.Paint.LineDashArray.Length > 0)
            {
                SKPathEffect effect = SKPathEffect.CreateDash(style.Paint.LineDashArray.Map(n => (float)n).ToArray(), 0);
                fillPaint.PathEffect = effect;
            } // End if(style.Paint.LineDashArray.Count() > 0) 

            // System.Console.WriteLine(style.Paint.LineWidth);

            canvas.DrawPath(path, fillPaint);
        } // End Sub DrawLineString 


        protected SKTextAlign ConvertAlignment(TextAlignment alignment)
        {
            if (alignment == TextAlignment.Center)
            {
                return SKTextAlign.Center;
            }
            else if (alignment == TextAlignment.Left)
            {
                return SKTextAlign.Left;
            }
            else if (alignment == TextAlignment.Right)
            {
                return SKTextAlign.Right;
            }

            return SKTextAlign.Center;
        } // End Function ConvertAlignment 


        protected SKPaint GetTextStrokePaint(Brush style)
        {
            SKPaint paint = new SKPaint()
            {
                IsStroke = true,
                StrokeWidth = (float)style.Paint.TextStrokeWidth,
                Color = new SKColor(style.Paint.TextStrokeColor.R, style.Paint.TextStrokeColor.G, style.Paint.TextStrokeColor.B, (byte)Clamp(style.Paint.TextStrokeColor.A * style.Paint.TextOpacity, 0, 255)),
                TextSize = (float)style.Paint.TextSize,
                IsAntialias = true,
                TextEncoding = SKTextEncoding.Utf32,
                TextAlign = ConvertAlignment(style.Paint.TextJustify),
                Typeface = GetFont(style.Paint.TextFont, style),
            };

            return paint;
        } // End Function GetTextStrokePaint 


        protected SKPaint GetTextPaint(Brush style)
        {
            SKPaint paint = new SKPaint()
            {
                Color = new SKColor(style.Paint.TextColor.R, style.Paint.TextColor.G, style.Paint.TextColor.B, (byte)Clamp(style.Paint.TextColor.A * style.Paint.TextOpacity, 0, 255)),
                TextSize = (float)style.Paint.TextSize,
                IsAntialias = true,
                TextEncoding = SKTextEncoding.Utf32,
                TextAlign = ConvertAlignment(style.Paint.TextJustify),
                Typeface = GetFont(style.Paint.TextFont, style),
            };

            return paint;
        } // End Function GetTextPaint 


        protected string TransformText(string text, Brush style)
        {
            if (text.Length == 0)
            {
                return "";
            }

            if (style.Paint.TextTransform == TextTransform.Uppercase)
            {
                text = text.ToUpper();
            }
            else if (style.Paint.TextTransform == TextTransform.Lowercase)
            {
                text = text.ToLower();
            }

            SKPaint paint = GetTextPaint(style);
            text = BreakText(text, paint, style);

            // return System.Text.Encoding.UTF32.GetBytes(newText);
            return text;
        } // End Function TransformText 


        protected string BreakText(string input, SKPaint paint, Brush style)
        {
            string restOfText = input;
            string brokenText = "";
            do
            {
                long lineLength = paint.BreakText(restOfText, (float)(style.Paint.TextMaxWidth * style.Paint.TextSize));

                if (lineLength == restOfText.Length)
                {
                    // its the end
                    brokenText += restOfText.Trim();
                    break;
                } // End if(lineLength == restOfText.Length) 

                int lastSpaceIndex = restOfText.LastIndexOf(' ', (int)(lineLength - 1));
                if (lastSpaceIndex == -1 || lastSpaceIndex == 0)
                {
                    // no more spaces, probably ;)
                    brokenText += restOfText.Trim();
                    break;
                } // End if(lastSpaceIndex == -1 || lastSpaceIndex == 0) 

                brokenText += restOfText.Substring(0, (int)lastSpaceIndex).Trim() + "\n";

                restOfText = restOfText.Substring((int)lastSpaceIndex, restOfText.Length - (int)lastSpaceIndex);

            } while (restOfText.Length > 0);

            return brokenText.Trim();
        } // End Function BreakText 


        protected bool TextCollides(Rect rectangle)
        {
            foreach (Rect rect in textRectangles)
            {
                if (rect.IntersectsWith(rectangle))
                {
                    return true;
                } // End if(rect.IntersectsWith(rectangle)) 

            } // Next rect 

            return false;
        } // End Function TextCollides 





        static SKTypeface notoFont;

        public static SKTypeface GetNoto(string glyphDir)
        {
            string fontPath = System.IO.Path.Combine(glyphDir, "NotoSans-Regular.ttf");
            SKTypeface noto = SKTypeface.FromFile(fontPath);
            return noto;


        }


        protected SKTypeface GetFont(string[] familyNames, Brush style)
        {
            if (notoFont != null)
                return notoFont;

            notoFont = GetNoto(style.GlyphsDirectory);
            return notoFont;


            foreach (string name in familyNames)
            {
                if (fontPairs.ContainsKey(name))
                {
                    return fontPairs[name];
                } // End if (fontPairs.ContainsKey(name)) 

                if (style.GlyphsDirectory != null)
                {
                    // check file system for ttf
                    SKTypeface newType = SKTypeface.FromFile(System.IO.Path.Combine(style.GlyphsDirectory, name + ".ttf"));
                    if (newType != null)
                    {
                        fontPairs[name] = newType;
                        return newType;
                    } // End if (newType != null) 

                    // check file system for otf
                    newType = SKTypeface.FromFile(System.IO.Path.Combine(style.GlyphsDirectory, name + ".otf"));
                    if (newType != null)
                    {
                        fontPairs[name] = newType;
                        return newType;
                    } // End if (newType != null) 

                } // End if (style.GlyphsDirectory != null) 

                SKTypeface typeface = SKTypeface.FromFamilyName(name);
                if (typeface.FamilyName == name)
                {
                    // gotcha!
                    fontPairs[name] = typeface;
                    return typeface;
                } // End if (typeface.FamilyName == name) 

            } // Next name 

            // all options exhausted...
            // get the first one
            SKTypeface fallback = SKTypeface.FromFamilyName(familyNames.First());
            fontPairs[familyNames.First()] = fallback;
            return fallback;
        } // End Function GetFont 


        protected static FontManager fnt = new NotoFontManager(@"D:\Stefan.Steiger\Downloads\Noto-hinted");


        public void DrawText(Point geometry, Brush style, System.Collections.Generic.Dictionary<string, object> attributesDict)
        {
            if (style.Paint.TextOptional)
            {
                // TODO check symbol collision
                //return;
            }

            SKPaint paint = GetTextPaint(style);
            SKPaint strokePaint = GetTextStrokePaint(style);
            string text = TransformText(style.Text, style);
            string[] allLines = text.Split('\n');

            // detect collisions
            if (allLines.Length > 0)
            {
                string biggestLine = allLines.OrderBy(line => line.Length).Last();
                // byte[] bytes = System.Text.Encoding.UTF32.GetBytes(biggestLine);

                // int width = (int)(paint.MeasureText(bytes));
                int width = (int)paint.MeasureText(biggestLine);
                int left = (int)(geometry.X - width / 2);
                int top = (int)(geometry.Y - style.Paint.TextSize / 2 * allLines.Length);
                int height = (int)(style.Paint.TextSize * allLines.Length);

                Rect rectangle = new Rect(left, top, width, height);
                rectangle.Inflate(5, 5);

                if (ClipOverflow)
                {
                    if (!clipRectangle.Contains(rectangle))
                    {
                        return;
                    } // End if(!clipRectangle.Contains(rectangle)) 

                } // End if(ClipOverflow) 

                if (TextCollides(rectangle))
                {
                    // collision detected
                    return;
                } // End if (TextCollides(rectangle)) 

                textRectangles.Add(rectangle);

                // List<Point> list = new List<Point>()
                //{
                //    rectangle.TopLeft,
                //    rectangle.TopRight,
                //    rectangle.BottomRight,
                //    rectangle.BottomLeft,
                //};

                //Brush brush = new Brush();
                //brush.Paint = new Paint();
                //brush.Paint.FillColor = Color.FromArgb(150, 255, 0, 0);

                //this.DrawPolygon(list, brush);
            } // End if(allLines.Length > 0) 

            //   gl._glMap.setLayoutProperty(keys[i], 'text-field', ["coalesce", ["get", "name:ru"], ["get", "name:latin"], ["get", "name:nonlatin"]]);


            int i = 0;
            foreach (string line in allLines)
            {
                // byte[] bytes = System.Text.Encoding.UTF32.GetBytes(line);
                float lineOffset = (float)(i * style.Paint.TextSize) - ((float)(allLines.Length) * (float)style.Paint.TextSize) / 2 + (float)style.Paint.TextSize;
                SKPoint position = new SKPoint((float)geometry.X + (float)(style.Paint.TextOffset.X * style.Paint.TextSize), (float)geometry.Y + (float)(style.Paint.TextOffset.Y * style.Paint.TextSize) + lineOffset);

                FontInfo tf = fnt.GetBestMatchingFont(line);

                if (style.Paint.TextStrokeWidth != 0)
                {
                    if (tf != null)
                        strokePaint.Typeface = tf.Typeface;

                    canvas.DrawText(line, position, strokePaint);
                    // canvas.DrawText(bytes, position, strokePaint);
                }

                if (tf != null)
                    paint.Typeface = fnt.GetBestMatchingFont(line).Typeface;

                // canvas.DrawText(bytes, position, paint);
                canvas.DrawText(line, position, paint);
                i++;
            } // Next line 

        } // End Sub DrawText 


        public void DrawTextOnPath(List<Point> geometry, Brush style)
        {
            // buggggyyyyyy
            // requires an amazing collision system to work :/
            // --
            return;

            if (ClipOverflow)
            {
                geometry = ClipLine(geometry);
                if (geometry == null)
                {
                    return;
                }
            } // End if (ClipOverflow) 

            SKPath path = GetPathFromGeometry(geometry);
            string text = TransformText(style.Text, style);

            double left = geometry.Min(item => item.X) - style.Paint.TextSize;
            double top = geometry.Min(item => item.Y) - style.Paint.TextSize;
            double right = geometry.Max(item => item.X) + style.Paint.TextSize;
            double bottom = geometry.Max(item => item.Y) + style.Paint.TextSize;

            Rect rectangle = new Rect(left, top, right - left, bottom - top);

            if (TextCollides(rectangle))
            {
                // collision detected
                return;
            }
            textRectangles.Add(rectangle);


            // List<Point> list = new List<Point>()
            //{
            //    rectangle.TopLeft,
            //    rectangle.TopRight,
            //    rectangle.BottomRight,
            //    rectangle.BottomLeft,
            //};

            // Brush brush = new Brush();
            // brush.Paint = new Paint();
            // brush.Paint.FillColor = Color.FromArgb(150, 255, 0, 0);

            // this.DrawPolygon(list, brush);


            SKPoint offset = new SKPoint((float)style.Paint.TextOffset.X, (float)style.Paint.TextOffset.Y);
            byte[] bytes = System.Text.Encoding.UTF32.GetBytes(text);
            if (style.Paint.TextStrokeWidth != 0)
            {
                canvas.DrawTextOnPath(bytes, path, offset, GetTextStrokePaint(style));
            } // End if (style.Paint.TextStrokeWidth != 0) 

            canvas.DrawTextOnPath(bytes, path, offset, GetTextPaint(style));
        } // End Sub DrawTextOnPath 


        public void DrawPoint(Point geometry, Brush style)
        {
            if (style.Paint.IconImage != null)
            {
                // draw icon here
            }
        } // End Sub DrawPoint 


        public void DrawPolygon(List<Point> geometry, Brush style)
        {
            if (ClipOverflow)
            {
                geometry = ClipPolygon(geometry);
                if (geometry == null)
                {
                    return;
                }
            }

            SKPath path = GetPathFromGeometry(geometry);
            if (path == null)
            {
                return;
            }

            // if(style.Paint.FillColor.R == 15) { }

            if (style.Paint.FillColor == null)
                return;


            SKPaint fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeCap = ConvertCap(style.Paint.LineCap),
                Color = new SKColor(style.Paint.FillColor.R, style.Paint.FillColor.G, style.Paint.FillColor.B, (byte)Clamp(style.Paint.FillColor.A * style.Paint.FillOpacity, 0, 255)),
                IsAntialias = true,
            };

            canvas.DrawPath(path, fillPaint);
        } // End Sub DrawPolygon 


        /*
        private static SKImage ToSKImage(BitmapSource bitmap)
        {
            // TODO: maybe keep the same color types where we can, instead of just going to the platform default
            SKImageInfo info = new SKImageInfo(bitmap.PixelWidth, bitmap.PixelHeight);
            SKImage image = SKImage.Create(info);
            using (SKPixmap pixmap = image.PeekPixels())
            {
                ToSKPixmap(bitmap, pixmap);
            } // End Using pixmap 
            
            return image;
        } // End Function ToSKImage 

        
        private static void ToSKPixmap(BitmapSource bitmap, SKPixmap pixmap)
        {
            // TODO: maybe keep the same color types where we can, instead of just going to the platform default
            if (pixmap.ColorType == SKImageInfo.PlatformColorType)
            {
                SKImageInfo info = pixmap.Info;
                FormatConvertedBitmap converted = new FormatConvertedBitmap(bitmap, PixelFormats.Pbgra32, null, 0);
                converted.CopyPixels(new Int32Rect(0, 0, info.Width, info.Height), pixmap.GetPixels(), info.BytesSize, info.RowBytes);
            }
            else
            {
                // we have to copy the pixels into a format that we understand
                // and then into a desired format
                // TODO: we can still do a bit more for other cases where the color types are the same
                using (SKImage tempImage = ToSKImage(bitmap))
                {
                    tempImage.ReadPixels(pixmap, 0, 0);
                }
            }
        } // End Function ToSKPixmap 
        */


        public void DrawImage(System.IO.Stream imageStream, Brush style)
        {
            // SKImage image = ToSKImage(bitmapImage);

            // SKBitmap image = SKBitmap.Decode(imageStream);
            // canvas.DrawBitmap(image, new SKPoint(0, 0));


            // SKImage.FromEncodedData
            // SKImage.FromBitmap()

            // SKManagedStream sk = new SKManagedStream(imageStream);
            // SKData sd = SKData.CreateCopy(sk.Handle, (ulong)sk.Length);
            SKData sd = SKData.Create(imageStream);
            using (SKImage image = SKImage.FromEncodedData(sd))
            {
                canvas.DrawImage(image, new SKPoint(0, 0));
            } // End Using image 

        } // End Sub DrawImage 


        public void DrawUnknown(List<List<Point>> geometry, Brush style)
        { } // End Sub DrawUnknown 


        public byte[] FinishDrawing()
        {
            byte[] pngBytes = null;

            using (SKData skd = this.bitmap.Encode(SKEncodedImageFormat.Png, 100))
            {
                pngBytes = skd.ToArray();
            } // End Using skd 

            return pngBytes;
        } // End Function FinishDrawing 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
                    if (this.bitmap != null)
                        this.bitmap.Dispose();

                    if (this.canvas != null)
                        this.canvas.Dispose();

                    // this.grContext.Dispose();
                    // this.GRBackendRenderTargetDesc.Dispose();

                    if (this.fontPairs != null)
                    {
                        foreach (KeyValuePair<string, SKTypeface> kvp in this.fontPairs)
                        {
                            if (kvp.Value != null)
                            {
                                kvp.Value.Dispose();
                            }
                        } // Next kvp 

                    } // End if (this.fontPairs != null) 

                } // End if (disposing) 

                if (this.fontPairs != null)
                    this.fontPairs.Clear();

                if (this.clipRectanglePath != null)
                    this.clipRectanglePath.Clear();

                if (this.textRectangles != null)
                    this.textRectangles.Clear();

                this.fontPairs = null;
                this.clipRectanglePath = null;
                this.textRectangles = null;

                // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
                // TODO: Große Felder auf NULL setzen
                disposedValue = true;
            }
        }

        // // TODO: Finalizer nur überschreiben, wenn "Dispose(bool disposing)" Code für die Freigabe nicht verwalteter Ressourcen enthält
        // ~SkiaCanvas()
        // {
        //     // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
        //     Dispose(disposing: false);
        // }

        void System.IDisposable.Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    } // End Class SkiaCanvas 


} // End Namespace 
