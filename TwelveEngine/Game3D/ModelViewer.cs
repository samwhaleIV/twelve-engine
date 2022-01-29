using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.GameUI;
using TwelveEngine.Game3D.Entity.Types;
using TwelveEngine.Game3D.Entity;

namespace TwelveEngine.Game3D {

    public static class ModelViewer {
        public static ModelViewer<AnimatedModelEntity> CreateAnimated(string modelName,string textureName = null) {
            return new ModelViewer<AnimatedModelEntity>(modelName,textureName);
        }
        public static ModelViewer<ModelEntity> CreateStatic(string modelName,string textureName = null) {
            return new ModelViewer<ModelEntity>(modelName,textureName);
        }
    }

    public sealed class ModelViewer<TModelEntity>:World where TModelEntity : ModelBase, new() {

        private const string EntityName = "ModelViewerTarget";

        private const float VELOCITY_BASE = 1f / (1000f / 60f);
        public const float MOUSE_SPEED = 0.15f;

        public float ModelRotationSpeed { get; set; } = 2.5f;
        public float FreeCamSpeed { get; set; } = 0.05f;

        private ImpulseGuide ImpulseGuide { get; set; }

        private readonly string modelName;
        private readonly string textureName;

        internal ModelViewer(string modelName,string textureName) {
            this.modelName = modelName;
            this.textureName = textureName;

            OnLoad += ModelTest_OnLoad;
            OnUnload += ModelViewer_OnUnload;

            OnUpdate += ModelTest_OnUpdate;
            OnRender += ModelTest_OnRender;

            OnPreRender += PreRenderEntities;
        }

        private void UpdateImpulseGuide() {
            if(ControlModel) {
                ImpulseGuide.SetDescriptions(
                    (Impulse.Toggle, "Control Mode (Model)"),
                    (Impulse.Up, "-Pitch"),
                    (Impulse.Down, "+Pitch"),
                    (Impulse.Left, "-Roll"),
                    (Impulse.Right, "+Roll"),
                    (Impulse.Ascend, "+Yaw"),
                    (Impulse.Descend, "-Yaw")
                );
            } else {
                ImpulseGuide.SetDescriptions(
                    (Impulse.Toggle, "Control Mode (Camera)"),
                    (Impulse.Up, "Forward"),
                    (Impulse.Down, "Reverse"),
                    (Impulse.Left, "Left"),
                    (Impulse.Right, "Right"),
                    (Impulse.Ascend, "Up"),
                    (Impulse.Descend, "Down")
                );
            }
        }

        private void ModelViewer_OnUnload() {
            Input.OnToggleDown -= Input_OnToggleDown;
        }

        private bool ControlModel = false;
        private void Input_OnToggleDown() {
            ControlModel = !ControlModel;
            UpdateImpulseGuide();
        }

        private void ModelTest_OnLoad() {
            var camera = new AngleCamera() {
                NearPlane = 0.1f,
                FieldOfView = 75f,
                Angle = new Vector2(219.75f,215.10f),
                Position = new Vector3(1.24f,-1.4f,1.34f)
            };
            Camera = camera;

            Entities.Create(Entity3DType.GridLines);

            Entities.Add(new TModelEntity() {
                Name = EntityName,
                Texture = textureName,
                Model = modelName
            });

            Input.OnToggleDown += Input_OnToggleDown;
            ImpulseGuide = new ImpulseGuide(Game);

            UpdateImpulseGuide();
        }

        private float GetVelocity(GameTime gameTime,float velocity) {
            return (float)gameTime.ElapsedGameTime.TotalMilliseconds * VELOCITY_BASE * velocity;
        }

        private float GetRotationVelocity(GameTime gameTime) => GetVelocity(gameTime,ModelRotationSpeed);
        private float GetFreeCamVelocity(GameTime gameTime) => GetVelocity(gameTime,FreeCamSpeed);

        private WorldMatrixEntity GetMatrixEntity() {
            return Entities.Get<WorldMatrixEntity>(EntityName);
        }

        private void RotateModel(GameTime gameTime) {
            var matrixEntity = GetMatrixEntity();
            if(matrixEntity == null) {
                return;
            }

            var rotationDelta = Input.GetDelta3D();
            if(rotationDelta == Vector3.Zero) {
                return;
            }
            float velocity = GetRotationVelocity(gameTime);

            matrixEntity.Rotation += rotationDelta * new Vector3(velocity);
        }

        private void UpdateCamera(GameTime gameTime) {
            var camera = Camera as AngleCamera;
            if(camera == null) {
                return;
            }
            var mouseDelta = Game.MouseHandler.Delta;
            if(Game.MouseHandler.Capturing && mouseDelta != Point.Zero) {
                mouseDelta.Y = -mouseDelta.Y;
                camera.AddAngle(mouseDelta.ToVector2() * MOUSE_SPEED);
            }
            camera.UpdateFreeCam(Input.GetDelta3D(),GetFreeCamVelocity(gameTime));
        }

        private void ModelTest_OnUpdate(GameTime gameTime) {
            if(!ControlModel) {
                UpdateCamera(gameTime);
            }
            Camera.Update(Game.Viewport.AspectRatio);

            if(ControlModel) {
                RotateModel(gameTime);
            }
        }

        private static void ResetGraphicsDeviceState(GraphicsDevice graphicsDevice) {
            graphicsDevice.Clear(Color.Gray);
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
        }

        private void DrawCameraData() {
            var camera = Camera as AngleCamera;
            if(camera == null) return;

            var position1 = new Point(Constants.ScreenEdgePadding,Constants.ScreenEdgePadding);
            var text1 = $"Yaw {string.Format("{0:0.00}",camera.Yaw)}  Pitch {string.Format("{0:0.00}",camera.Pitch)}";

            var position2 = position1 + new Point(0,Game.DebugFont.LineSpacing);
            var text2 = $"X {string.Format("{0:0.00}",camera.Position.X)}  Y {string.Format("{0:0.00}",camera.Position.Y)}  Z {string.Format("{0:0.00}",camera.Position.Z)}";

            Game.SpriteBatch.DrawString(Game.DebugFont,text1,position1.ToVector2(),Color.White);
            Game.SpriteBatch.DrawString(Game.DebugFont,text2,position2.ToVector2(),Color.White);
        }

        private void ModelTest_OnRender(GameTime gameTime) {
            ResetGraphicsDeviceState(Game.GraphicsDevice);
            RenderEntities(gameTime);
            Game.SpriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            ImpulseGuide.Render();
#if DEBUG
            DrawCameraData();
#endif
            Game.SpriteBatch.End();
        }

    }
}
