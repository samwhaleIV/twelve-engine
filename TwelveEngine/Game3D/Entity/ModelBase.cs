using TwelveEngine.Serial;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game3D.Entity {
    public abstract class ModelBase:WorldMatrixEntity {

        public ModelBase() {
            OnLoad += ModelBase_OnLoad;
            OnUnload += ModelBase_OnUnload;
            OnUpdate += ModelBase_OnUpdate;
        }

        private void ModelBase_OnUpdate(GameTime gameTime) {
            UpdateWorldMatrix(ApplyWorldMatrix);
        }

        private string _modelName;
        public string Model {
            get => _modelName;
            set {
                if(_modelName == value) {
                    return;
                }
                _modelName = value;
                if(!IsLoaded) {
                    return;
                }
                LoadAndUpdateModel();
            }
        }

        private string _textureName = null;

        public string Texture {
            get => _textureName;
            set {
                if(Texture == value) {
                    return;
                }
                _textureName = value;
                if(!IsLoaded) {
                    return;
                }
                LoadTexture();
            }
        }

        private void LoadTexture() {
            if(string.IsNullOrEmpty(Texture)) {
                return;
            }
            var texture = Game.Content.Load<Texture2D>(Texture);
            ApplyTexture(texture);
        }

        private void LoadAndUpdateModel() {
            LoadModel();
            ApplyProjectionMatrix(Owner.ProjectionMatrix);
            ApplyViewMatrix(Owner.ViewMatrix);
        }

        private void ModelBase_OnLoad() {
            LoadAndUpdateModel();
            LoadTexture();
            Owner.OnProjectionMatrixChanged += ApplyProjectionMatrix;
            Owner.OnViewMatrixChanged += ApplyViewMatrix;
        }

        private void ModelBase_OnUnload() {
            Owner.OnProjectionMatrixChanged -= ApplyProjectionMatrix;
            Owner.OnViewMatrixChanged -= ApplyViewMatrix;
        }

        protected abstract void LoadModel();

        protected abstract void ApplyTexture(Texture2D texture);
        protected abstract void ApplyViewMatrix(Matrix viewMatrix);
        protected abstract void ApplyProjectionMatrix(Matrix projectionMatrix);
        protected abstract void ApplyWorldMatrix(Matrix worldMatrix);
    }
}
