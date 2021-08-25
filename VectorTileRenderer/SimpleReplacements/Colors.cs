
namespace VectorTileRenderer
{


    public class ColorConverter
    {


        public static Color ConvertFromString(string colorString)
        {
            return FromHtml(colorString);
        }


        private static Color FromHtml(string htmlColor)
        {
            Color c = FromHtmlRgb(htmlColor);

            if (c != null)
                return c;

            System.Collections.Generic.Dictionary<string, string> dict = GetCss4ColorList();
            if (dict.ContainsKey(htmlColor))
            {
                htmlColor = dict[htmlColor];
                c = FromHtmlRgb(htmlColor);

                if (c != null)
                    return c;
            }

            dict = GetSystemColorsList();
            if (dict.ContainsKey(htmlColor))
            {
                htmlColor = dict[htmlColor];
                c = FromHtmlRgb(htmlColor);

                if (c != null)
                    return c;
            }

            if (string.Equals(htmlColor, "empty", System.StringComparison.InvariantCultureIgnoreCase))
                return Colors.Empty;

            if (string.Equals(htmlColor, "transparent", System.StringComparison.InvariantCultureIgnoreCase))
                return Colors.Transparent;

            throw new System.InvalidCastException("There is NO html-color \"" + htmlColor + "\"!");
        }



        private static Color FromHtmlRgb(string htmlColor)
        {
            Color c = Colors.Empty; // Color.empty

            // empty color
            if ((htmlColor == null) || (htmlColor.Length == 0))
                return c;

            // #RRGGBB or #RGB
            if ((htmlColor[0] == '#') &&
                ((htmlColor.Length == 7) || (htmlColor.Length == 4)))
            {
                if (htmlColor.Length == 7)
                {
                    c = Color.FromArgb(System.Convert.ToInt32(htmlColor.Substring(1, 2), 16),
                                       System.Convert.ToInt32(htmlColor.Substring(3, 2), 16),
                                       System.Convert.ToInt32(htmlColor.Substring(5, 2), 16));
                }
                else
                {
                    string r = char.ToString(htmlColor[1]);
                    string g = char.ToString(htmlColor[2]);
                    string b = char.ToString(htmlColor[3]);

                    c = Color.FromArgb(System.Convert.ToInt32(r + r, 16),
                                       System.Convert.ToInt32(g + g, 16),
                                       System.Convert.ToInt32(b + b, 16));
                }

                return c;
            }

            return null;
        }



        public static System.Collections.Generic.Dictionary<string, string> GetCss4ColorList()
        {
            System.Collections.Generic.Dictionary<string, string> dict =
                new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.InvariantCultureIgnoreCase);


            // https://en.wikipedia.org/wiki/Web_colors
            // https://css-tricks.com/snippets/css/named-colors-and-hex-equivalents/
            // https://www.w3.org/TR/css-color-4/#valdef-color-aquamarine

            dict.Add("aliceblue", "#F0F8FF");
            dict.Add("antiquewhite", "#FAEBD7");
            dict.Add("aqua", "#00FFFF");
            dict.Add("aquamarine", "#7FFFD4");
            dict.Add("azure", "#F0FFFF");
            dict.Add("beige", "#F5F5DC");
            dict.Add("bisque", "#FFE4C4");
            dict.Add("black", "#000000");
            dict.Add("blanchedalmond", "#FFEBCD");
            dict.Add("blue", "#0000FF");
            dict.Add("blueviolet", "#8A2BE2");
            dict.Add("brown", "#A52A2A");
            dict.Add("burlywood", "#DEB887");
            dict.Add("cadetblue", "#5F9EA0");
            dict.Add("chartreuse", "#7FFF00");
            dict.Add("chocolate", "#D2691E");
            dict.Add("coral", "#FF7F50");
            dict.Add("cornflowerblue", "#6495ED");
            dict.Add("cornsilk", "#FFF8DC");
            dict.Add("crimson", "#DC143C");
            dict.Add("cyan", "#00FFFF");
            dict.Add("darkblue", "#00008B");
            dict.Add("darkcyan", "#008B8B");
            dict.Add("darkgoldenrod", "#B8860B");
            dict.Add("darkgray", "#A9A9A9");
            dict.Add("darkgreen", "#006400");
            dict.Add("darkgrey", "#A9A9A9");
            dict.Add("darkkhaki", "#BDB76B");
            dict.Add("darkmagenta", "#8B008B");
            dict.Add("darkolivegreen", "#556B2F");
            dict.Add("darkorange", "#FF8C00");
            dict.Add("darkorchid", "#9932CC");
            dict.Add("darkred", "#8B0000");
            dict.Add("darksalmon", "#E9967A");
            dict.Add("darkseagreen", "#8FBC8F");
            dict.Add("darkslateblue", "#483D8B");
            dict.Add("darkslategray", "#2F4F4F");
            dict.Add("darkslategrey", "#2F4F4F");
            dict.Add("darkturquoise", "#00CED1");
            dict.Add("darkviolet", "#9400D3");
            dict.Add("deeppink", "#FF1493");
            dict.Add("deepskyblue", "#00BFFF");
            dict.Add("dimgray", "#696969");
            dict.Add("dimgrey", "#696969");
            dict.Add("dodgerblue", "#1E90FF");
            dict.Add("firebrick", "#B22222");
            dict.Add("floralwhite", "#FFFAF0");
            dict.Add("forestgreen", "#228B22");
            dict.Add("fuchsia", "#FF00FF");
            dict.Add("gainsboro", "#DCDCDC");
            dict.Add("ghostwhite", "#F8F8FF");
            dict.Add("gold", "#FFD700");
            dict.Add("goldenrod", "#DAA520");
            dict.Add("gray", "#808080");
            dict.Add("green", "#008000");
            dict.Add("greenyellow", "#ADFF2F");
            dict.Add("grey", "#808080");
            dict.Add("honeydew", "#F0FFF0");
            dict.Add("hotpink", "#FF69B4");
            dict.Add("indianred", "#CD5C5C");
            dict.Add("indigo", "#4B0082");
            dict.Add("ivory", "#FFFFF0");
            dict.Add("khaki", "#F0E68C");
            dict.Add("lavender", "#E6E6FA");
            dict.Add("lavenderblush", "#FFF0F5");
            dict.Add("lawngreen", "#7CFC00");
            dict.Add("lemonchiffon", "#FFFACD");
            dict.Add("lightblue", "#ADD8E6");
            dict.Add("lightcoral", "#F08080");
            dict.Add("lightcyan", "#E0FFFF");
            dict.Add("lightgoldenrodyellow", "#FAFAD2");
            dict.Add("lightgray", "#D3D3D3");
            dict.Add("lightgreen", "#90EE90");
            dict.Add("lightgrey", "#D3D3D3");
            dict.Add("lightpink", "#FFB6C1");
            dict.Add("lightsalmon", "#FFA07A");
            dict.Add("lightseagreen", "#20B2AA");
            dict.Add("lightskyblue", "#87CEFA");
            dict.Add("lightslategray", "#778899");
            dict.Add("lightslategrey", "#778899");
            dict.Add("lightsteelblue", "#B0C4DE");
            dict.Add("lightyellow", "#FFFFE0");
            dict.Add("lime", "#00FF00");
            dict.Add("limegreen", "#32CD32");
            dict.Add("linen", "#FAF0E6");
            dict.Add("magenta", "#FF00FF");
            dict.Add("maroon", "#800000");
            dict.Add("mediumaquamarine", "#66CDAA");
            dict.Add("mediumblue", "#0000CD");
            dict.Add("mediumorchid", "#BA55D3");
            dict.Add("mediumpurple", "#9370DB");
            dict.Add("mediumseagreen", "#3CB371");
            dict.Add("mediumslateblue", "#7B68EE");
            dict.Add("mediumspringgreen", "#00FA9A");
            dict.Add("mediumturquoise", "#48D1CC");
            dict.Add("mediumvioletred", "#C71585");
            dict.Add("midnightblue", "#191970");
            dict.Add("mintcream", "#F5FFFA");
            dict.Add("mistyrose", "#FFE4E1");
            dict.Add("moccasin", "#FFE4B5");
            dict.Add("navajowhite", "#FFDEAD");
            dict.Add("navy", "#000080");
            dict.Add("oldlace", "#FDF5E6");
            dict.Add("olive", "#808000");
            dict.Add("olivedrab", "#6B8E23");
            dict.Add("orange", "#FFA500");
            dict.Add("orangered", "#FF4500");
            dict.Add("orchid", "#DA70D6");
            dict.Add("palegoldenrod", "#EEE8AA");
            dict.Add("palegreen", "#98FB98");
            dict.Add("paleturquoise", "#AFEEEE");
            dict.Add("palevioletred", "#DB7093");
            dict.Add("papayawhip", "#FFEFD5");
            dict.Add("peachpuff", "#FFDAB9");
            dict.Add("peru", "#CD853F");
            dict.Add("pink", "#FFC0CB");
            dict.Add("plum", "#DDA0DD");
            dict.Add("powderblue", "#B0E0E6");
            dict.Add("purple", "#800080");
            dict.Add("rebeccapurple", "#663399");
            dict.Add("red", "#FF0000");
            dict.Add("rosybrown", "#BC8F8F");
            dict.Add("royalblue", "#4169E1");
            dict.Add("saddlebrown", "#8B4513");
            dict.Add("salmon", "#FA8072");
            dict.Add("sandybrown", "#F4A460");
            dict.Add("seagreen", "#2E8B57");
            dict.Add("seashell", "#FFF5EE");
            dict.Add("sienna", "#A0522D");
            dict.Add("silver", "#C0C0C0");
            dict.Add("skyblue", "#87CEEB");
            dict.Add("slateblue", "#6A5ACD");
            dict.Add("slategray", "#708090");
            dict.Add("slategrey", "#708090");
            dict.Add("snow", "#FFFAFA");
            dict.Add("springgreen", "#00FF7F");
            dict.Add("steelblue", "#4682B4");
            dict.Add("tan", "#D2B48C");
            dict.Add("teal", "#008080");
            dict.Add("thistle", "#D8BFD8");
            dict.Add("tomato", "#FF6347");
            dict.Add("turquoise", "#40E0D0");
            dict.Add("violet", "#EE82EE");
            dict.Add("wheat", "#F5DEB3");
            dict.Add("white", "#FFFFFF");
            dict.Add("whitesmoke", "#F5F5F5");
            dict.Add("yellow", "#FFFF00");
            dict.Add("yellowgreen", "#9ACD32");

            return dict;
        }


        public class MultiDict<TKey, TValue>  // no (collection) base class
            : System.Collections.Generic.ICollection<
                System.Collections.Generic.KeyValuePair<TKey, System.Collections.Generic.List<TValue>>>
        {
            public System.Collections.Generic.Dictionary<TKey, System.Collections.Generic.List<TValue>> _data =
                new System.Collections.Generic.Dictionary<TKey, System.Collections.Generic.List<TValue>>();

            public int Count
            {
                get
                {
                    return this._data.Count;
                }
            }


            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }


            public void Add(TKey k, TValue v)
            {
                // can be a optimized a little with TryGetValue, this is for clarity
                if (_data.ContainsKey(k))
                    _data[k].Add(v);
                else
                    _data.Add(k, new System.Collections.Generic.List<TValue>() { v });
            }

            public void Add(System.Collections.Generic.KeyValuePair<TKey, System.Collections.Generic.List<TValue>> item)
            {
                this._data.Add(item.Key, item.Value);
            }

            public void Clear()
            {
                this._data.Clear();
            }

            public bool Contains(System.Collections.Generic.KeyValuePair<TKey, System.Collections.Generic.List<TValue>> item)
            {
                return this._data.ContainsKey(item.Key);
            }

            public void CopyTo(System.Collections.Generic.KeyValuePair<TKey, System.Collections.Generic.List<TValue>>[] array, int arrayIndex)
            {
                throw new System.NotImplementedException();
            }

            public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, System.Collections.Generic.List<TValue>>> GetEnumerator()
            {
                return this._data.GetEnumerator();
            }

            public bool Remove(System.Collections.Generic.KeyValuePair<TKey, System.Collections.Generic.List<TValue>> item)
            {
                return this._data.Remove(item.Key);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this._data.GetEnumerator();
            }
        }

        public static void UnduplicateKnownColors()
        {
            MultiDict<uint, string> dict = new MultiDict<uint, string>();

            System.Collections.Generic.Dictionary<string, uint> reverse =
                new System.Collections.Generic.Dictionary<string, uint>(System.StringComparer.InvariantCultureIgnoreCase);

            // https://github.com/mono/mono/blob/main/mcs/class/System.Drawing/System.Drawing/KnownColors.cs


            dict.Add(0x00000000, "000 - Empty");
            dict.Add(0xFFD4D0C8, "001 - ActiveBorder");
            dict.Add(0xFF0054E3, "002 - ActiveCaption");
            dict.Add(0xFFFFFFFF, "003 - ActiveCaptionText");
            dict.Add(0xFF808080, "004 - AppWorkspace");
            dict.Add(0xFFECE9D8, "005 - Control");
            dict.Add(0xFFACA899, "006 - ControlDark");
            dict.Add(0xFF716F64, "007 - ControlDarkDark");
            dict.Add(0xFFF1EFE2, "008 - ControlLight");
            dict.Add(0xFFFFFFFF, "009 - ControlLightLight");
            dict.Add(0xFF000000, "010 - ControlText");
            dict.Add(0xFF004E98, "011 - Desktop");
            dict.Add(0xFFACA899, "012 - GrayText");
            dict.Add(0xFF316AC5, "013 - Highlight");
            dict.Add(0xFFFFFFFF, "014 - HighlightText");
            dict.Add(0xFF000080, "015 - HotTrack");
            dict.Add(0xFFD4D0C8, "016 - InactiveBorder");
            dict.Add(0xFF7A96DF, "017 - InactiveCaption");
            dict.Add(0xFFD8E4F8, "018 - InactiveCaptionText");
            dict.Add(0xFFFFFFE1, "019 - Info");
            dict.Add(0xFF000000, "020 - InfoText");
            dict.Add(0xFFFFFFFF, "021 - Menu");
            dict.Add(0xFF000000, "022 - MenuText");
            dict.Add(0xFFD4D0C8, "023 - ScrollBar");
            dict.Add(0xFFFFFFFF, "024 - Window");
            dict.Add(0xFF000000, "025 - WindowFrame");
            dict.Add(0xFF000000, "026 - WindowText");
            dict.Add(0x00FFFFFF, "027 - Transparent");
            dict.Add(0xFFF0F8FF, "028 - AliceBlue");
            dict.Add(0xFFFAEBD7, "029 - AntiqueWhite");
            dict.Add(0xFF00FFFF, "030 - Aqua");
            dict.Add(0xFF7FFFD4, "031 - Aquamarine");
            dict.Add(0xFFF0FFFF, "032 - Azure");
            dict.Add(0xFFF5F5DC, "033 - Beige");
            dict.Add(0xFFFFE4C4, "034 - Bisque");
            dict.Add(0xFF000000, "035 - Black");
            dict.Add(0xFFFFEBCD, "036 - BlanchedAlmond");
            dict.Add(0xFF0000FF, "037 - Blue");
            dict.Add(0xFF8A2BE2, "038 - BlueViolet");
            dict.Add(0xFFA52A2A, "039 - Brown");
            dict.Add(0xFFDEB887, "040 - BurlyWood");
            dict.Add(0xFF5F9EA0, "041 - CadetBlue");
            dict.Add(0xFF7FFF00, "042 - Chartreuse");
            dict.Add(0xFFD2691E, "043 - Chocolate");
            dict.Add(0xFFFF7F50, "044 - Coral");
            dict.Add(0xFF6495ED, "045 - CornflowerBlue");
            dict.Add(0xFFFFF8DC, "046 - Cornsilk");
            dict.Add(0xFFDC143C, "047 - Crimson");
            dict.Add(0xFF00FFFF, "048 - Cyan");
            dict.Add(0xFF00008B, "049 - DarkBlue");
            dict.Add(0xFF008B8B, "050 - DarkCyan");
            dict.Add(0xFFB8860B, "051 - DarkGoldenrod");
            dict.Add(0xFFA9A9A9, "052 - DarkGray");
            dict.Add(0xFF006400, "053 - DarkGreen");
            dict.Add(0xFFBDB76B, "054 - DarkKhaki");
            dict.Add(0xFF8B008B, "055 - DarkMagenta");
            dict.Add(0xFF556B2F, "056 - DarkOliveGreen");
            dict.Add(0xFFFF8C00, "057 - DarkOrange");
            dict.Add(0xFF9932CC, "058 - DarkOrchid");
            dict.Add(0xFF8B0000, "059 - DarkRed");
            dict.Add(0xFFE9967A, "060 - DarkSalmon");
            dict.Add(0xFF8FBC8B, "061 - DarkSeaGreen");
            dict.Add(0xFF483D8B, "062 - DarkSlateBlue");
            dict.Add(0xFF2F4F4F, "063 - DarkSlateGray");
            dict.Add(0xFF00CED1, "064 - DarkTurquoise");
            dict.Add(0xFF9400D3, "065 - DarkViolet");
            dict.Add(0xFFFF1493, "066 - DeepPink");
            dict.Add(0xFF00BFFF, "067 - DeepSkyBlue");
            dict.Add(0xFF696969, "068 - DimGray");
            dict.Add(0xFF1E90FF, "069 - DodgerBlue");
            dict.Add(0xFFB22222, "070 - Firebrick");
            dict.Add(0xFFFFFAF0, "071 - FloralWhite");
            dict.Add(0xFF228B22, "072 - ForestGreen");
            dict.Add(0xFFFF00FF, "073 - Fuchsia");
            dict.Add(0xFFDCDCDC, "074 - Gainsboro");
            dict.Add(0xFFF8F8FF, "075 - GhostWhite");
            dict.Add(0xFFFFD700, "076 - Gold");
            dict.Add(0xFFDAA520, "077 - Goldenrod");
            dict.Add(0xFF808080, "078 - Gray");
            dict.Add(0xFF008000, "079 - Green");
            dict.Add(0xFFADFF2F, "080 - GreenYellow");
            dict.Add(0xFFF0FFF0, "081 - Honeydew");
            dict.Add(0xFFFF69B4, "082 - HotPink");
            dict.Add(0xFFCD5C5C, "083 - IndianRed");
            dict.Add(0xFF4B0082, "084 - Indigo");
            dict.Add(0xFFFFFFF0, "085 - Ivory");
            dict.Add(0xFFF0E68C, "086 - Khaki");
            dict.Add(0xFFE6E6FA, "087 - Lavender");
            dict.Add(0xFFFFF0F5, "088 - LavenderBlush");
            dict.Add(0xFF7CFC00, "089 - LawnGreen");
            dict.Add(0xFFFFFACD, "090 - LemonChiffon");
            dict.Add(0xFFADD8E6, "091 - LightBlue");
            dict.Add(0xFFF08080, "092 - LightCoral");
            dict.Add(0xFFE0FFFF, "093 - LightCyan");
            dict.Add(0xFFFAFAD2, "094 - LightGoldenrodYellow");
            dict.Add(0xFFD3D3D3, "095 - LightGray");
            dict.Add(0xFF90EE90, "096 - LightGreen");
            dict.Add(0xFFFFB6C1, "097 - LightPink");
            dict.Add(0xFFFFA07A, "098 - LightSalmon");
            dict.Add(0xFF20B2AA, "099 - LightSeaGreen");
            dict.Add(0xFF87CEFA, "100 - LightSkyBlue");
            dict.Add(0xFF778899, "101 - LightSlateGray");
            dict.Add(0xFFB0C4DE, "102 - LightSteelBlue");
            dict.Add(0xFFFFFFE0, "103 - LightYellow");
            dict.Add(0xFF00FF00, "104 - Lime");
            dict.Add(0xFF32CD32, "105 - LimeGreen");
            dict.Add(0xFFFAF0E6, "106 - Linen");
            dict.Add(0xFFFF00FF, "107 - Magenta");
            dict.Add(0xFF800000, "108 - Maroon");
            dict.Add(0xFF66CDAA, "109 - MediumAquamarine");
            dict.Add(0xFF0000CD, "110 - MediumBlue");
            dict.Add(0xFFBA55D3, "111 - MediumOrchid");
            dict.Add(0xFF9370DB, "112 - MediumPurple");
            dict.Add(0xFF3CB371, "113 - MediumSeaGreen");
            dict.Add(0xFF7B68EE, "114 - MediumSlateBlue");
            dict.Add(0xFF00FA9A, "115 - MediumSpringGreen");
            dict.Add(0xFF48D1CC, "116 - MediumTurquoise");
            dict.Add(0xFFC71585, "117 - MediumVioletRed");
            dict.Add(0xFF191970, "118 - MidnightBlue");
            dict.Add(0xFFF5FFFA, "119 - MintCream");
            dict.Add(0xFFFFE4E1, "120 - MistyRose");
            dict.Add(0xFFFFE4B5, "121 - Moccasin");
            dict.Add(0xFFFFDEAD, "122 - NavajoWhite");
            dict.Add(0xFF000080, "123 - Navy");
            dict.Add(0xFFFDF5E6, "124 - OldLace");
            dict.Add(0xFF808000, "125 - Olive");
            dict.Add(0xFF6B8E23, "126 - OliveDrab");
            dict.Add(0xFFFFA500, "127 - Orange");
            dict.Add(0xFFFF4500, "128 - OrangeRed");
            dict.Add(0xFFDA70D6, "129 - Orchid");
            dict.Add(0xFFEEE8AA, "130 - PaleGoldenrod");
            dict.Add(0xFF98FB98, "131 - PaleGreen");
            dict.Add(0xFFAFEEEE, "132 - PaleTurquoise");
            dict.Add(0xFFDB7093, "133 - PaleVioletRed");
            dict.Add(0xFFFFEFD5, "134 - PapayaWhip");
            dict.Add(0xFFFFDAB9, "135 - PeachPuff");
            dict.Add(0xFFCD853F, "136 - Peru");
            dict.Add(0xFFFFC0CB, "137 - Pink");
            dict.Add(0xFFDDA0DD, "138 - Plum");
            dict.Add(0xFFB0E0E6, "139 - PowderBlue");
            dict.Add(0xFF800080, "140 - Purple");
            dict.Add(0xFFFF0000, "141 - Red");
            dict.Add(0xFFBC8F8F, "142 - RosyBrown");
            dict.Add(0xFF4169E1, "143 - RoyalBlue");
            dict.Add(0xFF8B4513, "144 - SaddleBrown");
            dict.Add(0xFFFA8072, "145 - Salmon");
            dict.Add(0xFFF4A460, "146 - SandyBrown");
            dict.Add(0xFF2E8B57, "147 - SeaGreen");
            dict.Add(0xFFFFF5EE, "148 - SeaShell");
            dict.Add(0xFFA0522D, "149 - Sienna");
            dict.Add(0xFFC0C0C0, "150 - Silver");
            dict.Add(0xFF87CEEB, "151 - SkyBlue");
            dict.Add(0xFF6A5ACD, "152 - SlateBlue");
            dict.Add(0xFF708090, "153 - SlateGray");
            dict.Add(0xFFFFFAFA, "154 - Snow");
            dict.Add(0xFF00FF7F, "155 - SpringGreen");
            dict.Add(0xFF4682B4, "156 - SteelBlue");
            dict.Add(0xFFD2B48C, "157 - Tan");
            dict.Add(0xFF008080, "158 - Teal");
            dict.Add(0xFFD8BFD8, "159 - Thistle");
            dict.Add(0xFFFF6347, "160 - Tomato");
            dict.Add(0xFF40E0D0, "161 - Turquoise");
            dict.Add(0xFFEE82EE, "162 - Violet");
            dict.Add(0xFFF5DEB3, "163 - Wheat");
            dict.Add(0xFFFFFFFF, "164 - White");
            dict.Add(0xFFF5F5F5, "165 - WhiteSmoke");
            dict.Add(0xFFFFFF00, "166 - Yellow");
            dict.Add(0xFF9ACD32, "167 - YellowGreen");
            dict.Add(0xFFECE9D8, "168 - ButtonFace");
            dict.Add(0xFFFFFFFF, "169 - ButtonHighlight");
            dict.Add(0xFFACA899, "170 - ButtonShadow");
            dict.Add(0xFF3D95FF, "171 - GradientActiveCaption");
            dict.Add(0xFF9DB9EB, "172 - GradientInactiveCaption");
            dict.Add(0xFFECE9D8, "173 - MenuBar");
            dict.Add(0xFF316AC5, "174 - MenuHighlight");

            foreach (System.Collections.Generic.KeyValuePair<uint, System.Collections.Generic.List<string>> kvp in dict)
            {
                for (int i = 0; i < kvp.Value.Count; ++i)
                {
                    int pos = kvp.Value[i].IndexOf(" - ");
                    string rest = kvp.Value[i].Substring(pos + 3);
                    kvp.Value[i] = rest;

                    reverse.Add(rest, kvp.Key);
                }
            }

            System.Console.WriteLine(reverse);


            System.Collections.Generic.Dictionary<string, string> foo = GetCss4ColorList();
            foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in foo)
            {
                if (reverse.ContainsKey(kvp.Key))
                    reverse.Remove(kvp.Key);
            }


            reverse.Remove("Empty");
            reverse.Remove("Transparent");

            string sb = "";

            foreach (System.Collections.Generic.KeyValuePair<string, uint> kvp in reverse)
            {
                sb += "dict.Add(\"" + kvp.Key.ToLowerInvariant() + "\", \"" + Color.FromArgb(kvp.Value).ToHtml() + "\"); " + System.Environment.NewLine;
            }

            System.Console.WriteLine(sb);

        }



        public static System.Collections.Generic.Dictionary<string, string> GetSystemColorsList()
        {
            System.Collections.Generic.Dictionary<string, string> dict =
                new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.InvariantCultureIgnoreCase);

            dict.Add("activeborder", "#D4D0C8");
            dict.Add("inactiveborder", "#D4D0C8");
            dict.Add("scrollbar", "#D4D0C8");
            dict.Add("activecaption", "#0054E3");
            dict.Add("activecaptiontext", "#FFFFFF");
            dict.Add("controllightlight", "#FFFFFF");
            dict.Add("highlighttext", "#FFFFFF");
            dict.Add("menu", "#FFFFFF");
            dict.Add("window", "#FFFFFF");
            dict.Add("buttonhighlight", "#FFFFFF");
            dict.Add("appworkspace", "#808080");
            dict.Add("control", "#ECE9D8");
            dict.Add("buttonface", "#ECE9D8");
            dict.Add("menubar", "#ECE9D8");
            dict.Add("controldark", "#ACA899");
            dict.Add("graytext", "#ACA899");
            dict.Add("buttonshadow", "#ACA899");
            dict.Add("controldarkdark", "#716F64");
            dict.Add("controllight", "#F1EFE2");
            dict.Add("controltext", "#000000");
            dict.Add("infotext", "#000000");
            dict.Add("menutext", "#000000");
            dict.Add("windowframe", "#000000");
            dict.Add("windowtext", "#000000");
            dict.Add("desktop", "#004E98");
            dict.Add("highlight", "#316AC5");
            dict.Add("menuhighlight", "#316AC5");
            dict.Add("hottrack", "#000080");
            dict.Add("inactivecaption", "#7A96DF");
            dict.Add("inactivecaptiontext", "#D8E4F8");
            dict.Add("info", "#FFFFE1");
            dict.Add("gradientactivecaption", "#3D95FF");
            dict.Add("gradientinactivecaption", "#9DB9EB");

            return dict;
        }



    }


    public class Colors
    {

        public static Color White
        {
            get
            {
                return Color.FromRgb(255, 255, 255);
            }
        }

        public static Color Black
        {
            get
            {
                return Color.FromRgb(0, 0, 0);
            }
        }


        public static Color Empty
        {
            get
            {
                return Color.FromArgb(0, 0, 0, 0); // Neither black nor white, but transparent black 
            }
        }


        public static Color Transparent
        {
            get
            {
                return Color.FromArgb(0, 255, 255, 255); // Neither black nor white, but transparent black 
            }
        }
    }


    public class Color
    {

        public byte A;
        public byte R;
        public byte G;
        public byte B;


        public uint ToUint()
        {
            // color.value = (int)((uint)alpha << 24) + (red << 16) + (green << 8) + blue;
            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/23cc280f-306e-444d-9577-3d6458094b99/converting-from-color-to-uint-and-vice-versa?forum=wpf
            return (uint)(((this.A << 24) | (this.R << 16) | (this.G << 8) | this.B) & 0xffffffffL);
        }


        public string ToHtml()
        {

            return FormatHtml(this.R, this.G, this.B);
        }




        public static Color FromArgb(uint value)
        {
            return FromArgb((byte)((value >> 24) & 0xFF),
                       (byte)((value >> 16) & 0xFF),
                       (byte)((value >> 8) & 0xFF),
                       (byte)(value & 0xFF));
        }


        static char GetHexNumber(int b)
        {
            return (char)(b > 9 ? 55 + b : 48 + b);
        }


        static string FormatHtml(int r, int g, int b)
        {
            // View current browser support for #RRGGBBAA color notation

            char[] htmlColor = new char[7];
            htmlColor[0] = '#';
            htmlColor[1] = GetHexNumber((r >> 4) & 15);
            htmlColor[2] = GetHexNumber(r & 15);
            htmlColor[3] = GetHexNumber((g >> 4) & 15);
            htmlColor[4] = GetHexNumber(g & 15);
            htmlColor[5] = GetHexNumber((b >> 4) & 15);
            htmlColor[6] = GetHexNumber(b & 15);

            return new string(htmlColor);
        }

        public static Color FromRgb(byte r, byte g, byte b)
        {
            return new Color() { A = 255, R = r, G = g, B = b };
        }

        public static Color FromArgb(int a, int r, int g, int b)
        {
            return FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }


        public static Color FromArgb(int r, int g, int b)
        {
            return FromArgb((byte)255, (byte)r, (byte)g, (byte)b);
        }


        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            return new Color() { A = a, R = r, G = g, B = b };
        }


    }



}
