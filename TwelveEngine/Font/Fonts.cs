using TwelveEngine.Shell;

namespace TwelveEngine.Font {
    public static partial class Fonts {

        public static bool IsLoaded { get; private set; } = false;

        public static UVSpriteFont Default { get; private set; }
        public static UVSpriteFont Retro { get; private set; }

        public static UVSpriteFont RetroOutlined { get; private set; }

        public static void Load(GameStateManager game) {
            if(IsLoaded) {
                return;
            }

            Default = new UVSpriteFont(game.Content.Load<Texture2D>("Font/twelven-font"),34,1,8,GetUIFontData());
            Retro = new UVSpriteFont(game.Content.Load<Texture2D>("Font/classic-font"),10,0.75f,3f,GetRetroFontData());

            RetroOutlined = new UVSpriteFont(game.Content.Load<Texture2D>("Font/classic-outline"),22,1,4,GetRetroFontOutlinedData());
            IsLoaded = true;
        }
    }
}
