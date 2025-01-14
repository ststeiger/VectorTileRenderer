﻿//Apache2, 2017-present, WinterDev
//Apache2, 2014-2016, Samuel Carlsson, WinterDev
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Typography.OpenFont.Tables;

namespace Typography.OpenFont
{

    public partial class Typeface
    {

        //TODO: implement vertical metrics
        HorizontalMetrics _hMetrics;
        NameEntry _nameEntry;
        Glyph[] _glyphs;
        CFF.Cff1FontSet _cff1FontSet;
        TableHeader[] _tblHeaders;
        bool _hasTtfOutline;
        bool _hasCffData;

        internal Typeface()
        {
            //blank typefaces
        }

        internal void SetTableEntryCollection(TableHeader[] headers) => _tblHeaders = headers;

        internal void SetBasicTypefaceTables(OS2Table os2Table,
             NameEntry nameEntry,
             Head head,
             HorizontalMetrics horizontalMetrics)
        {
            OS2Table = os2Table;
            _nameEntry = nameEntry;
            Head = head;
            Bounds = head.Bounds;
            UnitsPerEm = head.UnitsPerEm;
            _hMetrics = horizontalMetrics;
        }

        internal Head Head { get; set; }

        internal void SetTtfGlyphs(Glyph[] glyphs)
        {
            _glyphs = glyphs;
            _hasTtfOutline = true;
        }
        internal void SetBitmapGlyphs(Glyph[] glyphs, BitmapFontGlyphSource bitmapFontGlyphSource)
        {
            _glyphs = glyphs;
            _bitmapFontGlyphSource = bitmapFontGlyphSource;
        }
        internal void SetCffFontSet(CFF.Cff1FontSet cff1FontSet)
        {
            _cff1FontSet = cff1FontSet;
            _hasCffData = true;

            Glyph[] exisitingGlyphs = _glyphs;

            _glyphs = cff1FontSet._fonts[0]._glyphs; //TODO: review _fonts[0]

            if (exisitingGlyphs != null)
            {
                //
#if DEBUG
                if (_glyphs.Length != exisitingGlyphs.Length)
                {
                    throw new NotSupportedException();
                }
#endif
                for (int i = 0; i < exisitingGlyphs.Length; ++i)
                {
                    Glyph.CopyExistingGlyphInfo(exisitingGlyphs[i], _glyphs[i]);
                }
            }

        }

        public Languages Languages { get; } = new Languages();
        /// <summary>
        /// control values in Font unit
        /// </summary>
        internal int[] ControlValues { get; set; }
        internal byte[] PrepProgramBuffer { get; set; }
        internal byte[] FpgmProgramBuffer { get; set; }

        internal MaxProfile MaxProfile { get; set; }
        internal Cmap CmapTable { get; set; }
        internal Kern KernTable { get; set; }
        internal Gasp GaspTable { get; set; }
        internal HorizontalHeader HheaTable { get; set; }
        internal OS2Table OS2Table { get; set; }


        public bool IsTrueTypeFont
        {
            get
            {
                return this.OS2Table != null;
            }
        }


        public TrueTypeEmbeddability TrueTypeEmbeddingType
        {
            get
            {
                int otmfsType = (this.OS2Table.fsType & 0xf);

                if (!this.IsTrueTypeFont)
                {
                    // We could throw, or we could just define a new enum-member. 
                    // throw new System.InvalidOperationException("Not a TrueType-font.");
                    return TrueTypeEmbeddability.EMBED_NOTTRUETYPE;
                }

                if (otmfsType == (int)LicenseFlags.LICENSE_INSTALLABLE)
                    return TrueTypeEmbeddability.EMBED_INSTALLABLE;
                else if ((otmfsType & (int)LicenseFlags.LICENSE_EDITABLE) != 0)
                    return TrueTypeEmbeddability.EMBED_EDITABLE;
                else if ((otmfsType & (int)LicenseFlags.LICENSE_PREVIEWPRINT) != 0)
                    return TrueTypeEmbeddability.EMBED_PREVIEWPRINT;
                else if ((otmfsType & (int)LicenseFlags.LICENSE_NOEMBEDDING) != 0)
                    return TrueTypeEmbeddability.EMBED_NOEMBEDDING;

                // WARN("unrecognized flags, %#x\n", otmfsType);
                return TrueTypeEmbeddability.EMBED_INSTALLABLE;
            } // End Function TTGetEmbeddingType 
        }


        public bool HasPrepProgramBuffer => PrepProgramBuffer != null;

        /// <summary>
        /// actual font filename
        /// </summary>
        public string Filename { get; set; }

        public string FilePath { get; set; }


        /// <summary>
        /// OS2 sTypoAscender, in font designed unit
        /// </summary>
        public short Ascender => OS2Table.sTypoAscender;
        /// <summary>
        /// OS2 sTypoDescender, in font designed unit
        /// </summary>
        public short Descender => OS2Table.sTypoDescender;
        /// <summary>
        /// OS2 usWinAscender
        /// </summary>
        public ushort ClipedAscender => OS2Table.usWinAscent;
        /// <summary>
        /// OS2 usWinDescender
        /// </summary>
        public ushort ClipedDescender => OS2Table.usWinDescent;

        /// <summary>
        /// OS2 Linegap
        /// </summary>
        public short LineGap => OS2Table.sTypoLineGap;
        //The typographic line gap for this font.
        //Remember that this is not the same as the LineGap value in the 'hhea' table, 
        //which Apple defines in a far different manner.
        //The suggested usage for sTypoLineGap is 
        //that it be used in conjunction with unitsPerEm 
        //to compute a typographically correct default line spacing.
        //
        //Typical values average 7 - 10 % of units per em.
        //The goal is to free applications from Macintosh or Windows - specific metrics
        //which are constrained by backward compatability requirements
        //(see chapter, “Recommendations for OpenType Fonts”).
        //These new metrics, when combined with the character design widths,
        //will allow applications to lay out documents in a typographically correct and portable fashion. 
        //These metrics will be exposed through Windows APIs.
        //Macintosh applications will need to access the 'sfnt' resource and 
        //parse it to extract this data from the “OS / 2” table
        //(unless Apple exposes the 'OS/2' table through a new API)
        //---------------

        public string Name => _nameEntry.FontName;
        public string FontSubFamily => _nameEntry.FontSubFamily;
        public string PostScriptName => _nameEntry.PostScriptName;
        public string VersionString => _nameEntry.VersionString;
        public string UniqueFontIden => _nameEntry.UniqueFontIden;

        public int GlyphCount => _glyphs.Length;

        //
        /// <summary>
        /// find glyph index by codepoint
        /// </summary>
        /// <param name="codepoint"></param>
        /// <param name="nextCodepoint"></param>
        /// <returns></returns>

        public ushort GetGlyphIndex(int codepoint, int nextCodepoint, out bool skipNextCodepoint)
        {
            return CmapTable.GetGlyphIndex(codepoint, nextCodepoint, out skipNextCodepoint);
        }
        public ushort GetGlyphIndex(int codepoint)
        {
            return CmapTable.GetGlyphIndex(codepoint, 0, out bool skipNextCodepoint);
        }
        public void CollectUnicode(List<uint> unicodes)
        {
            CmapTable.CollectUnicode(unicodes);
        }

        public Glyph GetGlyphByName(string glyphName) => GetGlyph(GetGlyphIndexByName(glyphName));

        Dictionary<string, ushort> _cachedGlyphDicByName;

        void UpdateCff1FontSetNamesCache()
        {
            if (_cff1FontSet != null && _cachedGlyphDicByName == null)
            {
                //create cache data
                _cachedGlyphDicByName = new Dictionary<string, ushort>();
                for (int i = 1; i < _glyphs.Length; ++i)
                {
                    Glyph glyph = _glyphs[i];

                    if (glyph._cff1GlyphData != null && glyph._cff1GlyphData.Name != null)
                    {
                        _cachedGlyphDicByName.Add(glyph._cff1GlyphData.Name, (ushort)i);
                    }
                    else
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("Cff unknown glyphname");
#endif
                    }
                }
            }
        }
        public ushort GetGlyphIndexByName(string glyphName)
        {
            if (glyphName == null) return 0;

            if (_cff1FontSet != null && _cachedGlyphDicByName == null)
            {
                //we create a dictionary 
                //create cache data
                _cachedGlyphDicByName = new Dictionary<string, ushort>();
                for (int i = 1; i < _glyphs.Length; ++i)
                {
                    Glyph glyph = _glyphs[i];
                    if (glyph._cff1GlyphData.Name != null)
                    {
                        _cachedGlyphDicByName.Add(glyph._cff1GlyphData.Name, (ushort)i);
                    }
                    else
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("Cff unknown glyphname");
#endif
                    }
                }
                return _cachedGlyphDicByName.TryGetValue(glyphName, out ushort glyphIndex) ? glyphIndex : (ushort)0;
            }
            else if (PostTable != null)
            {
                if (PostTable.Version == 2)
                {
                    return PostTable.GetGlyphIndex(glyphName);
                }
                else
                {
                    //check data from adobe glyph list 
                    //from the unicode value
                    //select glyph index   

                    //we use AdobeGlyphList
                    //from https://github.com/adobe-type-tools/agl-aglfn/blob/master/glyphlist.txt

                    //but user can provide their own map here...

                    return GetGlyphIndex(AdobeGlyphList.GetUnicodeValueByGlyphName(glyphName));
                }
            }
            return 0;
        }

        public IEnumerable<GlyphNameMap> GetGlyphNameIter()
        {
            if (_cachedGlyphDicByName == null && _cff1FontSet != null)
            {
                UpdateCff1FontSetNamesCache();
            }

            if (_cachedGlyphDicByName != null)
            {
                //iter from here
                foreach (var kv in _cachedGlyphDicByName)
                {
                    yield return new GlyphNameMap(kv.Value, kv.Key);
                }
            }
            else if (PostTable.Version == 2)
            {
                foreach (var kp in PostTable.GlyphNames)
                {
                    yield return new GlyphNameMap(kp.Key, kp.Value);
                }
            }
        }
        public Glyph GetGlyph(ushort glyphIndex)
        {
            if (glyphIndex < _glyphs.Length)
            {
                return _glyphs[glyphIndex];
            }
            else
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("found unknown glyph:" + glyphIndex);
#endif
                return _glyphs[0]; //return empty glyph?;
            }
        }

        public ushort GetAdvanceWidthFromGlyphIndex(ushort glyphIndex) => _hMetrics.GetAdvanceWidth(glyphIndex);
        public short GetLeftSideBearing(ushort glyphIndex) => _hMetrics.GetLeftSideBearing(glyphIndex);
        public short GetKernDistance(ushort leftGlyphIndex, ushort rightGlyphIndex)
        {
            //DEPRECATED -> use OpenFont layout instead
            return this.KernTable.GetKerningDistance(leftGlyphIndex, rightGlyphIndex);
        }
        //
        public Bounds Bounds { get; private set; }
        public ushort UnitsPerEm { get; private set; }
        public short UnderlinePosition => PostTable.UnderlinePosition; //TODO: review here
        //

        const int pointsPerInch = 72; //TODO: should be configurable
        /// <summary>
        /// convert from point-unit value to pixel value
        /// </summary>
        /// <param name="targetPointSize"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static float ConvPointsToPixels(float targetPointSize, int resolution = 96)
        {
            //http://stackoverflow.com/questions/139655/convert-pixels-to-points
            //points = pixels * 72 / 96
            //------------------------------------------------
            //pixels = targetPointSize * 96 /72
            //pixels = targetPointSize * resolution / pointPerInch
            return targetPointSize * resolution / pointsPerInch;
        }
        /// <summary>
        /// calculate scale to target pixel size based on current typeface's UnitsPerEm
        /// </summary>
        /// <param name="targetPixelSize">target font size in point unit</param>
        /// <returns></returns>
        public float CalculateScaleToPixel(float targetPixelSize)
        {
            //1. return targetPixelSize / UnitsPerEm
            return targetPixelSize / this.UnitsPerEm;
        }
        /// <summary>
        ///  calculate scale to target pixel size based on current typeface's UnitsPerEm
        /// </summary>
        /// <param name="targetPointSize">target font size in point unit</param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public float CalculateScaleToPixelFromPointSize(float targetPointSize, int resolution = 96)
        {
            //1. var sizeInPixels = ConvPointsToPixels(sizeInPointUnit);
            //2. return  sizeInPixels / UnitsPerEm
            return (targetPointSize * resolution / pointsPerInch) / this.UnitsPerEm;
        }


        internal BASE BaseTable { get; set; }
        internal GDEF GDEFTable { get; set; }

        public COLR COLRTable { get; private set; }
        public CPAL CPALTable { get; private set; }

        internal bool HasColorAndPal { get; private set; }
        internal void SetColorAndPalTable(COLR colr, CPAL cpal)
        {
            COLRTable = colr;
            CPALTable = cpal;
            HasColorAndPal = colr != null;
        }

        public GPOS GPOSTable { get; internal set; }
        public GSUB GSUBTable { get; internal set; }

        internal void LoadOpenFontLayoutInfo(GDEF gdefTable, GSUB gsubTable, GPOS gposTable, BASE baseTable, COLR colrTable, CPAL cpalTable)
        {

            //***
            this.GDEFTable = gdefTable;
            this.GSUBTable = gsubTable;
            this.GPOSTable = gposTable;
            this.BaseTable = baseTable;
            this.COLRTable = colrTable;
            this.CPALTable = cpalTable;
            //---------------------------
            //fill glyph definition            
            if (gdefTable != null)
            {
                gdefTable.FillGlyphData(_glyphs);
            }
        }

        internal PostTable PostTable { get; set; }
        internal bool _evalCffGlyphBounds;
        public bool IsCffFont => _hasCffData;

        //Math Table

        MathGlyphs.MathGlyphInfo[] _mathGlyphInfos;
        internal MathTable _mathTable;
        //
        public MathGlyphs.MathConstants MathConsts => _mathTable?._mathConstTable;
        internal void LoadMathGlyphInfos(MathGlyphs.MathGlyphInfo[] mathGlyphInfos)
        {
            _mathGlyphInfos = mathGlyphInfos;
            if (mathGlyphInfos != null)
            {
                //fill to original glyph?
                for (int glyphIndex = 0; glyphIndex < _glyphs.Length; ++glyphIndex)
                {
                    _glyphs[glyphIndex].MathGlyphInfo = mathGlyphInfos[glyphIndex];
                }
            }
        }
        public MathGlyphs.MathGlyphInfo GetMathGlyphInfo(ushort glyphIndex) => _mathGlyphInfos[glyphIndex];

        //-------------------------
        //svg and bitmap font
        SvgTable _svgTable;

        internal bool HasSvgTable { get; private set; }
        internal void SetSvgTable(SvgTable svgTable)
        {
            HasSvgTable = (_svgTable = svgTable) != null;
        }
        public void ReadSvgContent(ushort glyphIndex, System.Text.StringBuilder output) => _svgTable?.ReadSvgContent(glyphIndex, output);


        internal BitmapFontGlyphSource _bitmapFontGlyphSource;
        public bool IsBitmapFont => _bitmapFontGlyphSource != null;
        public void ReadBitmapContent(Glyph glyph, System.IO.Stream output)
        {
            _bitmapFontGlyphSource.CopyBitmapContent(glyph, output);
        }

        /// <summary>
        /// undate lang info
        /// </summary>
        /// <param name="metaTable"></param>
        internal void UpdateLangs(Meta metaTable) => Languages.Update(OS2Table, metaTable, this.GSUBTable, this.GPOSTable);



    }

    public interface IGlyphPositions
    {
        int Count { get; }

        GlyphClassKind GetGlyphClassKind(int index);
        void AppendGlyphOffset(int index, short appendOffsetX, short appendOffsetY);
        void AppendGlyphAdvance(int index, short appendAdvX, short appendAdvY);

        ushort GetGlyph(int index, out ushort advW);
        ushort GetGlyph(int index, out ushort inputOffset, out short offsetX, out short offsetY, out short advW);
        //
        void GetOffset(int index, out short offsetX, out short offsetY);
    }


    public static class StringUtils
    {
        public static void FillWithCodepoints(List<int> codepoints, char[] str, int startAt = 0, int len = -1)
        {

            if (len == -1) len = str.Length;
            // this is important!
            // -----------------------
            //  from @samhocevar's PR: (https://github.com/LayoutFarm/Typography/pull/56/commits/b71c7cf863531ebf5caa478354d3249bde40b96e)
            // In many places, "char" is not a valid type to handle characters, because it
            // only supports 16 bits.In order to handle the full range of Unicode characters,
            // we need to use "int".
            // This allows characters such as 🙌 or 𐐷 or to be treated as single codepoints even
            // though they are encoded as two "char"s in a C# string.
            for (int i = 0; i < len; ++i)
            {
                char ch = str[startAt + i];
                int codepoint = ch;
                if (char.IsHighSurrogate(ch) && i + 1 < len)
                {
                    char nextCh = str[startAt + i + 1];
                    if (char.IsLowSurrogate(nextCh))
                    {
                        ++i;
                        codepoint = char.ConvertToUtf32(ch, nextCh);
                    }
                }
                codepoints.Add(codepoint);
            }
        }
        public static IEnumerable<int> GetCodepoints(char[] str, int startAt = 0, int len = -1)
        {
            if (len == -1) len = str.Length;
            // this is important!
            // -----------------------
            //  from @samhocevar's PR: (https://github.com/LayoutFarm/Typography/pull/56/commits/b71c7cf863531ebf5caa478354d3249bde40b96e)
            // In many places, "char" is not a valid type to handle characters, because it
            // only supports 16 bits.In order to handle the full range of Unicode characters,
            // we need to use "int".
            // This allows characters such as 🙌 or 𐐷 or to be treated as single codepoints even
            // though they are encoded as two "char"s in a C# string.
            for (int i = 0; i < len; ++i)
            {
                char ch = str[startAt + i];
                int codepoint = ch;
                if (char.IsHighSurrogate(ch) && i + 1 < len)
                {
                    char nextCh = str[startAt + i + 1];
                    if (char.IsLowSurrogate(nextCh))
                    {
                        ++i;
                        codepoint = char.ConvertToUtf32(ch, nextCh);
                    }
                }
                yield return codepoint;
            }
        }
    }


    namespace Extensions
    {

        public static class TypefaceExtensions
        {
            public static bool RecommendToUseTypoMetricsForLineSpacing(this Typeface typeface)
            {
                //https://www.microsoft.com/typography/otspec/os2.htm
                //
                //fsSelection ...
                //
                //bit     name                
                //7       USE_TYPO_METRICS   
                //  
                //        Description
                //        If set, it is strongly recommended to use
                //        OS/2.sTypoAscender - OS/2.sTypoDescender + OS/2.sTypoLineGap 
                //        as a value for default line spacing for this font.

                return ((typeface.OS2Table.fsSelection >> 7) & 1) != 0;
            }
            public static TranslatedOS2FontStyle TranslatedOS2FontStyle(this Typeface typeface)
            {
                return TranslatedOS2FontStyle(typeface.OS2Table);
            }

            internal static TranslatedOS2FontStyle TranslatedOS2FontStyle(OS2Table os2Table)
            {
                //@prepare's note, please note:=> this is not real value, this is 'translated' value from OS2.fsSelection 


                //https://www.microsoft.com/typography/otspec/os2.htm
                //Bit # 	macStyle bit 	C definition 	Description
                //0         bit 1           ITALIC          Font contains italic or oblique characters, otherwise they are upright.
                //1                         UNDERSCORE      Characters are underscored.
                //2                         NEGATIVE        Characters have their foreground and background reversed.
                //3                         OUTLINED        Outline(hollow) characters, otherwise they are solid.
                //4                         STRIKEOUT       Characters are overstruck.
                //5         bit 0           BOLD            Characters are emboldened.
                //6                         REGULAR Characters are in the standard weight / style for the font.
                //7                         USE_TYPO_METRICS    If set, it is strongly recommended to use OS / 2.sTypoAscender - OS / 2.sTypoDescender + OS / 2.sTypoLineGap as a value for default line spacing for this font.
                //8                         WWS     The font has ‘name’ table strings consistent with a weight / width / slope family without requiring use of ‘name’ IDs 21 and 22. (Please see more detailed description below.)
                //9                         OBLIQUE     Font contains oblique characters.
                //10–15 < reserved > Reserved; set to 0.
                ushort fsSelection = os2Table.fsSelection;
                TranslatedOS2FontStyle result = Extensions.TranslatedOS2FontStyle.UNSET;

                if ((fsSelection & 0x1) != 0)
                {

                    result |= Extensions.TranslatedOS2FontStyle.ITALIC;
                }

                if (((fsSelection >> 5) & 0x1) != 0)
                {
                    result |= Extensions.TranslatedOS2FontStyle.BOLD;
                }

                if (((fsSelection >> 6) & 0x1) != 0)
                {
                    result |= Extensions.TranslatedOS2FontStyle.REGULAR;
                }
                if (((fsSelection >> 9) & 0x1) != 0)
                {
                    result |= Extensions.TranslatedOS2FontStyle.OBLIQUE;
                }

                return result;
            }


            /// <summary>
            /// overall calculated line spacing 
            /// </summary>
            static int Calculate_TypoMetricLineSpacing(Typeface typeface)
            {

                //from https://www.microsoft.com/typography/OTSpec/recom.htm#tad
                //sTypoAscender, sTypoDescender and sTypoLineGap
                //sTypoAscender is used to determine the optimum offset from the top of a text frame to the first baseline.
                //sTypoDescender is used to determine the optimum offset from the last baseline to the bottom of the text frame. 
                //The value of (sTypoAscender - sTypoDescender) is recommended to equal one em.
                //
                //While the OpenType specification allows for CJK (Chinese, Japanese, and Korean) fonts' sTypoDescender and sTypoAscender 
                //fields to specify metrics different from the HorizAxis.ideo and HorizAxis.idtp baselines in the 'BASE' table,
                //CJK font developers should be aware that existing applications may not read the 'BASE' table at all but simply use 
                //the sTypoDescender and sTypoAscender fields to describe the bottom and top edges of the ideographic em-box. 
                //If developers want their fonts to work correctly with such applications, 
                //they should ensure that any ideographic em-box values in the 'BASE' table describe the same bottom and top edges as the sTypoDescender and
                //sTypoAscender fields. 
                //See the sections “OpenType CJK Font Guidelines“ and ”Ideographic Em-Box“ for more details.

                //For Western fonts, the Ascender and Descender fields in Type 1 fonts' AFM files are a good source of sTypoAscender
                //and sTypoDescender, respectively. 
                //The Minion Pro font family (designed on a 1000-unit em), 
                //for example, sets sTypoAscender = 727 and sTypoDescender = -273.

                //sTypoAscender, sTypoDescender and sTypoLineGap specify the recommended line spacing for single-spaced horizontal text.
                //The baseline-to-baseline value is expressed by:
                //OS/2.sTypoAscender - OS/2.sTypoDescender + OS/2.sTypoLineGap




                //sTypoLineGap will usually be set by the font developer such that the value of the above expression is approximately 120% of the em.
                //The application can use this value as the default horizontal line spacing. 
                //The Minion Pro font family (designed on a 1000-unit em), for example, sets sTypoLineGap = 200.


                return typeface.Ascender - typeface.Descender + typeface.LineGap;

            }

            /// <summary>
            /// calculate Baseline-to-Baseline Distance (BTBD) for Windows
            /// </summary>
            /// <param name="typeface"></param>
            /// <returns>return 'unscaled-to-pixel' BTBD value</returns>
            static int Calculate_BTBD_Windows(Typeface typeface)
            {

                //from https://www.microsoft.com/typography/otspec/recom.htm#tad

                //Baseline to Baseline Distances
                //The 'OS/2' table fields sTypoAscender, sTypoDescender, and sTypoLineGap 
                //free applications from Macintosh-or Windows - specific metrics
                //which are constrained by backward compatibility requirements.
                //
                //The following discussion only pertains to the platform-specific metrics.
                //The suggested Baseline to Baseline Distance(BTBD) is computed differently for Windows and the Macintosh,
                //and it is based on different OpenType metrics.
                //However, if the recommendations below are followed, the BTBD will be the same for both Windows and the Mac.

                //Windows Metric         OpenType Metric
                //ascent                    usWinAscent
                //descent                   usWinDescent
                //internal leading          usWinAscent + usWinDescent - unitsPerEm
                //external leading          MAX(0, LineGap - ((usWinAscent + usWinDescent) - (Ascender - Descender)))

                //The suggested BTBD = ascent + descent + external leading

                //It should be clear that the “external leading” can never be less than zero. 
                //Pixels above the ascent or below the descent will be clipped from the character; 
                //this is true for all output devices.

                //The usWinAscent and usWinDescent are values 
                //from the 'OS/2' table.
                //The unitsPerEm value is from the 'head' table.
                //The LineGap, Ascender and Descender values are from the 'hhea' table.

                int usWinAscent = typeface.OS2Table.usWinAscent;
                int usWinDescent = typeface.OS2Table.usWinDescent;
                int internal_leading = usWinAscent + usWinDescent - typeface.UnitsPerEm;
                HorizontalHeader hhea = typeface.HheaTable;
                int external_leading = System.Math.Max(0, hhea.LineGap - ((usWinAscent + usWinDescent) - (hhea.Ascent - hhea.Descent)));
                return usWinAscent + usWinDescent + external_leading;
            }
            /// <summary>
            /// calculate Baseline-to-Baseline Distance (BTBD) for macOS
            /// </summary>
            /// <param name="typeface"></param>
            /// <returns>return 'unscaled-to-pixel' BTBD value</returns>
            static int CalculateBTBD_Mac(Typeface typeface)
            {
                //from https://www.microsoft.com/typography/otspec/recom.htm#tad

                //Ascender and Descender are metrics defined by Apple 
                //and are not to be confused with the Windows ascent or descent, 
                //nor should they be confused with the true typographic ascender and descender that are found in AFM files.
                //The Macintosh metrics below are returned by the Apple Advanced Typography(AAT) GetFontInfo() API.
                //
                //
                //Macintosh Metric      OpenType Metric
                //ascender                  Ascender
                //descender                 Descender
                //leading                   LineGap

                //The suggested BTBD = ascent + descent + leading
                //If pixels extend above the ascent or below the descent, 
                //the character will be squashed in the vertical direction 
                //so that all pixels fit within these limitations; this is true for screen display only.

                //TODO: please test this
                HorizontalHeader hhea = typeface.HheaTable;
                return hhea.Ascent + hhea.Descent + hhea.LineGap;
            }


            public static int CalculateRecommendLineSpacing(this Typeface typeface, out LineSpacingChoice choice)
            {

                //from https://docs.microsoft.com/en-us/typography/opentype/spec/os2#wa
                //usWinAscent
                //Format: 	uint16
                //Description: 
                //The “Windows ascender” metric. 
                //This should be used to specify the height above the baseline for a clipping region.

                //This is similar to the sTypoAscender field, 
                //and also to the ascender field in the 'hhea' table.
                //There are important differences between these, however.

                //In the Windows GDI implementation, 
                //the usWinAscent and usWinDescent values have been used to determine
                //the size of the bitmap surface in the TrueType rasterizer.
                //Windows GDI will clip any portion of a TrueType glyph outline that appears above the usWinAscent value.
                //If any clipping is unacceptable, then the value should be set greater than or equal to yMax.

                //Note: This pertains to the default position of glyphs,
                //not their final position in layout after data from the GPOS or 'kern' table has been applied.
                //Also, this clipping behavior also interacts with the VDMX table:
                //if a VDMX table is present and there is data for the current device aspect ratio and rasterization size,
                //then the VDMX data will supersede the usWinAscent and usWinDescent values.

                //****
                //Some legacy applications use the usWinAscent and usWinDescent values to determine default line spacing.
                //This is **strongly discouraged**. The sTypo* fields should be used for this purpose.

                //Note that some applications use either the usWin* values or the sTypo* values to determine default line spacing,
                //depending on whether the USE_TYPO_METRICS flag (bit 7) of the fsSelection field is set.
                //This may be useful to provide **compatibility with legacy documents using older fonts**,
                //while also providing better and more-portable layout using newer fonts. 
                //See fsSelection for additional details.

                //Applications that use the sTypo* fields for default line spacing can use the usWin* 
                //values to determine the size of a clipping region. 
                //Some applications use a clipping region for editing scenarios to determine what portion of the display surface to re-draw when text is edited, or how large a selection rectangle to draw when text is selected. This is an appropriate use for the usWin* values.

                //Early versions of this specification suggested that the usWinAscent value be computed as the yMax 
                //for all characters in the Windows “ANSI” character set. 

                //For new fonts, the value should be determined based on the primary languages the font is designed to support,
                //and **should take into consideration additional height that may be required to accommodate tall glyphs or mark positioning.*** 

                //-----------------------------------------------------------------------------------
                //usWinDescent
                //Format: 	uint16
                //Description:
                //The “Windows descender” metric.This should be used to specify the vertical extent
                //below the baseline for a clipping region.

                //This is similar to the sTypoDescender field,
                //and also to the descender field in the 'hhea' table.

                //***
                //There are important differences between these, however.
                //Some of these differences are described below.
                //In addition, the usWinDescent value treats distances below the baseline as positive values;
                //thus, usWinDescent is usually a positive value, while sTypoDescender and hhea.descender are usually negative.

                //In the Windows GDI implementation,
                //the usWinDescent and usWinAscent values have been used 
                //to determine the size of the bitmap surface in the TrueType rasterizer.
                //Windows GDI will clip any portion of a TrueType glyph outline that appears below(-1 × usWinDescent). 
                //If any clipping is unacceptable, then the value should be set greater than or equal to(-yMin).

                //Note: This pertains to the default position of glyphs,
                //not their final position in layout after data from the GPOS or 'kern' table has been applied.
                //Also, this clipping behavior also interacts with the VDMX table:
                //if a VDMX table is present and there is data for the current device aspect ratio and rasterization size,
                //***then the VDMX data will supersede the usWinAscent and usWinDescent values.****
                //-----------------------------------------------------------------------------------

                //so ...
                choice = LineSpacingChoice.TypoMetric;
                return Calculate_TypoMetricLineSpacing(typeface);

                //if (RecommendToUseTypoMetricsForLineSpacing(typeface))
                //{
                //    choice = LineSpacingChoice.TypoMetric;
                //    return Calculate_TypoMetricLineSpacing(typeface);
                //}
                //else
                //{
                //    //check if we are on Windows or mac 
                //    if (CurrentEnv.CurrentOSName == CurrentOSName.Mac)
                //    {
                //        choice = LineSpacingChoice.Mac;
                //        return CalculateBTBD_Mac(typeface);
                //    }
                //    else
                //    {
                //        choice = LineSpacingChoice.Windows;
                //        return Calculate_BTBD_Windows(typeface);
                //    }
                //}

            }
            public static int CalculateRecommendLineSpacing(this Typeface typeface)
            {
                return CalculateMaxLineClipHeight(typeface);
                //return CalculateRecommendLineSpacing(typeface, out var _);
            }
            public static int CalculateLineSpacing(this Typeface typeface, LineSpacingChoice choice)
            {
                switch (choice)
                {
                    default:
                    case LineSpacingChoice.Windows:
                        return Calculate_BTBD_Windows(typeface);
                    case LineSpacingChoice.Mac:
                        return CalculateBTBD_Mac(typeface);
                    case LineSpacingChoice.TypoMetric:
                        return Calculate_TypoMetricLineSpacing(typeface);
                }
            }
            public static int CalculateMaxLineClipHeight(this Typeface typeface)
            {
                //TODO: review here
                return typeface.OS2Table.usWinAscent + typeface.OS2Table.usWinDescent;
            }


        }
        public enum LineSpacingChoice
        {
            TypoMetric,
            Windows,
            Mac
        }
        public enum CurrentOSName
        {
            None,//not evaluate yet
            Windows,
            Mac,
            Others
        }


        [System.Flags]
        public enum TranslatedOS2FontStyle : ushort
        {

            //@prepare's note, please note:=> this is not real value, this is 'translated' value from OS2.fsSelection 

            UNSET = 0,

            ITALIC = 1,
            BOLD = 1 << 1,
            REGULAR = 1 << 2,
            OBLIQUE = 1 << 3,
        }

        public static class CurrentEnv
        {
            public static CurrentOSName CurrentOSName;
        }
    }


    public struct GlyphNameMap
    {
        public readonly ushort glyphIndex;
        public readonly string glyphName;
        public GlyphNameMap(ushort glyphIndex, string glyphName)
        {
            this.glyphIndex = glyphIndex;
            this.glyphName = glyphName;
        }
    }





    public static class TypefaceExtension2
    {

        public static bool HasMathTable(this Typeface typeface) => typeface.MathConsts != null;

        public static bool HasSvgTable(this Typeface typeface) => typeface.HasSvgTable;

        public static bool HasColorTable(this Typeface typeface) => typeface.HasColorAndPal;


        class CffBoundFinder : IGlyphTranslator
        {

            float _minX, _maxX, _minY, _maxY;
            float _curX, _curY;
            float _latestMove_X, _latestMove_Y;
            /// <summary>
            /// curve flatten steps  => this a copy from Typography.Contours's GlyphPartFlattener
            /// </summary>
            int _nsteps = 3;
            bool _contourOpen = false;
            bool _first_eval = true;
            public CffBoundFinder()
            {

            }
            public void Reset()
            {
                _curX = _curY = _latestMove_X = _latestMove_Y = 0;
                _minX = _minY = float.MaxValue;//**
                _maxX = _maxY = float.MinValue;//**
                _first_eval = true;
                _contourOpen = false;
            }
            public void BeginRead(int contourCount)
            {

            }
            public void EndRead()
            {

            }
            public void CloseContour()
            {
                _contourOpen = false;
                _curX = _latestMove_X;
                _curY = _latestMove_Y;
            }
            public void Curve3(float x1, float y1, float x2, float y2)
            {

                //this a copy from Typography.Contours -> GlyphPartFlattener

                float eachstep = (float)1 / _nsteps;
                float t = eachstep;//start

                for (int n = 1; n < _nsteps; ++n)
                {
                    float c = 1.0f - t;

                    UpdateMinMax(
                         (c * c * _curX) + (2 * t * c * x1) + (t * t * x2),  //x
                         (c * c * _curY) + (2 * t * c * y1) + (t * t * y2)); //y

                    t += eachstep;
                }

                //
                UpdateMinMax(
                    _curX = x2,
                    _curY = y2);

                _contourOpen = true;
            }

            public void Curve4(float x1, float y1, float x2, float y2, float x3, float y3)
            {

                //this a copy from Typography.Contours -> GlyphPartFlattener


                float eachstep = (float)1 / _nsteps;
                float t = eachstep;//start

                for (int n = 1; n < _nsteps; ++n)
                {
                    float c = 1.0f - t;

                    UpdateMinMax(
                        (_curX * c * c * c) + (x1 * 3 * t * c * c) + (x2 * 3 * t * t * c) + x3 * t * t * t,  //x
                        (_curY * c * c * c) + (y1 * 3 * t * c * c) + (y2 * 3 * t * t * c) + y3 * t * t * t); //y

                    t += eachstep;
                }
                //
                UpdateMinMax(
                    _curX = x3,
                    _curY = y3);

                _contourOpen = true;
            }
            public void LineTo(float x1, float y1)
            {
                UpdateMinMax(
                    _curX = x1,
                    _curY = y1);

                _contourOpen = true;
            }
            public void MoveTo(float x0, float y0)
            {

                if (_contourOpen)
                {
                    CloseContour();
                }

                UpdateMinMax(
                    _curX = x0,
                    _curY = y0);
            }
            void UpdateMinMax(float x0, float y0)
            {

                if (_first_eval)
                {
                    //4 times

                    if (x0 < _minX)
                    {
                        _minX = x0;
                    }
                    //
                    if (x0 > _maxX)
                    {
                        _maxX = x0;
                    }
                    //
                    if (y0 < _minY)
                    {
                        _minY = y0;
                    }
                    //
                    if (y0 > _maxY)
                    {
                        _maxY = y0;
                    }

                    _first_eval = false;
                }
                else
                {
                    //2 times

                    if (x0 < _minX)
                    {
                        _minX = x0;
                    }
                    else if (x0 > _maxX)
                    {
                        _maxX = x0;
                    }

                    if (y0 < _minY)
                    {
                        _minY = y0;
                    }
                    else if (y0 > _maxY)
                    {
                        _maxY = y0;
                    }
                }

            }

            public Bounds GetResultBounds()
            {
                return new Bounds(
                    (short)System.Math.Floor(_minX),
                    (short)System.Math.Floor(_minY),
                    (short)System.Math.Ceiling(_maxX),
                    (short)System.Math.Ceiling(_maxY));
            }
        }
        public static void UpdateAllCffGlyphBounds(this Typeface typeface)
        {
            //TODO: review here again,

            if (typeface.IsCffFont && !typeface._evalCffGlyphBounds)
            {
                int j = typeface.GlyphCount;
                CFF.CffEvaluationEngine evalEngine = new CFF.CffEvaluationEngine();
                CffBoundFinder boundFinder = new CffBoundFinder();
                for (ushort i = 0; i < j; ++i)
                {
                    Glyph g = typeface.GetGlyph(i);
                    boundFinder.Reset();

                    evalEngine.Run(boundFinder, g._cff1GlyphData.GlyphInstructions);

                    g.Bounds = boundFinder.GetResultBounds();
                }
                typeface._evalCffGlyphBounds = true;
            }
        }
    }





}




