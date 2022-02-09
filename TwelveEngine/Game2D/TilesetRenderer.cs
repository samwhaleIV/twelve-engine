using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D {
    public sealed class TilesetRenderer:TileRenderer {

        private void initialize() => OnLoad += TilesetRenderer_OnLoad;

        public TilesetRenderer() => initialize();

        private string pendingTileset;
        private Texture2D tileset;
        private SpriteBatch spriteBatch;
        private Rectangle[] textureSources;

        public TilesetRenderer(string tileset) {
            pendingTileset = tileset;
            initialize();
        }
        public TilesetRenderer(Texture2D tileset) {
            this.tileset = tileset;
            initialize();
        }

        private void loadTileset() {
            if(tileset != null) {
                return;
            }
            if(pendingTileset == null) {
                pendingTileset = Constants.Config.Tileset;
            }
            tileset = Game.Content.Load<Texture2D>(pendingTileset);
            pendingTileset = null;
        }

        private void TilesetRenderer_OnLoad() {
            loadTileset();

            int tileSize = Grid.TileSize;
            spriteBatch = Game.SpriteBatch;

            int rows = tileset.Height / tileSize;
            int columns = tileset.Width / tileSize;

            textureSources = getTextureSources(rows * columns,columns,tileSize);
        }

        private static Rectangle[] getTextureSources(int count,int columns,int size) {
            var sources = new Rectangle[count];
            for(int i = 0; i < sources.Length;i++) {
                var source = new Rectangle();
                source.X = (i % columns) * size;
                source.Y = (i / columns) * size;
                source.Width = size;
                source.Height = size;
                sources[i] = source;
            }
            return sources;
        }

        private (Point start, Point tile, Point size, Point offset,int tileSize) tileArea;

        private void draw(Point start,Point tile,Point size,Point offset,int tileSize,int[,] data) {
            var target = new Rectangle(Point.Zero,new Point(tileSize));

            float depthBase = 1f / Grid.Viewport.Height;
            int gridX, value, y;
            float depth;

            for(int x = offset.X;x < size.X;x++) {
                gridX = x + start.X;
                target.X = tile.X + x * tileSize;
                for(y = offset.Y;y < size.Y;y++) {
                    value = data[gridX,y + start.Y];
                    if(value < 1) continue;

                    target.Y = tile.Y + y * tileSize;

                    depth = target.Y * depthBase;
                    if(depth < 0) depth = 0;

                    spriteBatch.Draw(tileset,target,textureSources[value],Color.White,0f,Vector2.Zero,SpriteEffects.None,depth);
                }
            }
        }

        public override void CacheArea(ScreenSpace screenSpace) {
            Point start = screenSpace.Location.ToPoint();
            Vector2 render = start.ToVector2() - screenSpace.Location;

            Point size = Vector2.Ceiling(screenSpace.Size - render).ToPoint();

            int tileSize = screenSpace.TileSize;
            if(render.X * -2 > tileSize) size.X++;
            if(render.Y * -2 > tileSize) size.Y++;

            Point tile = Vector2.Round(render * tileSize).ToPoint();

            Point offset = Point.Zero;
            if(start.X < 0) offset.X = -start.X;
            if(start.Y < 0) offset.Y = -start.Y;

            Point end = start + size;
            if(end.X > Grid.Columns) size.X -= end.X - Grid.Columns;
            if(end.Y > Grid.Rows) size.Y -= end.Y - Grid.Rows;

            tileArea = (start, tile, size, offset, tileSize);
        }

        public override void RenderTiles(int[,] data) {
            draw(tileArea.start,tileArea.tile,tileArea.size,tileArea.offset,tileArea.tileSize,data);
        }
    }
}
