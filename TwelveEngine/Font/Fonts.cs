using TwelveEngine.Shell;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Font {
    public static partial class Fonts {

        public static bool IsLoaded { get; private set; } = false;

        public static UVSpriteFont DefaultFont { get; private set; }
        public static UVSpriteFont RetroFont { get; private set; }

        public static UVSpriteFont RetroFontOutlined { get; private set; }

        public static void Load(GameManager game) {
            if(IsLoaded) {
                return;
            }

            DefaultFont = new UVSpriteFont(game.Content.Load<Texture2D>("Font/twelven-font"),34,1,8,GetUIFontData());
            RetroFont = new UVSpriteFont(game.Content.Load<Texture2D>("Font/classic-font"),10,1,4,GetRetroFontData());

            RetroFontOutlined = new UVSpriteFont(game.Content.Load<Texture2D>("Font/classic-outline"),22,1,4,GetRetroFontOutlinedData());
            IsLoaded = true;
        }
    }
}
