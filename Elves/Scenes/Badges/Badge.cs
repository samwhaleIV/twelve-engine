using Microsoft.Xna.Framework.Graphics;

namespace Elves.Scenes.Badges {
    public readonly struct Badge {

        public readonly Texture2D Texture { get; private init; }
        public readonly string Text { get; private init; }
        public readonly float Scale { get; private init; }

        public Badge(Texture2D texture,float scale = 1,string text = null) {
            Texture = texture;
            Text = text ?? string.Empty;
            Scale = scale;
        }
    }
}
