
namespace VectorTileRenderer
{

    /// <summary>
    /// Int32Rect - The primitive which represents an integer rectangle.
    /// </summary>
    public partial struct Int32Rect
    {
        private int _x;
        private int _y;
        private int _width;
        private int _height;


        //public System.Windows.Int32Rect WindowsShit
        //{
        //    get {
        //        return new System.Windows.Int32Rect(_x, _y, _width, _height);
        //    }
        //}


        /// <summary>
        /// Constructor which sets the initial values to the values of the parameters.
        /// </summary>
        public Int32Rect(int x,
                    int y,
                    int width,
                    int height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Empty - a static property which provides an Empty Int32Rectangle.
        /// </summary>
        public static Int32Rect Empty
        {
            get
            {
                return s_empty;
            }
        }

        /// <summary>
        /// Returns true if this Int32Rect is the Empty integer rectangle.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (_x == 0) && (_y == 0) && (_width == 0) && (_height == 0);
            }
        }

        /// <summary>
        /// Returns true if this Int32Rect has area.
        /// </summary>
        public bool HasArea
        {
            get
            {
                return _width > 0 && _height > 0;
            }
        }

        private readonly static Int32Rect s_empty = new Int32Rect(0, 0, 0, 0);

    }


}
