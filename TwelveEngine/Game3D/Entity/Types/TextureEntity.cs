using TwelveEngine.Serial;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game3D.Entity.Types {
    public class TextureEntity:TextureRectangle {

        private void BindEvents() {
            OnLoad += TextureEntity_OnLoad;
        }

        public TextureEntity(string textureName) {
            TextureName = textureName;
            BindEvents();
        }

        public TextureEntity() => BindEvents();

        private string _textureName = null;
        public string TextureName {
            get => _textureName;
            set {
                if(_textureName == value) {
                    return;
                }
                _textureName = value;
            }
        }

        private void TextureEntity_OnLoad() {
            Texture = Game.Content.Load<Texture2D>(TextureName);
        }
    }
}
