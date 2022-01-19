using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.GameUI;
using TwelveEngine.Game3D.Entity;
using TwelveEngine.EntitySystem;

namespace TwelveEngine.Game3D {
    public sealed class ModelViewer:World {

        private const string EntityName = "ModelViewerTarget";
        private const int FactoryID = 0;

        private readonly string modelName;
        private AngleCamera camera;
        private ModelEntity modelEntity;

        private const float VELOCITY_BASE = 1f / (1000f / 60f);
        public const float MOUSE_SPEED = 0.15f;

        public float ModelRotationSpeed { get; set; } = 0.05f;
        public float FreeCamSpeed { get; set; } = 0.05f;

        private ImpulseGuide ImpulseGuide { get; set; }
        private readonly GridLines gridLines = new GridLines();


        public ModelViewer(string modelName) : base(new EntityFactory<Entity3D,World>(
            (0,() => new ModelEntity(modelName) {
                FactoryID = FactoryID,
                Name = EntityName
            })
        )) {
            this.modelName = modelName;

            OnLoad += ModelTest_OnLoad;
            OnUnload += ModelViewer_OnUnload;

            OnUpdate += ModelTest_OnUpdate;
            OnRender += ModelTest_OnRender;
        }

        private void UpdateImpulseGuide() {
            if(ControlModel) {
                ImpulseGuide.SetDescriptions(
                    (Impulse.Toggle, "Control Mode (Model)"),
                    (Impulse.Up, "-Y"),
                    (Impulse.Down, "+Y"),
                    (Impulse.Left, "-X"),
                    (Impulse.Right, "+X"),
                    (Impulse.Ascend, "+Z"),
                    (Impulse.Descend, "-Z")
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
            gridLines.Unload();
            Input.OnToggleDown -= Input_OnToggleDown;
        }

        private bool ControlModel = false;
        private void Input_OnToggleDown() {
            ControlModel = !ControlModel;
            UpdateImpulseGuide();
        }

        private void ModelTest_OnLoad() {
            camera = new AngleCamera() {
                NearPlane = 0.1f,
                FieldOfView = 75f,
                Angle = new Vector2(219.75f,215.10f),
                Position = new Vector3(1.24f,-1.4f,1.34f)
            };
            Camera = camera;

            modelEntity = new ModelEntity(modelName) {
                FactoryID = FactoryID,
                Name = EntityName
            };
            OnImport += frame => {
                var reloadedModel = (ModelEntity)EntityManager.Get(EntityName);
                modelEntity = reloadedModel;
            };

            EntityManager.AddEntity(modelEntity);
            gridLines.Load(this);
            Input.OnToggleDown += Input_OnToggleDown;
            ImpulseGuide = new ImpulseGuide(Game);

            UpdateImpulseGuide();
        }

        private float GetVelocity(GameTime gameTime,float velocity) {
            return (float)gameTime.ElapsedGameTime.TotalMilliseconds * VELOCITY_BASE * velocity;
        }

        private float GetRotationVelocity(GameTime gameTime) => GetVelocity(gameTime,ModelRotationSpeed);
        private float GetFreeCamVelocity(GameTime gameTime) => GetVelocity(gameTime,FreeCamSpeed);

        private void RotateModel(GameTime gameTime) {
            var rotationDelta = Input.GetDelta3D();
            if(rotationDelta == Vector3.Zero) {
                return;
            }

            float velocity = GetRotationVelocity(gameTime);
            modelEntity.Rotation += rotationDelta * new Vector3(velocity);
        }

        private void UpdateCamera(GameTime gameTime) {
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
            camera.Update(Game.GraphicsDevice.Viewport.AspectRatio);

            if(ControlModel) {
                RotateModel(gameTime);
            }
        }

        private void ConfigureSamplerState() {
            Game.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
        }

        private void DrawCameraData() {
            var position1 = new Point(Constants.ScreenEdgePadding,Constants.ScreenEdgePadding);
            var text1 = $"Yaw {string.Format("{0:0.00}",camera.Yaw)}  Pitch {string.Format("{0:0.00}",camera.Pitch)}";

            var position2 = position1 + new Point(0,Game.DebugFont.LineSpacing);
            var text2 = $"X {string.Format("{0:0.00}",camera.Position.X)}  Y {string.Format("{0:0.00}",camera.Position.Y)}  Z {string.Format("{0:0.00}",camera.Position.Z)}";

            Game.SpriteBatch.DrawString(Game.DebugFont,text1,position1.ToVector2(),Color.White);
            Game.SpriteBatch.DrawString(Game.DebugFont,text2,position2.ToVector2(),Color.White);
        }

        private void ModelTest_OnRender(GameTime gameTime) {
            Game.GraphicsDevice.Clear(Color.Gray);
            gridLines.Render();
            ConfigureSamplerState();
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
