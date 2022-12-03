using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TwelveEngine.Shell;

namespace Elves {
    public static class Textures {

        public static Texture2D ClassicFont { get; private set; }
        public static Texture2D UIFont { get; private set; }
        public static Texture2D UIPanel { get; private set; }
        public static Texture2D Nothing { get; private set; }

        public static bool IsLoaded { get; private set; } = false;

        public static void Load(GameManager game) {
            if(IsLoaded) {
                return;
            }
            var content = game.Content;
            ClassicFont = content.Load<Texture2D>("UI/font");
            UIFont = content.Load<Texture2D>("UI/font");
            UIPanel = content.Load<Texture2D>("UI/font");
            Nothing  = content.Load<Texture2D>("UI/font");
            IsLoaded = true;
        }
    }
}
