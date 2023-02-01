using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TwelveEngine;
using TwelveEngine.Effects;

namespace Elves.Scenes.Test {

    public sealed class DrawingFrameTest:Scene {
        public DrawingFrameTest() {
            OnLoad += DrawingFrameTest_OnLoad;
            OnUnload += DrawingFrameTest_OnUnload;
            OnRender += DrawingFrameTest_OnRender;
            OnUpdate += DrawingFrameTest_OnUpdate;
            ClearColor = Color.DarkGray;
        }

        private void DrawingFrameTest_OnUpdate() {
            UpdateFrameDestination();
            ProcessMouseDrawing();
        }

        private Rectangle frameDestination;

        private void UpdateFrameDestination() {
            var bounds = new FloatRectangle(Viewport.Bounds);
            float scale = bounds.Height / drawingFrame.Height * 0.75f;
            Vector2 drawingFrameSize = drawingFrame.Size * scale;
            Vector2 origin = bounds.Center - drawingFrameSize * 0.5f;
            frameDestination = new Rectangle((int)origin.X,(int)origin.Y,(int)drawingFrameSize.X,(int)drawingFrameSize.Y);
        }

        private void ProcessMouseDrawing() {
            if(!Mouse.Capturing) {
                drawingFrame.ReleaseDraw();
                return;
            }
            Vector2 relativePosition = (Mouse.Position.ToVector2() - frameDestination.Location.ToVector2()) / frameDestination.Size.ToVector2();
            drawingFrame.Draw(this,relativePosition);
        }

        private void DrawingFrameTest_OnRender() {
            SpriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);
            SpriteBatch.Draw(drawingFrame.RenderTarget,frameDestination,Color.White);
            SpriteBatch.End();
        }

        private void DrawingFrameTest_OnUnload() {
            drawingFrame?.Unload();
            drawingFrame = null;
        }

        private DrawingFrame drawingFrame = new(256,128) { BrushTexture = Program.Textures.CircleBrush, BrushSize = 5 };

        private void DrawingFrameTest_OnLoad() {
            drawingFrame.Load(GraphicsDevice);
            var byteCount = drawingFrame.PixelCount / 8;
            var array = new byte[byteCount];
            drawingFrame.Import(array);
        }
    }
}
