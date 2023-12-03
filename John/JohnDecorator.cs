using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace John {

    using static Constants;

    public sealed class JohnDecorator {

        public Texture2D Texture => _renderTarget;
        private RenderTarget2D _renderTarget;

        private readonly Color[] _outputData, _inputData;
        private readonly Point _sourceDataSize;

        public JohnDecorator(GraphicsDevice graphicsDevice,Texture2D texture,Rectangle textureSource) {
            int size = (int)Math.Sqrt(JOHN_CONFIG_COUNT / 2) * JOHN_BLOCK_SIZE;
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
            if(johnConfigs.Count >= JOHN_CONFIG_COUNT) {
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

        private static Color MultiplyPreserveAlpha(Color value,float scale) {
            return new Color((int)(value.R * scale),(int)(value.G * scale),(int)(value.B * scale),value.A);
        }

        private static Color ChannelShading(Color color,byte gradient) {
            return MultiplyPreserveAlpha(color,(float)gradient / byte.MaxValue);
        }

        private void DrawConfig(Point origin,JohnConfig config) {
            /* Shaded colors */
            Color r = config.Color1, g = config.Color2, b = config.Color3;

            for(int x = 0;x<_sourceDataSize.X;x++) {
                for(int y = 0;y<_sourceDataSize.Y;y++) {
                    Color color = GetSourceColor(x,y);
                    color = GetColorChannel(GetSourceColor(x,y)) switch {
                        ColorChannel.Red => ChannelShading(r,color.R), ColorChannel.Green => ChannelShading(g,color.G), ColorChannel.Blue => ChannelShading(b,color.B),
                        _ => color
                    };
                    SetOutputColor(origin.X+x,origin.Y+y,color);
                }
            }
        }

        public void GenerateTexture() {
            spriteOrigins.Clear();

            int horizontalBlocks = _renderTarget.Width / JOHN_BLOCK_SIZE, verticalBlocks = _renderTarget.Height / JOHN_BLOCK_SIZE;

            bool breakLoop = false;
            for(int y = 0;y<verticalBlocks && !breakLoop;y++) {
                for(int x = 0;x<horizontalBlocks;x++) {
                    (int ID, JohnConfig Value) config;
                    if(!johnConfigs.TryDequeue(out config)) {
                        breakLoop = true;
                        break;
                    }
                    Point origin = new Point(x * JOHN_BLOCK_SIZE,y * JOHN_BLOCK_SIZE);
                    DrawConfig(origin,config.Value);
                    spriteOrigins.Add(config.ID,origin);

                    if(!johnConfigs.TryDequeue(out config)) {
                        breakLoop = true;
                        break;
                    }
                    origin.Y += JOHN_HEIGHT;
                    DrawConfig(origin,config.Value);
                    spriteOrigins.Add(config.ID,origin);
                }
            }

            _renderTarget.SetData(_outputData);
        }

        public void WriteTextureToFile() {
            using(Stream file = File.Create("decorator_debug.png")) {
                _renderTarget.SaveAsPng(file,_renderTarget.Width,_renderTarget.Height);
            }
        }

        public Point GetTextureOrigin(int configID) {
            return spriteOrigins[configID];
        }
    }
}
