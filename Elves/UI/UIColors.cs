using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Elves.UI {
    public static class UIColors {
        public static readonly Color PressedColor = Color.Lerp(Color.White, Color.Black,0.1f);
        public static readonly Color SelectColor = Color.Lerp(Color.White,Color.Black,0.05f);
    }
}
