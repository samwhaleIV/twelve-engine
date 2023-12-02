using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace John {
    public sealed class JohnDecorator {

        public Texture2D Texture => _renderTarget;
        private RenderTarget2D _renderTarget;

        private const int MAX_CONFIGS = 32, JOHN_BLOCK_SIZE = 32, JOHN_HEIGHT = 16;

        private readonly Color[] _outputData, _inputData;
        private readonly Point _sourceDataSize;

        public JohnDecorator(GraphicsDevice graphicsDevice,Texture2D texture,Rectangle textureSource) {
            int size = (int)Math.Sqrt(MAX_CONFIGS / 2) * JOHN_BLOCK_SIZE;
            _renderTarget = new RenderTarget2D(graphicsDevice,size,size,false,SurfaceFormat.Color,DepthFormat.None,0,RenderTargetUsage.PreserveContents);
            _outputData = new Color[size*size];

            _sourceDataSize = textureSource.Size;
            Color[] textureColors = new Color[textureSource.Width * textureSource.Height];
            texture.GetData(0,textureSource,textureColors,0,textureColors.Length);
            _inputData = textureColors;
        }

        private readonly Dictionary<int,Point> spriteOrigins = new();
        private readonly Queue<(int,JohnConfig)> johnConfigs = new();

        public void Unload() {
            _renderTarget?.Dispose();
            _renderTarget = null;
        }

        public void AddConfig(int ID,JohnConfig johnConfig) {
            if(johnConfigs.Count >= MAX_CONFIGS) {
                throw new IndexOutOfRangeException("Too many John configs! The texture will overflow!");
            }
            johnConfigs.Enqueue((ID,johnConfig));
        }

        public void ResetConfigs() {
            johnConfigs.Clear();
        }

        private Color GetSourceColor(int x,int y) {
            return _inputData[y * _sourceDataSize.X + x];
        }

        private void SetOutputColor(int x,int y,Color color) {
            _outputData[y * _renderTarget.Width + x] = color;
        }

        [Flags]
        private enum ColorChannel { None = 0, Red = 1, Blue = 2, Green = 4 }

        private static ColorChannel GetColorChannel(Color color) {
            ColorChannel colorChannel = ColorChannel.None;
            if(color.R > 0) colorChannel |= ColorChannel.Red; if(color.G > 0) colorChannel |= ColorChannel.Green; if(color.B > 0) colorChannel |= ColorChannel.Blue;
            return colorChannel;
        }

        private void DrawConfig(int originX,int originY,(int ID,JohnConfig Value) config) {

            Color red = config.Value.Color1, green = config.Value.Color2, blue = config.Value.Color3;

            for(int x = 0;x<_sourceDataSize.X;x++) {
                for(int y = 0;y<_sourceDataSize.Y;y++) {
                    Color color = GetSourceColor(x,y);
                    color = GetColorChannel(color) switch {
                        ColorChannel.Red => red * color.R, ColorChannel.Green => green * color.G, ColorChannel.Blue => blue * color.B,
                        _ => color
                    };
                    SetOutputColor(originX+x,originY+y,color);
                }
            }
        }

        public void RegenerateTexture() {

            int horizontalBlocks = _renderTarget.Width / JOHN_BLOCK_SIZE, verticalBlocks = _renderTarget.Height / JOHN_BLOCK_SIZE;

            bool breakLoop = false;
            for(int y = 0;y<verticalBlocks && !breakLoop;y++) {
                for(int x = 0;x<horizontalBlocks;x++) {
                    (int ID, JohnConfig Value) config;
                    if(!johnConfigs.TryDequeue(out config)) {
                        breakLoop = true;
                        break;
                    }
                    DrawConfig(x*horizontalBlocks,y * verticalBlocks,config);
                    if(!johnConfigs.TryDequeue(out config)) {
                        breakLoop = true;
                        break;
                    }
                    DrawConfig(x*horizontalBlocks,y * verticalBlocks + JOHN_HEIGHT,config);
                }
            }

            _renderTarget.SetData(_outputData);
        }

        public Point GetTextureOrigin(int configID) {
            return spriteOrigins[configID];
        }
    }
}
