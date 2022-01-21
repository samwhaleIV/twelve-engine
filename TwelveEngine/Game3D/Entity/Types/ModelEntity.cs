using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TwelveEngine.Game3D.Entity.Types {
    public class ModelEntity:WorldMatrixEntity {

        private string _modelName;
        public string ModelName {
            get => _modelName;
            set {
                if(_modelName == value) {
                    return;
                }
                _modelName = value;
                if(!IsLoaded) {
                    return;
                }
                LoadModel();
            }
        }

        private Model model;

        private ModelMesh[] modelMeshList = new ModelMesh[0];
        private BasicEffect[] modelEffectsList = new BasicEffect[0];

        public ModelMesh[] MeshList => modelMeshList;
        public BasicEffect[] ModelEffects => modelEffectsList;

        protected override int GetEntityType() => Entity3DType.Model;

        public ModelEntity() {
            OnLoad += ModelEntity_OnLoad;
            OnUnload += ModelEntity_OnUnload;

            OnUpdate += ModelEntity_OnUpdate;
            OnRender += ModelEntity_OnRender;

            OnImport += frame => ModelName = frame.GetString();
            OnExport += frame => frame.Set(ModelName);
        }

        private void ModelEntity_OnUpdate(GameTime gameTime) {
            UpdateWorldMatrix(ApplyWorldMatrix);
        }

        private void ModelEntity_OnRender(GameTime gameTime) {
            for(int i = 0;i<modelMeshList.Length;i++) {
                modelMeshList[i].Draw();
            }
        }

        private void ApplyWorldMatrix(Matrix worldMatrix) {
            for(int i = 0;i<modelEffectsList.Length;i++) {
                modelEffectsList[0].World = worldMatrix;
            }
        }

        private void ApplyViewMatrix(Matrix viewMatrix) {
            for(int i = 0;i<modelEffectsList.Length;i++) {
                modelEffectsList[0].View = viewMatrix;
            }
        }

        private void ApplyProjectionMatrix(Matrix projectionMatrix) {
            for(int i = 0;i<modelEffectsList.Length;i++) {
                modelEffectsList[0].Projection = projectionMatrix;
            }
        }

        private void LoadModel() {
            model = Game.Content.Load<Model>(ModelName);

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

            ApplyProjectionMatrix(Owner.ProjectionMatrix);
            ApplyViewMatrix(Owner.ViewMatrix);
        }

        private void ModelEntity_OnLoad() {
            LoadModel();
            Owner.OnProjectionMatrixChanged += ApplyProjectionMatrix;
            Owner.OnViewMatrixChanged += ApplyViewMatrix;
        }

        private void ModelEntity_OnUnload() {
            Owner.OnProjectionMatrixChanged -= ApplyProjectionMatrix;
            Owner.OnViewMatrixChanged -= ApplyViewMatrix;
        }
    }
}
