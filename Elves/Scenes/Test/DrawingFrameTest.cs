using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TwelveEngine;

namespace Elves.Scenes.Test {

    public sealed class DrawingFrameTest:Scene {
        public DrawingFrameTest() {
            OnLoad += DrawingFrameTest_OnLoad;
            OnUnload += DrawingFrameTest_OnUnload;
            OnRender += DrawingFrameTest_OnRender;
            ClearColor = Color.DarkGray;
        }

        private Rectangle frameDestination;

        private void UpdateFrameDestination() {
            var bounds = new VectorRectangle(Game.Viewport.Bounds);
            float scale = bounds.Height / drawingFrame.Height * 0.75f;
            Vector2 drawingFrameSize = drawingFrame.Size * scale;
            Vector2 origin = bounds.Center - drawingFrameSize * 0.5f;
            frameDestination = new Rectangle((int)origin.X,(int)origin.Y,(int)drawingFrameSize.X,(int)drawingFrameSize.Y);
        }

        protected override void UpdateGame() {
            UpdateFrameDestination();
            UpdateInputs();
            ProcessMouseDrawing();
            UpdateCameraScreenSize();
            Entities.Update();
            UpdateCamera();
        }

        private void ProcessMouseDrawing() {
            if(!Mouse.Capturing) {
                drawingFrame.ReleaseDraw();
                return;
            }
            Vector2 relativePosition = (Mouse.Position.ToVector2() - frameDestination.Location.ToVector2()) / frameDestination.Size.ToVector2();
            drawingFrame.Draw(Game,relativePosition);
        }

        private void DrawingFrameTest_OnRender() {
            Game.SpriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);
            Game.SpriteBatch.Draw(drawingFrame.RenderTarget,frameDestination,Color.White);
            Game.SpriteBatch.End();
        }

        private void DrawingFrameTest_OnUnload() {
            drawingFrame?.Unload();
            drawingFrame = null;
        }

        private DrawingFrame drawingFrame = new(256,128) { BrushTexture = Program.Textures.Circle, BrushSize = 5 };

        private void DrawingFrameTest_OnLoad() {
            drawingFrame.Load(Game.GraphicsDevice);
            var byteCount = drawingFrame.PixelCount / 8;
            var array = new byte[byteCount];
            drawingFrame.Import(array);
        }
    }
}
