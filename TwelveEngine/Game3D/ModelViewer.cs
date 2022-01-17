using TwelveEngine.Serial;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.GameUI;

namespace TwelveEngine.Game3D {
    public sealed class ModelViewer:DataGameState<string> {

        private ImpulseGuide ImpulseGuide { get; set; }

        public const float MOUSE_SPEED = 0.1f;

        private readonly GridLines gridLines = new GridLines();

        private void UpdateImpulseGuide() {
            if(ControlModel) {
                ImpulseGuide.SetDescriptions(
                    (Impulse.Toggle, "Control Mode"),
                    (Impulse.Up, "-Y"),
                    (Impulse.Down, "+Y"),
                    (Impulse.Left, "-X"),
                    (Impulse.Right, "+X"),
                    (Impulse.Ascend, "+Z"),
                    (Impulse.Descend, "-Z")
                );
            } else {
                ImpulseGuide.SetDescriptions(
                    (Impulse.Toggle, "Control Mode"),
                    (Impulse.Up, "Forward"),
                    (Impulse.Down, "Reverse"),
                    (Impulse.Left, "Left"),
                    (Impulse.Right, "Right"),
                    (Impulse.Ascend, "Up"),
                    (Impulse.Descend, "Down")
                );
            }
        }

        public ModelViewer() {
            OnLoad += ModelTest_OnLoad;
            OnUnload += ModelViewer_OnUnload;

            OnUpdate += ModelTest_OnUpdate;
            OnRender += ModelTest_OnRender;

            camera = new AngleCamera() {
                NearPlane = 0.1f,
                FieldOfView = 75f,
                Angle = new Vector2(350.1f,34.6f),
                Position = new Vector3(-0.05f,-0.25f,0.17f)
            };

            OnImport += ModelViewer_OnImport;
            OnExport += ModelViewer_OnExport;
        }

        private void ModelViewer_OnUnload() {
            gridLines.Unload();
            Input.OnToggleDown -= Input_OnToggleDown;
        }

        private void ModelViewer_OnExport(SerialFrame frame) {
            frame.Set(angle);
            frame.Set(camera);
        }

        private void ModelViewer_OnImport(SerialFrame frame) {
            var startAngle = angle;
            angle = frame.GetVector3();
            if(startAngle != angle) {
                UpdateModelMatrix();
            }
            frame.Get(camera);
        }

        public float RotationSpeed { get; set; } = 0.05f;
        public float FreeCamSpeed { get; set; } = 0.01f;

        private const float VELOCITY_BASE = 1f / (1000f / 60f);

        private Model model;

        private readonly AngleCamera camera;

        private bool ControlModel = false;
        private void Input_OnToggleDown() {
            ControlModel = !ControlModel;
            UpdateImpulseGuide();
        }

        private static Matrix GetOriginMatrix() {
            return Matrix.CreateWorld(Vector3.Zero,Orientation.WorldForward,Orientation.WorldUp);
        }

        private readonly Matrix originMatrix = GetOriginMatrix();

        private Vector3 angle = Vector3.Zero;
        private Matrix modelMatrix = GetOriginMatrix();

        private void ModelTest_OnLoad() {
            model = Game.Content.Load<Model>(Data);
            gridLines.Load(Game.GraphicsDevice);
            Input.OnToggleDown += Input_OnToggleDown;
            ImpulseGuide = new ImpulseGuide(Game);
            UpdateImpulseGuide();
        }

        private float GetVelocity(GameTime gameTime,float velocity) {
            return (float)gameTime.ElapsedGameTime.TotalMilliseconds * VELOCITY_BASE * velocity;
        }

        private float GetRotationVelocity(GameTime gameTime) => GetVelocity(gameTime,RotationSpeed);
        private float GetFreeCamVelocity(GameTime gameTime) => GetVelocity(gameTime,FreeCamSpeed);

        private void UpdateModelMatrix() {
            modelMatrix = originMatrix * Matrix.CreateFromYawPitchRoll(angle.X,angle.Y,angle.Z);
        }

        private void RotateModel(GameTime gameTime) {
            var rotationDelta = Input.GetDelta3D();
            if(rotationDelta == Vector3.Zero) {
                return;
            }

            float velocity = GetRotationVelocity(gameTime);
            angle += rotationDelta * new Vector3(velocity);

            UpdateModelMatrix();
        }

        private void UpdateCamera(GameTime gameTime) {
            var mouseDelta = Game.MouseHandler.Delta;
            if(Game.MouseHandler.Capturing && mouseDelta != Point.Zero) camera.AddRotation(
                mouseDelta.X * MOUSE_SPEED,
                -mouseDelta.Y * MOUSE_SPEED /* INVERTED Y AXIS??? >:( */
            );
            camera.UpdateFreeCam(Input.GetDelta3D(),GetFreeCamVelocity(gameTime));
        }

        private void ModelTest_OnUpdate(GameTime gameTime) {
            if(!ControlModel) {
                UpdateCamera(gameTime);
            }
            camera.UpdateMatrices(Game.GraphicsDevice.Viewport.AspectRatio);
            gridLines.Update(camera);

            if(ControlModel) {
                RotateModel(gameTime);
            }
        }

        private void UpdateMeshEffect(BasicEffect effect) {
            effect.Projection = camera.ProjectionMatrix;
            effect.View = camera.ViewMatrix;
            effect.World = modelMatrix;
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
            foreach(var mesh in model.Meshes) {
                foreach(BasicEffect effect in mesh.Effects) {
                    UpdateMeshEffect(effect);
                }
                mesh.Draw();
            }
            Game.SpriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            ImpulseGuide.Render();
            DrawCameraData();
            Game.SpriteBatch.End();
        }

    }
}
