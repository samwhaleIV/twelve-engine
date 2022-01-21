using TwelveEngine.Serial;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game3D.Entity.Types {
    public sealed class TextureEntity:TextureRectangle {

        protected override int GetEntityType() => Entity3DType.Texture;

        private void BindEvents() {
            OnLoad += TextureEntity_OnLoad;
            OnImport += TextureEntity_OnImport;
            OnExport += TextureEntity_OnExport;
        }

        public TextureEntity(string textureName) {
            TextureName = textureName;
            BindEvents();
        }

        public TextureEntity() => BindEvents();

        private string _textureName = string.Empty;
        public string TextureName {
            get => _textureName;
            set {
                if(string.IsNullOrEmpty(value)) {
                    value = string.Empty;
                }
                if(_textureName == value) {
                    return;
                }
                _textureName = value;
                if(!IsLoaded) {
                    return;
                }
            }
        }

        private void TextureEntity_OnExport(SerialFrame frame) {
            frame.Set(TextureName);
        }

        private void TextureEntity_OnImport(SerialFrame frame) {
            TextureName = frame.GetString();
        }

        private void TextureEntity_OnLoad() {
            Texture = Owner.Game.Content.Load<Texture2D>(TextureName);
        }
    }
}
