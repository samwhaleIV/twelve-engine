using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using Microsoft.Xna.Framework;
using TwelveEngine.Shell;

namespace TwelveEngine.Effects {
    public sealed class DrawingFrame {

        private readonly int width, height, pixelCount;
        private readonly Vector2 size;

        public int Width => width;
        public int Height => height;
        public int PixelCount => pixelCount;
        public Vector2 Size => size;

        public DrawingFrame(int width,int height) {
            pixelCount = width * height;
            if(pixelCount % 8 != 0) {
                throw new Exception("Drawing frame pixel count must be divisible by 8.");
            }
            this.width = width;
            this.height = height;
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

        public void Reset(GameManager game) {
            game.RenderTarget.Push(RenderTarget);
            game.GraphicsDevice.Clear(ClearOptions.Target,EmptyColor,1,0);
            game.RenderTarget.Pop();
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

            game.RenderTarget.Push(RenderTarget);
            game.SpriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);

            Vector2 difference = oldLocation - newLocation;
            int steps = (int)Vector2.Distance(oldLocation,newLocation);
            steps = Math.Max(steps,1);

            Vector2 brushScale = brushSize / BrushTexture.Bounds.Size.ToVector2();

            for(int i = 0;i<steps;i++) {
                Vector2 interpolatedPosition = newLocation + difference * ((float)i / steps);
                interpolatedPosition = Vector2.Floor(interpolatedPosition);
                game.SpriteBatch.Draw(BrushTexture,interpolatedPosition,null,drawingColor,0f,Vector2.Zero,brushScale,SpriteEffects.None,1f);
            }

            game.SpriteBatch.End();
            game.RenderTarget.Pop();

            lastDrawLocation = location;
        }

        public Color EmptyColor { get; set; } = Color.Black;
        public Color DrawColor { get; set; } = Color.White;

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

        private const int BYTE_SIZE = 8;

        public byte[] Export() {
            byte[] data = new byte[PixelCount / BYTE_SIZE];

            Color[] colorData = new Color[PixelCount];
            RenderTarget.GetData(colorData,0,colorData.Length);

            BitArray bitBuffer = new(new bool[BYTE_SIZE]);

            byte drawColorA = DrawColor.A; /* Only check the alpha value... YOLO. Let's speed this up. */

            /* Color data is validated to be divisible by 8 in the DrawingFrame constructor. */
            for(int colorIndex = 0;colorIndex<colorData.Length;colorIndex += BYTE_SIZE) {

                /* MORE. SPEED. */
                bitBuffer[0] = colorData[colorIndex+0].A == drawColorA;
                bitBuffer[1] = colorData[colorIndex+1].A == drawColorA;
                bitBuffer[2] = colorData[colorIndex+2].A == drawColorA;
                bitBuffer[3] = colorData[colorIndex+3].A == drawColorA;
                bitBuffer[4] = colorData[colorIndex+4].A == drawColorA;
                bitBuffer[5] = colorData[colorIndex+5].A == drawColorA;
                bitBuffer[6] = colorData[colorIndex+6].A == drawColorA;
                bitBuffer[7] = colorData[colorIndex+7].A == drawColorA;

                bitBuffer.CopyTo(data,colorIndex / 8);
            }

            return data;
        }
    }
}
