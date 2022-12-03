using Elves.UI;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Shell;

namespace Elves {
    public static partial class Fonts {

        public static bool IsLoaded { get; private set; } = false;

        public static UVSpriteFont UIFont { get; private set; }
        public static UVSpriteFont ClassicFont { get; private set; }

        public static void Load() {
            if(IsLoaded) {
                return;
            }
            UIFont = new UVSpriteFont(Textures.UIFont,34,1,4,GetUIFontData());
            ClassicFont = null;//todo
            IsLoaded = true;
        }
    }
}
