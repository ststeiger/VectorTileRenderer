
namespace VectorTileRenderer
{


    public class NotoFontManager
        : FontManager
    {

        protected readonly static string[] PATTERNS = { "*.ttf", "*.otf" };


        protected string[] explicityAllowed = new string[] {
            // "NotoSansSymbols2-Regular", 
        };


        // https://en.wikipedia.org/wiki/List_of_writing_systems#List_of_writing_systems_by_adoption
        // https://en.wikipedia.org/wiki/List_of_languages_by_number_of_native_speakers_in_India
        protected string[] priority = new string[] {
            "NotoSans-Regular.ttf", // Latin & Cyrillic & Greek 

            "NotoSansCJKsc-Regular.otf", // Red-Chinese 
            "NotoSansCJKtc-Regular.otf", // Blue-Chinese

            "NotoSansArabic-Regular.ttf", // Arabic 660+ millino
            "NotoSansTifinagh-Regular.ttf", // Tifinagh, 1 million, Let's speed up Morocco/Berber rendering

            "NotoSansDevanagari-Regular.ttf", // 608 Million 
            "NotoSansBengali-Regular.ttf", // 210 million, of which 100 in Bangladesh 

            "NotoSansCJKjp-Regular.otf", // 120 million 
            "NotoSansCJKkr-Regular.otf", // Hangul 78.8 million 

            "NotoSansTelugu-Regular.ttf", // 74 million 
            "NotoSansTamil-Regular.ttf", // 70 million 
            "NotoSansGujarati-Regular.ttf", // 48 million
            "NotoSansKannada-Regular.ttf", //45 million
            "NotoSansMyanmar-Regular.ttf", // Burmese, 39 million 
            "NotoSansMalayalam-Regular.ttf", // 38 Million

            "NotoSansThai-Regular.ttf", // 38 million 
            "NotoSansTaiViet-Regular.ttf", // 38 million  ?
            "NotoSansLao-Regular.ttf", // Laos 22 million 
            "NotoSansEthiopic-Regular.ttf", // Ge'ez/Ethiopia, 18 million 
            "NotoSansSinhala-Regular.ttf", // 14.4 million 
            "NotoSansHebrew-Regular.ttf", // 14 million 
            "NotoSansArmenian-Regular.ttf", // Armenian, 12 million
            "NotoSansKhmer-Regular.ttf", // Cambodia, 11.4 million 
            "NotoSansTibetan-Regular.ttf", // Tibetan, 5 million 

            "NotoSansGeorgian-Regular.ttf", // Georgian 4.5 million 

            "NotoSansYi-Regular.ttf", // Yi, 2 million
            "NotoSansMongolian-Regular.ttf", // 2 million 
            "NotoSansTagalog-Regular.ttf", // Philippines
            "NotoSansBalinese-Regular.ttf", // Bali

            // Syria 0.4 million 
            "NotoSansSyriacEastern-Regular.ttf",
            "NotoSansSyriacEstrangela-Regular.ttf",
            "NotoSansSyriacWestern-Regular.ttf",

            "NotoSansEgyptianHieroglyphs-Regular.ttf",
            "NotoSansPhoenician-Regular.ttf",
        };


        protected string[] excludes = new string[] {
            "NotoEmoji-Regular",
            "NotoSansSymbols2-Regular"
        };

        protected string[] buggy = new string[] {
                "NotoSansAvestan-Regular",
                "NotoSansBamum-Regular",
                "NotoSansCoptic-Regular",
                "NotoSansGlagolitic-Regular",
                "NotoSansNewTaiLue-Regular",
                "NotoSansOsage-Regular",
                "NotoSansSymbols-Regular",

                "NotoSansSymbols-Black",
                "NotoSansSymbols-Bold",
                "NotoSansSymbols-ExtraBold",
                "NotoSansSymbols-ExtraLight",
                "NotoSansSymbols-Light",
                "NotoSansSymbols-Medium",
                "NotoSansSymbols-SemiBold",
                "NotoSansSymbols-Thin",
                "NotoKufiArabic-Bold",
            };


        public NotoFontManager()
            : base()
        { }



        public NotoFontManager(System.Collections.Generic.IEnumerable<string> sourceDirectories)
            : base(sourceDirectories, PATTERNS)
        { }


        public NotoFontManager(string sourceDirectory)
            : base(new string[] { sourceDirectory }, PATTERNS)
        { }




        protected override bool IncludeFont(string fileName)
        {
            if (-1 != System.Array.FindIndex(buggy, item => string.Equals(item, fileName, System.StringComparison.InvariantCultureIgnoreCase))) return false;

            if (-1 != System.Array.FindIndex(explicityAllowed, item => string.Equals(item, fileName, System.StringComparison.InvariantCultureIgnoreCase))) return true;

            if (!fileName.EndsWith("-Regular", System.StringComparison.InvariantCultureIgnoreCase)) return false;
            // if (!fileName.EndsWith("-Italic", System.StringComparison.InvariantCultureIgnoreCase)) return false;
            // if (!fileName.EndsWith("-Bold", System.StringComparison.InvariantCultureIgnoreCase)) return false;
            // if (!fileName.EndsWith("-BoldItalic", System.StringComparison.InvariantCultureIgnoreCase)) return false;

            if (-1 != System.Array.FindIndex(excludes, item => string.Equals(item, fileName, System.StringComparison.InvariantCultureIgnoreCase))) return false;



            if (fileName.IndexOf("Serif", System.StringComparison.InvariantCultureIgnoreCase) != -1) return false;
            if (fileName.IndexOf("Mono", System.StringComparison.InvariantCultureIgnoreCase) != -1) return false;
            if (fileName.IndexOf("UI-", System.StringComparison.InvariantCultureIgnoreCase) != -1) return false;



            return true;
        }

        public override void SortByPriority()
        {
            this.FontList.Sort(
                delegate (FontInfo x, FontInfo y)
                {
                    if (string.IsNullOrEmpty(x.FileName))
                        return 1;

                    if (string.IsNullOrEmpty(y.FileName))
                        return -1;

                    int priority1 = System.Array.FindIndex(this.priority, item => string.Equals(item, x.FileName, System.StringComparison.InvariantCultureIgnoreCase));
                    int priority2 = System.Array.FindIndex(this.priority, item => string.Equals(item, y.FileName, System.StringComparison.InvariantCultureIgnoreCase));

                    if (priority1 == -1 && priority2 == -1)
                    {
                        return x.FileName.CompareTo(y.FileName);
                    }

                    if (priority1 == -1)
                        priority1 = int.MaxValue;

                    if (priority2 == -1)
                        priority2 = int.MaxValue;

                    return priority1.CompareTo(priority2);

                    // int sortOrder = 1;
                    // if (string.Equals(x.FileName, "NotoSans-Regular.ttf"))
                    //     return sortOrder*-1;

                    // if (string.Equals(y.FileName, "NotoSans-Regular.ttf"))
                    //     return sortOrder * 1;

                    // return x.FileName.CompareTo(y.FileName); 
                }
            );
            
            // string[] fn = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Select(this.FontList, x => x.FileName));
            // string namez = string.Join(System.Environment.NewLine, fn);
            // System.Console.WriteLine(namez);

        } // End Sub SortByPriority 


    } // End Class NotoFontManager 


} // End Namespace 
