using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine;
using TwelveEngine.UI.Book;

namespace Elves.Settings {
    public sealed class VolumeBar:SpriteElement {
        private float _value;

        public float Value {
            get => _value;
            set {
                if(_value == value) {
                    return;
                }
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }

        private readonly Vector2[] origins = new Vector2[] {
            new(4,2),new(10,2),new(16,2),new(22,2),
            new(28,3),new(34,3),new(40,3),new(46,3)
        };

        private readonly Rectangle[] textureSources = new Rectangle[] {
            new(236,13,4,6), new(241,13,4,6), new(246,13,4,6)
        };

        public event Action<float> OnValueChanged;

        public VolumeBar(float value) {
            _value = value;
            Texture = Program.Textures.SettingsPhone;
            TextureSource = new(200,61,54,12);
            OnRender += RenderVolumeDots;
        }

        private Vector2 _destination;
        private float _pixelSize;

        private void RenderDot(SpriteBatch spriteBatch,Vector2 origin,Rectangle textureSource) {
            FloatRectangle destination = new(_destination + origin * _pixelSize,textureSource.Size.ToVector2() * _pixelSize);
            destination.Y += ComputedArea.Y - Position.Y; /* Animation hack so we can follow the parent */
            spriteBatch.Draw(Texture,(Rectangle)destination,textureSource,ComputedColor,0,Vector2.Zero,SpriteEffects.None,Depth + 0.1f);
        }

        private void RenderVolumeDots(SpriteBatch spriteBatch) {
            if(ComputedArea.Y > Viewport.Height) {
                return;
            }
            for(int i = 0;i<origins.Length;i++) {
                float value = (float)(i+1) / origins.Length;
                /* Next stop, one line central. I don't want to program this user interface anymore, OKAY? */
                RenderDot(spriteBatch,origins[i],value > Value ? textureSources[0] : textureSources[i % 2 == 0 ? 2 : 1]);
            }
        }

        public void SetOrigin(Vector2 destination,float pixelSize) {
            this._destination = destination;
            this._pixelSize = pixelSize;
        }
    }
}
