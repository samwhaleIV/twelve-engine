using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwelveEngine.UI.Elements {
    internal sealed class ScrollBar:RenderElement {
        private Texture2D glyphs;

        public ScrollBar() {
            Anchor = Anchor.TopRight;
            Width = 16;
            Height = 100;
            Sizing = Sizing.PercentY;
            OnLoad += () => glyphs = GetImage("scroll-glpyh");
            IsInteractable = true;

            OnRender += ScrollBar_OnRender;
        }

        private void ScrollBar_OnRender(GameTime obj) {
            throw new NotImplementedException();
        }
    }
}
