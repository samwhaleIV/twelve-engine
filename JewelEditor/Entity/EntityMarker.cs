using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JewelEditor.Entity {
    internal sealed class EntityMarker:JewelEntity {
        protected override int GetEntityType() => JewelEntities.EntityMarker;

        public bool Highlighted { get; set; } = false;

        private Texture2D texture;

        public EntityMarker() {
            OnRender += EntityMarker_OnRender;
            OnLoad += EntityMarker_OnLoad;
        }

        private void EntityMarker_OnLoad() {
            texture = Game.Content.Load<Texture2D>(Editor.Tileset);
        }

        private Rectangle textureSource = new Rectangle(32,32,16,16);
        private Rectangle textureSourceHighlighted = new Rectangle(48,32,16,16);

        private Rectangle GetTextureSource() {
            if(Highlighted) {
                return textureSourceHighlighted;
            } else {
                return textureSource;
            }
        }

        private void EntityMarker_OnRender(GameTime gameTime) {
            if(!OnScreen()) {
                return;
            }
            Game.SpriteBatch.Draw(texture,GetDestination(),GetTextureSource(),Color.White);
        }
    }
}
