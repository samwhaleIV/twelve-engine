using System.Collections.Generic;

namespace Elves.UI.Font {
    public static partial class Fonts {

        private static Dictionary<char,Glyph> GetUIFontData() {
            var glyphDictionary = new Dictionary<char,Glyph>();
            
            void AddRow(int x,int y,int height,int yOffset,params (char Value,int Width)[] characters) {
                foreach(var (Value, Width) in characters) {
                    glyphDictionary.Add(Value,new Glyph(x,y,Width,height,yOffset));
                    x += 2 + Width;
                }
            }

            AddRow(2,2,34,0,
                ('A', 21),('B', 18),('C', 18),('D', 18),('E', 14),('F', 14),('G', 21),('H', 18),('I', 10),
                ('J', 15),('K', 15),('L', 10),('M', 23),('N', 18),('O', 18),('P', 16),('Q', 18),('R', 18),
                ('S', 18),('T', 18),('U', 20),('V', 18),('W', 28),('X', 18),('Y', 18),('Z', 18),('1', 15),
                ('2', 18),('3', 18),('4', 20),('5', 18),('6', 18),('7', 18),('8', 18),('9', 18),('0', 18)
            );

            AddRow(2,53,19,15,
                ('a', 16),('c', 12),('e', 16),('m', 21),('n', 13),('o', 14),('r', 12),('s', 12),('u', 13),('v', 14),('w', 20),('x', 13),('z', 14)
            );

            glyphDictionary.Add('t',new Glyph(218,45,12,27,7));
            glyphDictionary.Add('i',new Glyph(232,47,4,25,9));

            AddRow(238,38,34,0,('b', 14),('d', 14),('f', 15),('k', 12),('l', 4),('h', 14));

            AddRow(323,53,30,15,('g', 14),('p', 14),('q', 16),('y', 14));

            glyphDictionary.Add('j',new Glyph(389,47,14,36,9));

            AddRow(405,38,34,0,('!', 6),('?', 18),('(', 10),(')', 10),('[', 10),(']', 10),('/', 12),('\\', 12));

            AddRow(509,45,20,7,('+', 14),('-', 14),('=', 14),('<', 8),('>', 8),('|', 12),('*', 11),('%', 12)); //overdraw
            AddRow(618,67,8,29,(',', 5),('.', 4)); //overdraw
            AddRow(631,49,15,11,(':', 4),(';', 5)); //overdraw
            AddRow(644,38,8,0,('\'', 3),('\"', 9)); //overdraw

            return glyphDictionary;
        }
    }
}
