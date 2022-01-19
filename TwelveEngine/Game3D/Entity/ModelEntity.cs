using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TwelveEngine.Game3D.Entity {
    public class ModelEntity:Entity3D {

        public int FactoryID { get; set; }

        protected override int GetEntityType() => FactoryID;

        private readonly string modelName;
        public ModelEntity(string modelName) {
            this.modelName = modelName;
            OnLoad += ModelEntity_OnLoad;
            OnUpdate += ModelEntity_OnUpdate;
            OnRender += ModelEntity_OnRender;
        }

        private void ModelEntity_OnRender(GameTime obj) {
            for(int i = 0;i<modelMeshList.Length;i++) {
                modelMeshList[i].Draw();
            }
        }

        private Model model;

        private ModelMesh[] modelMeshList = new ModelMesh[0];
        private BasicEffect[] modelEffectsList = new BasicEffect[0];

        public ModelMesh[] MeshList => modelMeshList;
        public BasicEffect[] ModelEffects => modelEffectsList;

        private Matrix originMatrix = Matrix.Identity;

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

        private void UpdateOriginMatrix() {
            if(PositionValid) {
                return;
            }
            originMatrix = Matrix.CreateWorld(Position,Orientation.WorldForward,Orientation.WorldUp);
            PositionValid = true;
            RotationValid = false;
        }
        private void UpdateWorldMatrix() {
            if(RotationValid) {
                return;
            }
            var rotation = Rotation;
            var worldMatrix = originMatrix * Matrix.CreateFromYawPitchRoll(rotation.X,rotation.Y,rotation.Z);
            RotationValid = true;
            ApplyWorldMatrix(worldMatrix);
        }

        private void ModelEntity_OnUpdate(GameTime gameTime) {
            UpdateOriginMatrix(); UpdateWorldMatrix();
        }

        private void ModelEntity_OnLoad() {
            model = Game.Content.Load<Model>(modelName);

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

            Owner.OnProjectionMatrixChanged += ApplyProjectionMatrix;
            Owner.OnViewMatrixChanged += ApplyViewMatrix;

            ApplyProjectionMatrix(Owner.ProjectionMatrix);
            ApplyViewMatrix(Owner.ViewMatrix);
        }
    }
}
