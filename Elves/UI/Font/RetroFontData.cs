using System.Collections.Generic;

namespace Elves.UI.Font {
    public static partial class Fonts {
        private static Dictionary<char,Glyph> GetRetroFontData() {
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

            AddRow(2,2,10,0,
                ('A', 6),('B', 6),('C', 5),('D', 6),('E', 5),
                ('F', 5),('G', 6),('H', 6),('I', 4),('J', 4),
                ('K', 6),('L', 4),('M', 10),('N', 6)
            );
            AddRow(2,14,10,0,
                ('O', 6),('P', 6),('Q', 7),('R', 6),('S', 5),('T', 6),
                ('U', 6),('V', 6),('W', 10),('X', 6),('Y', 6),('Z', 6)
            );
            AddRow(2,26,10,0,
                ('0', 6),('1', 4),('2', 6),('3', 6),('4', 7),('5', 6),('6', 6),
                ('7', 6),('8', 6),('9', 6)
            );

            AddRow(2,38,10,0,
                ('/',4),('\\', 4),('+', 6),('-', 6),('=', 6),('%', 8),
                ('(',3),(')',3),('[', 3),(']', 3),('!',4),
                ('?',6),('*',4),('<',4),('>',4)
            );

            AddRow(80,25,12,-1,
                ('.', 2),(',', 2),('\'', 2),(':', 2),(';', 2),('\"', 5)
            );

            return glyphDictionary;
        }
    }
}
