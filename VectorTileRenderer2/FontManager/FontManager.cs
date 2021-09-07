
namespace VectorTileRenderer
{


    public abstract class FontManager
    {
        protected Typography.OpenFont.OpenFontReader m_fontReader;
        public System.Collections.Generic.List<FontInfo> FontList;
        public System.Collections.Generic.List<string> SourceDirectories;


        public FontManager()
        {
            this.m_fontReader = new Typography.OpenFont.OpenFontReader();
            this.FontList = new System.Collections.Generic.List<FontInfo>();
            this.SourceDirectories = new System.Collections.Generic.List<string>();
        } // End Constructor 


        public FontManager(System.Collections.Generic.IEnumerable<string> sourceDirectories, string[] validPatterns)
            :this()
        {
            this.SourceDirectories.AddRange(sourceDirectories);
            this.AddSourceDirectories(this.SourceDirectories, validPatterns);
        } // End Constructor 


        public FontManager(string sourceDirectory, params string[] validPatterns)
            : this(new string[] { sourceDirectory }, validPatterns)
        { }


        public virtual FontManager AddSourceDirectories(System.Collections.Generic.IEnumerable<string> paths, params string[] validPatterns)
        {
            foreach (string path in paths)
            {
                foreach (string thisPattern in validPatterns)
                {
                    this.LoadFontsFromPath(path, thisPattern);
                } // Next thisPattern
                  // 
            } // Next path 

            this.SortByPriority();
            return this;
        } // End Function AddSourceDirectories 



        public virtual FontManager AddSourceDirectory(string path, params string[] validPatterns)
        {
            return AddSourceDirectories(new string[] { path }, validPatterns);
        } // End Function AddSourceDirectory 


        public virtual FontManager AddFont(params Typography.OpenFont.Typeface[] fonts)
        {
            for (int i = 0; i < fonts.Length; ++i)
            {
                this.FontList.Add(new FontInfo(fonts[i]));
            } // Next i 

            this.SortByPriority();
            return this;
        } // End Function AddFont 





        protected virtual bool IncludeFont(string fileName)
        {
            return true;
        }



        public virtual System.Collections.Generic.List<FontInfo> LoadFontsFromPath(string path, string searchPattern)
        {
            System.Collections.Generic.List<FontInfo> ls = new System.Collections.Generic.List<FontInfo>();

            string[] files = System.IO.Directory.GetFiles(path, searchPattern, System.IO.SearchOption.AllDirectories);

            foreach (string fontFile in files)
            {
                string test = System.IO.Path.GetFileNameWithoutExtension(fontFile);

                if (!this.IncludeFont(test))
                    continue;

                try
                {
                    Typography.OpenFont.Typeface tf = this.TypefaceFromFile(fontFile);
                    ls.Add(new FontInfo(tf));
                    this.FontList.Add(new FontInfo(tf));
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(fontFile);
                    // System.Console.WriteLine(ex.Message);
                    // System.Console.WriteLine(ex.StackTrace);
                    // System.Console.WriteLine(System.Environment.NewLine);
                    // System.Console.WriteLine(System.Environment.NewLine);
                }


            }

            //string[] fn = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Select(ls, x => x.Filename));
            //string namez = string.Join(System.Environment.NewLine, fn);
            //System.Console.WriteLine(namez);

            return ls;
        } // End Function LoadFontsFromPath 


        public virtual void SortByPriority()
        { }


        public virtual Typography.OpenFont.Typeface TypefaceFromFile(string fontFile)
        {
            Typography.OpenFont.Typeface tf;

            using (System.IO.FileStream fs = new System.IO.FileStream(fontFile, System.IO.FileMode.Open))
            {
                tf = this.m_fontReader.Read(fs);
            }

            return tf;
        } // End Function TypefaceFromFile 


        // From Typography.GlyphLayout\GlyphLayout.cs
        private static System.Collections.Generic.List<int> GetCodepoints(string text)
        {
            //char[] str = text.ToCharArray();
            int startAt = 0;
            int len = text.Length;


            // convert from char[] to codepoint-list
            // this is important!
            // -----------------------
            // from @samhocevar's PR: (https://github.com/LayoutFarm/Typography/pull/56/commits/b71c7cf863531ebf5caa478354d3249bde40b96e)
            // In many places, "char" is not a valid type to handle characters, because it
            // only supports 16 bits.In order to handle the full range of Unicode characters,
            // we need to use "int".
            // This allows characters such as 🙌 or 𐐷 or to be treated as single codepoints even
            // though they are encoded as two "char"s in a C# string.

            System.Collections.Generic.List<int> ls = new System.Collections.Generic.List<int>();


            for (int i = 0; i < len; ++i)
            {
                char ch = text[startAt + i];
                int codepoint = ch;
                if (ch >= 0xd800 && ch <= 0xdbff && i + 1 < len)//high surrogate
                {
                    char nextCh = text[startAt + i + 1];
                    if (nextCh >= 0xdc00 && nextCh <= 0xdfff) //low-surrogate 
                    {
                        //please note: 
                        //num of codepoint may be less than  original user input char 
                        ++i;
                        codepoint = char.ConvertToUtf32(ch, nextCh);
                    }
                }

                ls.Add(codepoint);
            }

            return ls;
        } // End Function GetCodepoints 


        // From Typography.GlyphLayout\GlyphLayout.cs
        private static bool FontSupportsString(Typography.OpenFont.Typeface tf, string text)
        {
            System.Collections.Generic.List<int> inputCodePoints = GetCodepoints(text);

            int startAt = 0;
            int len = inputCodePoints.Count;

            // convert codepoint-list to input glyph-list 
            // clear before use
            // System.Collections.Generic.List<ushort> _inputGlyphs = new System.Collections.Generic.List<ushort>();


            int end = startAt + len;
            int cur_codepoint, next_codepoint;

            for (int i = 0; i < end; ++i)
            {
                //find glyph index by specific codepoint  
                if (i + 1 < end)
                {
                    cur_codepoint = inputCodePoints[i];
                    next_codepoint = inputCodePoints[i + 1];
                }
                else
                {
                    cur_codepoint = inputCodePoints[i];
                    next_codepoint = 0;
                }

                ushort glyphIndex = 0;
                bool skipNextCodepoint;
                try
                {
                    // ushort glyphIndex = tf.GetGlyphIndex(cur_codepoint, next_codepoint, out bool skipNextCodepoint);
                    glyphIndex = tf.GetGlyphIndex(cur_codepoint, next_codepoint, out skipNextCodepoint);
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    System.Console.WriteLine(ex.StackTrace);
                    ushort myGlyphIndex = tf.GetGlyphIndex(cur_codepoint, next_codepoint, out bool skipNextCodepoin1t);
                    System.Console.WriteLine(myGlyphIndex);
                    return false;
                }
                

                if (glyphIndex == 0) // && _glyphNotFoundHandler != null)
                {
                    // System.Console.WriteLine("Glyph not found");

                    if (cur_codepoint != 10) // Ignore newLine
                        return false;
                    
                    // handle glyph not found
                    // glyphIndex = _glyphNotFoundHandler(this, cur_codepoint, next_codepoint);
                } // End if (glyphIndex == 0)  

                if (skipNextCodepoint)
                {
                    // Maybe this is a UVS sequence; in that case,
                    // ***SKIP*** the second codepoint 
                    ++i;
                } // End if (skipNextCodepoint) 

            } // Next i 

            return true;
        } // End Function FontSupportsString 


        public virtual FontInfo GetBestMatchingFont(ref string text)
        {
            foreach (FontInfo tf in this.FontList)
            {
                if (FontSupportsString(tf.OpenFont, text))
                    return tf;
            } // Next tf 

            // in this case, the string is composed of multiple languages
            // (or belongs to a language for which we don't have a font, so it couldn't be found.)
            // Now to solve the multi-language debacle, do the following: 
            // split the string by whitespace, takes the result of first split,
            // and concatenate all remaing split-results if they match the same font. 
            // Override the text to be rendered with the concatenation. 
            // To accomplish that, we have to use call-by-reference for argument text
            string[] allWords = text.Split(' ');

            if (allWords == null || allWords.Length < 1)
                return null; // No font that supports this charset 

            string newText = "";

            foreach (FontInfo tf in this.FontList)
            {
                if (FontSupportsString(tf.OpenFont, allWords[0]))
                {
                    newText = allWords[0];

                    if (allWords.Length > 1)
                    {
                        for (int i = 1; i < allWords.Length; ++i)
                        {
                            if (FontSupportsString(tf.OpenFont, allWords[i]))
                                newText += " " + allWords[i];
                            else
                                break;
                        } // Next i 

                    } // End if (allWords.Length > 1) 

                    text = newText;
                    return tf;
                } // End if (FontSupportsString(tf.OpenFont, allWords[0])) 

            } // Next tf 

            // TODO: 
            // Create ScriptRuns from Text, and return list of ScriptRuns 
            // then handle drawing each ScriptRun 
            // that way, there can be multiple writing systems (scripts) in each line. 

            return null; // No font that supports this charset 
        } // End Function GetBestMatchingFont 


    } // End Class FontManager 


} // End Namespace 
