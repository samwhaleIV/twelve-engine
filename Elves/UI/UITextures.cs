using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Shell;

namespace Elves.UI {
    public static class UITextures {

        public static Texture2D RetroFont { get; private set; }
        public static Texture2D DefaultFont { get; private set; }
        public static Texture2D Panel { get; private set; }
        public static Texture2D Nothing { get; private set; }

        public static bool IsLoaded { get; private set; } = false;

        public static void Load(GameManager game) {
            if(IsLoaded) {
                return;
            }
            var content = game.Content;

            RetroFont = content.Load<Texture2D>("UI/classic-font");
            DefaultFont = content.Load<Texture2D>("UI/twelven-font");
            Panel = content.Load<Texture2D>("UI/panel");
            Nothing  = content.Load<Texture2D>("UI/nothing");

            IsLoaded = true;
        }
    }
}
