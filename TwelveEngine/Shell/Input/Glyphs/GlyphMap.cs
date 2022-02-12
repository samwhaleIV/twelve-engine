using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TwelveEngine.Shell.Input.Glyphs {
    internal abstract class GlyphMap<T> {

        public int BlockColumns { get; set; } = 4;
        public int GlyphSize { get; set; } = 16;

        public int BlockSize => BlockColumns * GlyphSize;

        protected abstract T[] GetList();

        private Dictionary<T,Rectangle> glyphTable;

        public void LoadGlyphs() {
            if(glyphTable != null) {
                return;
            }
            var list = GetList();

            var table = new Dictionary<T,Rectangle>();

            for(var i = 0;i < list.Length;i++) {
                int x = i % BlockColumns, y = i / BlockColumns;
                x *= GlyphSize; y *= GlyphSize;

                table[list[i]] = new Rectangle(x,y,GlyphSize,GlyphSize);
            }
         
            glyphTable = table;
        }

        public Rectangle GetGlyph(T type) {
            return glyphTable[type];
        }
    }
}
