using System;
using Liru3D.Animations;
using Liru3D.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game3D.Entity.Types {
    public class AnimatedModelEntity:ModelBase {

        protected override int GetEntityType() => Entity3DType.AnimatedModel;

        private SkinnedModel model;
        private SkinnedEffect skinnedEffect;

        public AnimationPlayer AnimationPlayer { get; private set; }

        public AnimatedModelEntity() {
            OnUnload += AnimatedModel_OnUnload;
            OnUpdate += AnimatedModel_OnUpdate;
            OnRender += AnimatedModel_OnRender;
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

        private void SetDefaultAnimation() {
            AnimationPlayer.Animation = model.Animations[0];
            AnimationPlayer.IsLooping = false;
        }

        protected override void LoadModel() {
            model = Game.Content.Load<SkinnedModel>(Model);
            skinnedEffect = new SkinnedEffect(Game.GraphicsDevice);
            skinnedEffect.EnableDefaultLighting();
            skinnedEffect.AmbientLightColor = Color.White.ToVector3();
            AnimationPlayer = new AnimationPlayer(model);
            SetDefaultAnimation();
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
