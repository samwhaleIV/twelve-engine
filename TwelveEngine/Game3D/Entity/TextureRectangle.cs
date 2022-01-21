using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game3D.Entity {
    public abstract class TextureRectangle:WorldMatrixEntity {

        private Texture2D _texture;
        protected Texture2D Texture {
            get => _texture;
            set {
                _texture = value;
                if(effect == null) {
                    return;
                }
                effect.Texture = value;
            }
        }

        private BasicEffect effect = null;

        public TextureRectangle() {
            OnUpdate += TextureRectangle_OnUpdate;
            OnRender += TextureRectangle_OnRender;
            OnLoad += TextureRectangle_OnLoad;
            OnUnload += TextureRectangle_OnUnload;
        }

        private void WorldMatrixChanged(Matrix worldMatrix) {
            effect.World = worldMatrix;
        }
        private void ViewMatrixChanged(Matrix viewMatrix) {
            effect.View = viewMatrix;
        }
        private void ProjectionMatrixChanged(Matrix projectionMatrix) {
            effect.Projection = projectionMatrix;
        }

        private void TextureRectangle_OnLoad() {

            effect = new BasicEffect(Owner.Game.GraphicsDevice) {
                TextureEnabled = true,
                LightingEnabled = false,
                VertexColorEnabled = true,
            };

            Owner.OnProjectionMatrixChanged += ProjectionMatrixChanged;
            Owner.OnViewMatrixChanged += ViewMatrixChanged;

            ViewMatrixChanged(Owner.ViewMatrix);
            ProjectionMatrixChanged(Owner.ProjectionMatrix);
        }

        private void TextureRectangle_OnUnload() {
            Owner.OnProjectionMatrixChanged -= ProjectionMatrixChanged;
            Owner.OnViewMatrixChanged -= ViewMatrixChanged;
            effect?.Dispose();
        }

        private void TextureRectangle_OnUpdate(GameTime gameTime) {
            UpdateWorldMatrix(WorldMatrixChanged);
        }

        private static VertexPositionColorTexture[] GetVertices() {

            float left = -0.5f, top = -0.5f, right = 0.5f, bottom = 0.5f;

            var a = new VertexPositionColorTexture(new Vector3(left,top,0),Color.White,new Vector2(1,1));
            var b = new VertexPositionColorTexture(new Vector3(right,top,0),Color.White,new Vector2(0,1));

            var c = new VertexPositionColorTexture(new Vector3(left,bottom,0),Color.White,new Vector2(1,0));
            var d = new VertexPositionColorTexture(new Vector3(right,bottom,0),Color.White,new Vector2(0,0));

            return new VertexPositionColorTexture[] { a,b,c,b,d,c };
        }

        private readonly VertexPositionColorTexture[] vertices = GetVertices();

        private void TextureRectangle_OnRender(GameTime gameTime) {
            foreach(var pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                Owner.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,vertices,0,2);
            }
        }
    }
}
