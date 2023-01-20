using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Elves.Scenes.SaveSelect {
    public sealed class SaveActionButton {

        private static readonly Dictionary<SaveButtonType,Rectangle> buttonSources = new() {
            { SaveButtonType.Back, new(0,123,16,16) },
            { SaveButtonType.Play, new(17,123,16,16) },
            { SaveButtonType.Yes, new(0,140,16,16) },
            { SaveButtonType.Delete, new(17,140,16,16) },
        };

        public SaveButtonType Type { get; set; }

        private Rectangle GetButtonSource(SaveButtonType type) {
            if(!buttonSources.TryGetValue(type,out Rectangle source)) {
                return Rectangle.Empty;
            }
            return source;
        }

        public Rectangle Destination { get; private set; } = new(0,0,0,0);
        public float Rotation { get; set; } = 0f;
        public Texture2D Texture { get; set; }

        public void Update(Vector2 screenOrigin,float height) {
            if(Scale <= 0) {
                return;
            }
            Vector2 size = new Vector2(height) * Scale;
            Destination = new(screenOrigin.ToPoint(),size.ToPoint());
        }

        public float Scale { get; set; } = 1f;

        public void Render(SpriteBatch spriteBatch) {
            if(Scale <= 0) {
                return;
            }
            Rectangle source = GetButtonSource(Type);
            spriteBatch.Draw(Texture,Destination,source,Color.White,Rotation,source.Size.ToVector2()*0.5f,SpriteEffects.None,1f);
        }
    }
}
