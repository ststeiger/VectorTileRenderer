
namespace VectorTileRenderer
{

    public struct Size
    {
        private double _width;
        private double _height;

        public bool IsEmpty
        {
            get
            {
                return (_width == double.NegativeInfinity &&
                    _height == double.NegativeInfinity);
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                if (IsEmpty)
                    throw new System.InvalidOperationException("Cannot modify this property on the Empty Size.");

                if (value < 0)
                    throw new System.ArgumentException("height must be non-negative.");

                _height = value;
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                if (IsEmpty)
                    throw new System.InvalidOperationException("Cannot modify this property on the Empty Size.");

                if (value < 0)
                    throw new System.ArgumentException("width must be non-negative.");

                _width = value;
            }
        }

        public static Size Empty
        {
            get
            {
                Size s = new Size();
                s._width = s._height = double.NegativeInfinity;
                return s;
            }
        }


        public Size(double width, double height)
        {
            if (width < 0 || height < 0)
                throw new System.ArgumentException("Width and Height must be non-negative.");

            this._width = width;
            this._height = height;
        }

        public bool Equals(Size value)
        {
            return _width == value.Width && _height == value.Height;
        }

        public override bool Equals(object o)
        {
            if (!(o is Size))
                return false;

            return Equals((Size)o);
        }

        public static bool Equals(Size size1, Size size2)
        {
            return size1.Equals(size2);
        }


        public static bool operator ==(Size size1, Size size2)
        {
            return size1.Equals(size2);
        }

        public static bool operator !=(Size size1, Size size2)
        {
            return !size1.Equals(size2);
        }
    }


}
