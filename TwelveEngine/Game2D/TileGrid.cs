using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine.Serial;
using TwelveEngine.Serial.Map;

namespace TwelveEngine.Game2D {
    public class TileGrid:Grid2D {

        public LayerMode LayerMode { get; set; } = LayerModes.Default;

        private int[][,] layers;

        public TileGrid() {
            OnLoad += TileGrid_OnLoad;
            OnUnload += TileGrid_OnUnload;
        }

        private void TileGrid_OnUnload() {
            TileRenderer?.Unload();
        }

        private void TileGrid_OnLoad() {
            TileRenderer?.Load(this);
        }

        private void RenderLayers(int start,int length) {
            if(TileRenderer == null) {
                return;
            }
            int end = start + length;
            for(int i = start;i<end;i++) {
                TileRenderer.RenderTiles(layers[i]);
            }
        }

        public TileRenderer TileRenderer { get; set; }

        public Point GetTile(Vector2 location) => Vector2.Floor(location).ToPoint();
        public Point GetTile(Point screenPoint) => GetTile(GetWorldVector(screenPoint));

        public bool TryGetTile(Point screenPoint,out Point tile) {
            tile = GetTile(screenPoint);
            return UnitInRange(tile);
        }

        public bool TryGetTile(Vector2 location,out Point tile) {
            tile = GetTile(location);
            return UnitInRange(tile);
        }

        public void ImportMap(Map map) {
            layers = map.Layers2D;
            Size = new Vector2(map.Width,map.Height);
        }

        public void Fill(int layerIndex,Func<int,int,int> pattern) {
            var map2D = layers[layerIndex];
            int width = UnitWidth, height = UnitHeight;
            for(var x = 0;x < width;x++) {
                for(var y = 0;y < height;y++) {
                    map2D[x,y] = pattern(x,y);
                }
            }
        }

        public int[,] GetLayer(int index) => layers[index];

        protected override void RenderGrid(GameTime gameTime) {

            TileRenderer?.CacheArea(ScreenSpace);
            Game.GraphicsDevice.Clear(Color.Black);

            bool drawing = false;
            SpriteBatch spriteBatch = Game.SpriteBatch;

            if(LayerMode.Background) {
                if(LayerMode.BackgroundLength == 2) {
                    spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
                    RenderLayers(LayerMode.BackgroundStart,1);
                    spriteBatch.End();

                    spriteBatch.Begin(SpriteSortMode.BackToFront,null,SamplerState.PointClamp);
                    RenderLayers(LayerMode.BackgroundStart+1,1);
                    RenderEntities(gameTime);
                    spriteBatch.End();
                } else {
                    spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
                    RenderLayers(LayerMode.BackgroundStart,LayerMode.BackgroundLength);
                    RenderEntities(gameTime);
                    drawing = true;
                }
            } else {
                spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
                RenderEntities(gameTime);
                drawing = true;
            }

            if(!drawing) spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);

            if(LayerMode.Foreground) {
                RenderLayers(LayerMode.ForegroundStart,LayerMode.ForegroundLength);
            }

            InputGuide.Render();
            spriteBatch.End();
        }
    }
}
