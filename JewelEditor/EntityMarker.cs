﻿using TwelveEngine.Game2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JewelEditor {
    internal sealed class EntityMarker:Entity2D {
        protected override int GetEntityType() => JewelEntities.EntityMarker;

        public bool Highlighted { get; set; } = false;

        private Texture2D texture;

        public EntityMarker() {
            OnRender += EntityMarker_OnRender;
            OnLoad += EntityMarker_OnLoad;
        }

        private void EntityMarker_OnLoad() {
            texture = Game.Content.Load<Texture2D>(MapCreator.TilesetPath);
        }

        private Rectangle textureSource = new Rectangle(16,32,16,16);
        private Rectangle textureSourceHighlighted = new Rectangle(32,32,16,16);

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
