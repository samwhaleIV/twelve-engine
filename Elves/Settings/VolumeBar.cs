using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine;
using TwelveEngine.UI.Book;

namespace Elves.Settings {
    public sealed class VolumeBar:SpriteElement {
        public float Value { get; set; }
        public VolumeBar() {
            Texture = Program.Textures.SettingsPhone;
            TextureSource = new(200,61,54,12);
            OnRender += RenderVolumeDots;
        }

        private Vector2 _destination;
        private float _pixelSize;

        private void RenderVolumeDots(SpriteBatch spriteBatch) {
            //todo
        }

        public void SetOrigin(Vector2 destination,float pixelSize) {
            this._destination = destination;
            this._pixelSize = pixelSize;
        }
    }
}
