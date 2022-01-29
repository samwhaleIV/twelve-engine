using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TwelveEngine.Game3D.Entity.Types {
    public class ModelEntity:ModelBase {

        public ModelEntity() => OnRender += ModelEntity_OnRender;

        protected override int GetEntityType() => Entity3DType.Model;

        private Model model;

        private ModelMesh[] modelMeshList = new ModelMesh[0];
        private BasicEffect[] modelEffectsList = new BasicEffect[0];

        public ModelMesh[] MeshList => modelMeshList;
        public BasicEffect[] ModelEffects => modelEffectsList;

        private void ModelEntity_OnRender(GameTime gameTime) {
            for(int i = 0;i<modelMeshList.Length;i++) {
                modelMeshList[i].Draw();
            }
        }

        protected override void ApplyWorldMatrix(Matrix worldMatrix) {
            for(int i = 0;i<modelEffectsList.Length;i++) {
                modelEffectsList[i].World = worldMatrix;
            }
        }

        protected override void ApplyViewMatrix(Matrix viewMatrix) {
            for(int i = 0;i<modelEffectsList.Length;i++) {
                modelEffectsList[i].View = viewMatrix;
            }
        }

        protected override void ApplyProjectionMatrix(Matrix projectionMatrix) {
            for(int i = 0;i<modelEffectsList.Length;i++) {
                modelEffectsList[i].Projection = projectionMatrix;
            }
        }

        protected override void LoadModel() {
            model = Game.Content.Load<Model>(Model);

            var modelMeshList = new Queue<ModelMesh>();
            var modelEffectsList = new Queue<BasicEffect>();

            foreach(ModelMesh mesh in model.Meshes) {
                modelMeshList.Enqueue(mesh);
                foreach(BasicEffect effect in mesh.Effects) {
                    modelEffectsList.Enqueue(effect);
                    effect.EnableDefaultLighting();
                }
            }

            this.modelMeshList = modelMeshList.ToArray();
            this.modelEffectsList = modelEffectsList.ToArray();
        }

        protected override void ApplyTexture(Texture2D texture) {
            for(int i = 0;i<modelEffectsList.Length;i++) {
                modelEffectsList[i].Texture = texture;
            }
        }
    }
}
