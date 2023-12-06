using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine;

namespace John {
    public readonly struct NineGrid {
        public int Size { get; init; }
        public Rectangle[,] Value { get; init; }
        public Texture2D Texture { get; init; }

        public NineGrid(Texture2D texture,int tileSize,Point origin) {
            Point size = new(tileSize),
                x1 = new(tileSize,0), x2 = new(tileSize*2,0),
                y1 = new(0,tileSize), y2 = new(0,tileSize*2);

            Value = new Rectangle[3,3] {
                { new(origin,size), new(origin+y1,size), new(origin+y2,size)},
                { new(origin+x1,size), new(origin+x1+y1,size), new(origin+x1+y2,size)},
                { new(origin+x2,size), new(origin+x2+y1,size), new(origin+x2+y2,size) }
            };

            Texture = texture;
            Size = tileSize;
        }

        public void Draw(SpriteBatch spriteBatch,float pixelScale,FloatRectangle area,float layerDepth = 0.5f) {
            Vector2 cornerSize = new Vector2(pixelScale * Size);
            Vector2 centerSize = area.Size - cornerSize * 2;

            //(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)

            spriteBatch.Draw(Texture,area.Position,Value[0,0],Color.White,0f,Vector2.Zero,pixelScale,SpriteEffects.None,layerDepth);
            spriteBatch.Draw(Texture,area.Position + new Vector2(centerSize.X + cornerSize.X,0),Value[2,0],Color.White,0f,Vector2.Zero,pixelScale,SpriteEffects.None,layerDepth);

            spriteBatch.Draw(Texture,area.Position + new Vector2(0,centerSize.Y + cornerSize.Y),Value[0,2],Color.White,0f,Vector2.Zero,pixelScale,SpriteEffects.None,layerDepth);
            spriteBatch.Draw(Texture,area.Position + new Vector2(centerSize.X + cornerSize.X,centerSize.Y + cornerSize.Y),Value[2,2],Color.White,0f,Vector2.Zero,pixelScale,SpriteEffects.None,layerDepth);

            Vector2 centerScale = centerSize / Size;
            Vector2 columnScale = new Vector2(pixelScale,centerScale.Y);
            Vector2 rowScale = new Vector2(centerScale.X,pixelScale);

            spriteBatch.Draw(Texture,area.Position + new Vector2(cornerSize.X,cornerSize.Y),Value[1,1],Color.White,0f,Vector2.Zero,centerScale,SpriteEffects.None,layerDepth);

            spriteBatch.Draw(Texture,area.Position + new Vector2(cornerSize.X,0),Value[1,0],Color.White,0f,Vector2.Zero,rowScale,SpriteEffects.None,layerDepth);
            spriteBatch.Draw(Texture,area.Position + new Vector2(cornerSize.X,centerSize.Y + cornerSize.Y),Value[1,2],Color.White,0f,Vector2.Zero,rowScale,SpriteEffects.None,layerDepth);

            spriteBatch.Draw(Texture,area.Position + new Vector2(0,cornerSize.Y),Value[0,1],Color.White,0f,Vector2.Zero,columnScale,SpriteEffects.None,layerDepth);
            spriteBatch.Draw(Texture,area.Position + new Vector2(centerSize.X + cornerSize.X,cornerSize.Y),Value[2,1],Color.White,0f,Vector2.Zero,columnScale,SpriteEffects.None,layerDepth);
        }
    }
}
