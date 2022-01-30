using Liru3D.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Serial;

namespace TwelveEngine.Game3D.Entity.Types {
    public class AnimatedModelEntity:ModelBase {

        protected override int GetEntityType() => Entity3DType.AnimatedModel;

        private SkinnedModel model;
        private SkinnedEffect skinnedEffect;

        public SerialAnimationPlayer AnimationPlayer { get; private set; }

        public AnimatedModelEntity() {
            OnUnload += AnimatedModel_OnUnload;
            OnUpdate += AnimatedModel_OnUpdate;
            OnRender += AnimatedModel_OnRender;
            OnExport += AnimatedModelEntity_OnExport;
            OnImport += AnimatedModelEntity_OnImport;
        }

        private void AnimatedModelEntity_OnImport(SerialFrame frame) {
            AnimationPlayer = new SerialAnimationPlayer(model);
            AnimationPlayer.Import(frame);
        }

        private void AnimatedModelEntity_OnExport(SerialFrame frame) {
            AnimationPlayer.Export(frame);
        }

        private void AnimatedModel_OnUpdate(GameTime gameTime) {
            AnimationPlayer.Update(gameTime);
        }

        private void AnimatedModel_OnUnload() {
            skinnedEffect?.Dispose();
            skinnedEffect = null;
        }

        private void AnimatedModel_OnRender(GameTime gameTime) {
            foreach(SkinnedMesh mesh in model.Meshes) {
                AnimationPlayer.SetEffectBones(skinnedEffect);
                foreach(var pass in skinnedEffect.CurrentTechnique.Passes) {
                    pass.Apply();
                }
                mesh.Draw();
            }
        }

        private void LoadAnimationPlayer() {
            AnimationPlayer = new SerialAnimationPlayer(model);
        }

        private static SkinnedEffect GetSkinnedEffect(GraphicsDevice graphicsDevice) {
            var skinnedEffect = new SkinnedEffect(graphicsDevice) {
                AmbientLightColor = Color.White.ToVector3()
            };
            skinnedEffect.EnableDefaultLighting();
            return skinnedEffect;
        }

        protected override void LoadModel() {
            model = Game.Content.Load<SkinnedModel>(Model);

            skinnedEffect = GetSkinnedEffect(Game.GraphicsDevice);

            LoadAnimationPlayer();
        }

        protected override void ApplyTexture(Texture2D texture) {
            skinnedEffect.Texture = texture;
        }

        protected override void ApplyViewMatrix(Matrix viewMatrix) {
            skinnedEffect.View = viewMatrix;
        }

        protected override void ApplyProjectionMatrix(Matrix projectionMatrix) {
            skinnedEffect.Projection = projectionMatrix;
        }

        protected override void ApplyWorldMatrix(Matrix worldMatrix) {
            skinnedEffect.World = worldMatrix;
        }
    }
}
