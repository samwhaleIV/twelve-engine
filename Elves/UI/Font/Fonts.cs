using Elves.UI;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Shell;

namespace Elves.UI.Font {
    public static partial class Fonts {

        public static bool IsLoaded { get; private set; } = false;

        public static UVSpriteFont DefaultFont { get; private set; }
        public static UVSpriteFont RetroFont { get; private set; }

        public static void Load() {
            if(IsLoaded) {
                return;
            }
            DefaultFont = new UVSpriteFont(UITextures.DefaultFont,34,1,8,GetUIFontData());
            RetroFont = null;//todo
            IsLoaded = true;
        }
    }
}
