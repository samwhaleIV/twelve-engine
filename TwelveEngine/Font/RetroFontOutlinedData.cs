using System.Collections.Generic;

namespace TwelveEngine.Font {
    public static partial class Fonts {
        private static Dictionary<char,Glyph> GetRetroFontOutlinedData() {
            var glyphDictionary = new Dictionary<char,Glyph>();

            void AddRow(int x,int y,int height,int yOffset,params (char Value, int Width)[] characters) {
                foreach(var (Value, Width) in characters) {
                    char lowercase = char.ToLowerInvariant(Value);

                    Glyph glyph = new(x,y,Width,height,yOffset);
                    x += 2 + Width;

                    if(lowercase != Value) {
                        glyphDictionary.Add(lowercase,glyph);
                    }
                    glyphDictionary.Add(Value,glyph);
                }
            }

            AddRow(3,3,22,0,
                ('A', 14),('B', 14),('C', 12),('D', 14),('E', 12),
                ('F', 12),('G', 14),('H', 14),('I', 10),('J', 10),
                ('K', 14),('L', 10),('M', 22),('N', 14)
            );
            AddRow(3,27,22,0,
                ('O', 14),('P', 14),('Q', 16),('R', 14),('S', 12),('T', 14),
                ('U', 14),('V', 14),('W', 22),('X', 14),('Y', 14),('Z', 14)
            );
            AddRow(3,51,22,0,
                ('0', 14),('1', 10),('2', 14),('3', 14),('4', 16),('5', 14),('6', 14),
                ('7', 14),('8', 14),('9', 14)
            );

            AddRow(3,75,22,0,
                ('/', 10),('\\', 10),('+', 14),('-', 14),('=', 14),('%', 18),
                ('(', 8),(')', 8),('[', 8),(']', 8),('!', 10),
                ('?', 14),('*', 10),('<', 10),('>', 10)
            );

            AddRow(159,49,26,-1,
                ('.', 6),(',', 6),('\'', 6),(':', 6),(';', 6),('\"', 12)
            );

            return glyphDictionary;
        }
    }
}
