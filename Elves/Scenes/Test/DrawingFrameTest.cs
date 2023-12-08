using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TwelveEngine;
using TwelveEngine.Effects;
using TwelveEngine.Shell;

namespace Elves.Scenes.Test {

    public sealed class DrawingFrameTest:InputGameState {
        public DrawingFrameTest() {
            OnLoad.Add(Load);
            OnUnload.Add(Unload);
            OnRender.Add(Render);
            OnUpdate.Add(Update);
            ClearColor = Color.DarkGray;
        }

        private void Update() {
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
            if(!MouseHandler.Capturing) {
                drawingFrame.ReleaseDraw();
                return;
            }
            Vector2 relativePosition = (MouseHandler.Position.ToVector2() - frameDestination.Location.ToVector2()) / frameDestination.Size.ToVector2();
            drawingFrame.Draw(this,relativePosition);
        }

        private void Render() {
            SpriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);
            SpriteBatch.Draw(drawingFrame.RenderTarget,frameDestination,Color.White);
            SpriteBatch.End();
        }

        private void Unload() {
            drawingFrame?.Unload();
            drawingFrame = null;
        }

        private DrawingFrame drawingFrame = new(256,128) { BrushTexture = Program.Textures.CircleBrush, BrushSize = 5 };

        private void Load() {
            drawingFrame.Load(GraphicsDevice);
            var byteCount = drawingFrame.PixelCount / 8;
            var array = new byte[byteCount];
            drawingFrame.Import(array);
        }
    }
}
