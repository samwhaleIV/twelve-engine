using TwelveEngine.Game2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JewelEditor.Entity {
    internal sealed class GridLines:JewelEntity {
        protected override int GetEntityType() => JewelEntities.GridLines;

        public GridLines() {
            OnRender +=  GridLines_OnRender;
            OnLoad += GridLines_OnLoad;
            OnUnload += GridLines_OnUnload;
        }

        private Texture2D texture;

        private const int LINE_TEXTURE_SIZE = 32;

        private readonly Rectangle verticalLineSource = new Rectangle(0,0,1,LINE_TEXTURE_SIZE);
        private readonly Rectangle horizontalLineSource = new Rectangle(0,0,LINE_TEXTURE_SIZE,1);

        private static readonly Color GridLineColor = Color.DarkGray;

        private void GridLines_OnUnload() {
            texture?.Dispose();
            texture = null;
        }

        private void GridLines_OnLoad() {
            texture = new Texture2D(Game.GraphicsDevice,LINE_TEXTURE_SIZE,LINE_TEXTURE_SIZE);
            var textureData = new Color[LINE_TEXTURE_SIZE * LINE_TEXTURE_SIZE];
            for(int i = 0;i<textureData.Length;i++) {
                textureData[i] = i % 2 == (i / LINE_TEXTURE_SIZE % 2 == 0 ? 0 : 1) ? GridLineColor : Color.Transparent;
            }
            texture.SetData(textureData);
        }

        private void DrawLine(Rectangle source,Point point,Point size) {
            Game.SpriteBatch.Draw(texture,new Rectangle(point,size),source,Color.White,0f,Vector2.Zero,SpriteEffects.None,1f);
        }

        private void DrawLines(Point start,Point end,Point offset,float tileSize) {
            Point horizontalLine = new Point((int)tileSize,1), verticalLine = new Point(1,(int)tileSize);
            for(int x = start.X; x < end.X; x++) {
                for(int y = start.Y;y < end.Y;y++) {
                    var point = new Point((int)(x * tileSize),(int)(y * tileSize)) + offset;
                    DrawLine(horizontalLineSource,point,horizontalLine);
                    DrawLine(verticalLineSource,point,verticalLine);
                }
            }
        }

        private void GridLines_OnRender(GameTime gameTime) {
            var screenSpace = Owner.ScreenSpace;

            Vector2 start = Vector2.Floor(screenSpace.Location);
            Point end = (start + Vector2.Ceiling(screenSpace.Location - start + screenSpace.Size)).ToPoint();

            float tileSize = screenSpace.TileSize;
            Point offset = (-screenSpace.Location * tileSize).ToPoint();
            DrawLines(start.ToPoint(),end,offset,tileSize);
        }
    }
}
