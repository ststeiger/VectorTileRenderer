
namespace VectorTileRenderer
{

    public struct Rect
    {
        double _x;
        double _y;
        double _width;
        double _height;


        public double Left
        {
            get { return _x; }
        }

        public double Top
        {
            get { return _y; }
        }

        public double Right
        {
            get { return _x + _width; }
        }

        public double Bottom
        {
            get { return _y + _height; }
        }

        public bool IsEmpty
        {
            get
            {
                return (_x == double.PositiveInfinity &&
                    _y == double.PositiveInfinity &&
                    _width == double.NegativeInfinity &&
                    _height == double.NegativeInfinity);
            }
        }

        public double X
        {
            get { return _x; }
            set
            {
                if (IsEmpty)
                    throw new System.InvalidOperationException("Cannot modify this property on the Empty Rect.");

                _x = value;
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                if (IsEmpty)
                    throw new System.InvalidOperationException("Cannot modify this property on the Empty Rect.");

                _y = value;
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                if (IsEmpty)
                    throw new System.InvalidOperationException("Cannot modify this property on the Empty Rect.");

                if (value < 0)
                    throw new System.ArgumentException("width must be non-negative.");

                _width = value;
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                if (IsEmpty)
                    throw new System.InvalidOperationException("Cannot modify this property on the Empty Rect.");

                if (value < 0)
                    throw new System.ArgumentException("height must be non-negative.");

                _height = value;
            }
        }


        public Rect(Point point1, Point point2)
        {
            if (point1.X < point2.X)
            {
                _x = point1.X;
                _width = point2.X - point1.X;
            }
            else
            {
                _x = point2.X;
                _width = point1.X - point2.X;
            }

            if (point1.Y < point2.Y)
            {
                _y = point1.Y;
                _height = point2.Y - point1.Y;
            }
            else
            {
                _y = point2.Y;
                _height = point1.Y - point2.Y;
            }
        }

        public Rect(double x, double y, double width, double height)
        {
            if (width < 0 || height < 0)
                throw new System.ArgumentException("width and height must be non-negative.");

            this._x = x;
            this._y = y;
            this._width = width;
            this._height = height;
        }

        public bool IntersectsWith(Rect rect)
        {
            return !((Left >= rect.Right) || (Right <= rect.Left) ||
                (Top >= rect.Bottom) || (Bottom <= rect.Top));
        }


        public void Inflate(double width, double height)
        {
            // XXX any error checking like in the static case?
            _x -= width;
            _y -= height;

            this._width += 2 * width;
            this._height += 2 * height;
        }

        public bool Contains(Rect rect)
        {
            if (rect.Left < this.Left ||
                rect.Right > this.Right)
                return false;

            if (rect.Top < this.Top ||
                rect.Bottom > this.Bottom)
                return false;

            return true;
        }


        public override bool Equals(object o)
        {
            if (!(o is Rect))
                return false;

            return Equals((Rect)o);
        }

        public bool Equals(Rect value)
        {
            return (_x == value.X &&
                _y == value.Y &&
                _width == value.Width &&
                _height == value.Height);
        }


        public Point Location
        {
            get
            {
                return new Point(_x, _y);
            }
            set
            {
                if (IsEmpty)
                    throw new System.InvalidOperationException("Cannot modify this property on the Empty Rect.");

                _x = value.X;
                _y = value.Y;
            }
        }

        public Size Size
        {
            get
            {
                if (IsEmpty)
                    return Size.Empty;
                return new Size(_width, _height);
            }
            set
            {
                if (IsEmpty)
                    throw new System.InvalidOperationException("Cannot modify this property on the Empty Rect.");

                _width = value.Width;
                _height = value.Height;
            }
        }

        public static bool operator !=(Rect rect1, Rect rect2)
        {
            return !(rect1.Location == rect2.Location && rect1.Size == rect2.Size);
        }

        public static bool operator ==(Rect rect1, Rect rect2)
        {
            return rect1.Location == rect2.Location && rect1.Size == rect2.Size;
        }

    }


}
