using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Shell;

namespace Elves {
    public static class Textures {

        public static Texture2D Panel { get; private set; }
        public static Texture2D Nothing { get; private set; }

        public static Texture2D Menu { get; private set; }

        public static Texture2D CarouselMenu { get; private set; }

        public static Texture2D MissingTexture { get; private set; }

        public static bool IsLoaded { get; private set; } = false;

        public static void Load(GameManager game) {
            if(IsLoaded) {
                return;
            }
            var content = game.Content;

            Panel = content.Load<Texture2D>("UI/panel");
            Nothing  = content.Load<Texture2D>("UI/nothing");

            Menu = content.Load<Texture2D>("Menu/falling-elf");

            MissingTexture = content.Load<Texture2D>("MissingTexture");

            CarouselMenu = content.Load<Texture2D>("Menu/carousel");

            IsLoaded = true;
        }
    }
}
