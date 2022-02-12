using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Game3D.Entity;
using TwelveEngine.Game3D.Entity.Types;

namespace TwelveEngine.Game3D {
    public sealed class CRTTest:World {

        public CRTTest() {
            OnLoad += CRTTest_OnLoad;
            OnPreRender += PreRenderEntities;
            OnRender += CRTTest_OnRender;
            OnUpdate += CRTTest_OnUpdate;
        }

        private void CRTTest_OnUpdate(GameTime gameTime) {
            if(Camera is AngleCamera angleCamera) {
                angleCamera.UpdateFreeCam(this,0.05f,0.01f);
            }
            Camera.Update(Game.Viewport.AspectRatio);

            var monitor = Entities.Get<WorldMatrixEntity>("CRT");
            var renderTarget = Entities.Get<WorldMatrixEntity>("RenderTarget");

            var rotation = new Vector3(0f,0f,0f);
            var t = (float)(gameTime.TotalGameTime.TotalSeconds / 20f % 1f);
            rotation.X = t * 360f;

            monitor.Rotation = rotation;
            renderTarget.Rotation = rotation;
        }

        private Effect crtFilter;
        private Texture2D texture;

        private void CRTTest_OnRender(GameTime gameTime) {
            RenderEntities(gameTime);
        }

        private RenderTargetEntity GetRenderTargetEntity() {
            var CRTSize = new Point(800,600);

            var topLeft = new Vector2(-0.766212f,2.15167f);
            var bottomRight = new Vector2(0.766212f,0.974369f);

            var zAxis = 0.965f;

            return new RenderTargetEntity() {
                Size = CRTSize,
                TopLeft = new Vector3(topLeft,zAxis),
                BottomRight = new Vector3(bottomRight,zAxis),
                RenderOnTarget = RenderScreen,
                Name = "RenderTarget"
            };
        }

        private void RenderScreen(GameTime gameTime) {
            float t = (float)(gameTime.TotalGameTime.TotalSeconds / 5d % 1d);
            crtFilter.Parameters["time"].SetValue(t);

            Game.SpriteBatch.Begin(SpriteSortMode.Deferred,effect: crtFilter,samplerState: SamplerState.LinearWrap);
            Game.SpriteBatch.Draw(texture,Game.Viewport.Bounds,Color.White);
            Game.SpriteBatch.End();
        }

        private static void ApplyModelLighting(ModelEntity model) {
            foreach(var effect in model.ModelEffects) {
                effect.DirectionalLight0.Enabled = true;
                effect.DirectionalLight1.Enabled = true;
                effect.DirectionalLight2.Enabled = false;

                effect.DirectionalLight0.Direction = new Vector3(0f,1f,-0.5f);
                effect.DirectionalLight0.SpecularColor = new Vector3(1f);
                effect.DirectionalLight0.DiffuseColor = new Vector3(1f);

                effect.DirectionalLight1.Direction = new Vector3(1f,-0.25f,-0.45f);
                effect.DirectionalLight1.SpecularColor = Color.Orange.ToVector3();
                effect.DirectionalLight1.DiffuseColor = new Vector3(0.5f);
            }
        }

        private void SetupCamera() {
            var camera = new AngleCamera() {
                NearPlane = 0.1f,
                FieldOfView = 75f,
                Angle = new Vector2(359.19952f,192.00055f),
                Position = new Vector3(-0.0072858054f,1.7067217f,2.4753852f)
            };

            Camera = camera;
        }

        private void CRTTest_OnLoad() {
            crtFilter = Game.Content.Load<Effect>("Shaders/crt-filter");
            texture = Game.Content.Load<Texture2D>("Test/meta-meme");
            var monitor = new ModelEntity() {Model = "Models/crt-monitor", Name = "CRT"};

            var renderTargetEntity = GetRenderTargetEntity();

            Entities.Create(Entity3DType.GridLines);
            Entities.Add(monitor);
            Entities.Add(renderTargetEntity);

            ApplyModelLighting(monitor);

            SetupCamera();
        }

        protected override void ResetGraphicsDeviceState(GraphicsDevice graphicsDevice) {
            graphicsDevice.Clear(Color.LightGray);
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
        }
    }
}
