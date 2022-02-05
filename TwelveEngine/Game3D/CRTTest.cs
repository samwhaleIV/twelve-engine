using System;
using System.Collections.Generic;
using System.Text;
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
                angleCamera.Debug_UpdateFreeCam(Game,0.05f,0.01f);
            }
            Camera.Update(Game.Viewport.AspectRatio);
        }

        private Effect crtFilter;

        private void CRTTest_OnRender(GameTime gameTime) {
            RenderEntities(gameTime);
        }

        private Point CRTSize = new Point(800,600);

        private Vector2 CRTTopLeft = new Vector2(-0.766212f,-2.15167f);
        private Vector2 CRTBottomRight = new Vector2(0.766212f,0.974369f);

        private void CRTTest_OnLoad() {
            crtFilter = Game.Content.Load<Effect>("Shaders/crt-filter");
            var monitor = new ModelEntity() {Model = "Models/crt-monitor"};

            var faceWidth = Math.Abs(CRTBottomRight.X - CRTTopLeft.X);
            var faceHeight = Math.Abs(-CRTTopLeft.Y - CRTBottomRight.Y);

            var renderTargetEntity = new RenderTargetEntity() {
                Size = CRTSize,
                Rotation = new Vector3(0f,-90f,180f),
                Position = new Vector3(0f,-0.965f,1.563035f),
                Scale = new Vector3(faceWidth,faceHeight,1f)
            };

            var testTexture = Game.Content.Load<Texture2D>("Test/meta-meme");
            renderTargetEntity.RenderOnTarget = gameTime => {
                float t = (float)(gameTime.TotalGameTime.TotalSeconds / 5d % 1d);
                crtFilter.Parameters["time"].SetValue(t);

                Game.SpriteBatch.Begin(SpriteSortMode.Deferred,effect: crtFilter,samplerState: SamplerState.LinearWrap);
                Game.SpriteBatch.Draw(testTexture,Game.Viewport.Bounds,Color.White);
                Game.SpriteBatch.End();
            };

            var camera = new AngleCamera() {
                NearPlane = 0.1f,
                FieldOfView = 75f,
                Angle = new Vector2(180.15f,195.15f),
                Position = new Vector3(0.08f,-2.47f,1.8f)
            };

            Entities.Create(Entity3DType.GridLines);
            Entities.Add(monitor);
            Entities.Add(renderTargetEntity);

            foreach(var effect in monitor.ModelEffects) {
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

            Camera = camera;
        }

        protected override void ResetGraphicsDeviceState(GraphicsDevice graphicsDevice) {
            graphicsDevice.Clear(Color.LightGray);
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
        }
    }
}
