using TwelveEngine.Game2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JewelEditor {
    public sealed class GridLines:Entity2D {
        protected override int GetEntityType() => (int)EntityTypes.GridLines;

        public GridLines() {
            OnRender +=  GridLines_OnRender;
            OnLoad += GridLines_OnLoad;
            OnUnload += GridLines_OnUnload;
        }

        private Texture2D texture;

        private readonly Rectangle textureSource = new Rectangle(0,0,1,1);

        private void GridLines_OnUnload() {
            texture?.Dispose();
            texture = null;
        }

        private void GridLines_OnLoad() {
            texture = new Texture2D(Owner.Game.GraphicsDevice,1,1);
            texture.SetData(new Color[] { Color.Black });
        }

        private void DrawLine(Point point,Point size) {
            Owner.Game.SpriteBatch.Draw(texture,new Rectangle(point,size),textureSource,Color.White,0f,Vector2.Zero,SpriteEffects.None,1f);
        }

        private void DrawLines(Point start,Point end,Point offset,float tileSize) {
            Point horizontalLine = new Point((int)tileSize,1), verticalLine = new Point(1,(int)tileSize);
            for(int x = start.X; x < end.X; x++) {
                for(int y = start.Y;y < end.Y;y++) {
                    var point = new Point((int)(x * tileSize),(int)(y * tileSize)) + offset;
                    DrawLine(point,horizontalLine); DrawLine(point,verticalLine);
                }
            }
        }

        private void GridLines_OnRender(GameTime gameTime) {
            var screenSpace = Owner.ScreenSpace;

            Vector2 start = Vector2.Floor(screenSpace.Position);
            Point end = (start + Vector2.Ceiling(screenSpace.Position - start + screenSpace.Size)).ToPoint();

            float tileSize = screenSpace.TileSize;
            Point offset = (-screenSpace.Position * tileSize).ToPoint();
            DrawLines(start.ToPoint(),end,offset,tileSize);
        }
    }
}
