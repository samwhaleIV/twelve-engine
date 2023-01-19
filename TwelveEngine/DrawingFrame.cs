using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using Microsoft.Xna.Framework;
using TwelveEngine.Shell;

namespace TwelveEngine {
    public sealed class DrawingFrame {

        private readonly int width, height, pixelCount;
        private readonly Vector2 size;

        public int Width => width;
        public int Height => height;
        public int PixelCount => pixelCount;
        public Vector2 Size => size;

        public DrawingFrame(int width,int height) {
            this.width = width;
            this.height = height;
            pixelCount = width * height;
            size = new(width,height);
        }

        public RenderTarget2D RenderTarget { get; private set; } = null;

        public void Load(GraphicsDevice graphicsDevice) {
            RenderTarget = new RenderTarget2D(graphicsDevice,width,height,false,SurfaceFormat.Color,DepthFormat.None,0,RenderTargetUsage.PreserveContents);
        }

        public void Unload() {
            RenderTarget?.Dispose();
            RenderTarget = null;
        }

        private Vector2? lastDrawLocation = null;

        public void ReleaseDraw() {
            lastDrawLocation = null;
        }

        public Texture2D BrushTexture { get; set; } = null;

        public void Draw(GameManager game,Vector2 location) {

            Color drawingColor = DrawColor;

            if(lastDrawLocation == location) {
                return;
            }

            if(BrushTexture is null) {
                lastDrawLocation = location;
                return;
            }

            Vector2 oldLocation = (lastDrawLocation ?? location) * Size;
            Vector2 newLocation = location * Size;

            Vector2 brushSize = new(BrushSize);
            Vector2 halfBrushSize = brushSize * 0.5f;

            newLocation -= halfBrushSize; oldLocation -= halfBrushSize;

            game.PushRenderTarget(RenderTarget);
            game.SpriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);

            Vector2 difference = oldLocation - newLocation;
            int steps = (int)Vector2.Distance(oldLocation,newLocation);
            steps = Math.Max(steps,1);

            Vector2 brushScale = brushSize / BrushTexture.Bounds.Size.ToVector2();

            for(int i = 0;i<steps;i++) {
                Vector2 interpolatedPosition = newLocation + difference * i / steps;
                game.SpriteBatch.Draw(BrushTexture,interpolatedPosition,null,drawingColor,0f,Vector2.Zero,brushScale,SpriteEffects.None,1f);
            }

            game.SpriteBatch.End();
            game.PopRenderTarget();

            lastDrawLocation = location;
        }

        public Color EmptyColor { get; private set; } = Color.Black;
        public Color DrawColor { get; private set; } = Color.White;

        public float BrushSize { get; set; } = 4;

        public void Import(byte[] data) {
            if(data is null) {
                throw new ArgumentNullException(nameof(data));
            }
            if(RenderTarget is null) {
                throw new NullReferenceException("Cannot import image data before loading the RenderTarget.");
            }
            BitArray bits = new(data);
            Color[] colorData = new Color[PixelCount];
            int end = Math.Min(bits.Count,PixelCount);
            int bitIndex = 0;
            for(int i = 0;i < end;i++) {
                colorData[i] = bits[bitIndex++] ? DrawColor : EmptyColor;
            }
            RenderTarget.SetData(colorData,0,PixelCount);
        }
    }
}
