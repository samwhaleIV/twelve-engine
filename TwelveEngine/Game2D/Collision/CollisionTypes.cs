using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D {

    public static class CollisionTypes {

        private const int TILE_SIZE = 16;

        private static readonly Rectangle collisionTileRange = new Rectangle(256,0,256,64);

        private static readonly Dictionary<int,CollisionType> types = new Dictionary<int,CollisionType>();

        private static (Color[,] pixels,int tilesetColumns) getCollisionSlice(ContentManager contentManager) {
            var texture = contentManager.Load<Texture2D>(Constants.Tileset);

            var width = texture.Width;
            var height = texture.Height;

            var pixelsLinear = new Color[width * height];
            texture.GetData(pixelsLinear);

            var sliceWidth = collisionTileRange.Width;
            var sliceHeight = collisionTileRange.Height;

            var pixels = new Color[sliceWidth,sliceHeight];

            var xOffset = collisionTileRange.X;
            var yOffset = collisionTileRange.Y;

            for(var x = 0;x<sliceWidth;x++) {
                for(var y = 0;y<sliceHeight;y++) {
                    pixels[x,y] = pixelsLinear[(xOffset + x) + (yOffset + y) * width];
                }
            }

            return (pixels, width / TILE_SIZE);
        }

        private static readonly Color CollisionColor = new Color(128,0,0,128);

        private static CollisionType? getCollisionType(Color[,] pixels,int xOffset,int yOffset) {

            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;

            for(var x = 0;x < TILE_SIZE;x++) {
                bool hadMatch = false;
                for(var y = 0;y < TILE_SIZE;y++) {
                    var color = pixels[x + xOffset,y + yOffset];
                    if(color != CollisionColor) {
                        continue;
                    }
                    hadMatch = true;
                    if(y < minY) minY = y;
                    if(y > maxY) maxY = y;
                }
                if(hadMatch) {
                    if(x < minX) minX = x;
                    if(x > maxX) maxX = x;
                }
            }

            if(minX == int.MaxValue || minY == int.MaxValue) {
                return null;
            }

            return new CollisionType(minX,minY,maxX - minX + 1,maxY - minY + 1,TILE_SIZE);
        }

        public static void LoadTypes(ContentManager contentManager) {
            var (pixels,tilestColumns) = getCollisionSlice(contentManager);

            var width = pixels.GetLength(0);
            var height = pixels.GetLength(1);

            var tileWidth = width / TILE_SIZE;
            var tileHeight = height / TILE_SIZE;

            var xOffset = collisionTileRange.X / TILE_SIZE;
            var yOffset = collisionTileRange.Y / TILE_SIZE;

            for(var x = 0;x<tileWidth;x++) {
                for(var y = 0;y<tileHeight;y++) {
                    var type = getCollisionType(pixels,x*TILE_SIZE,y*TILE_SIZE);
                    if(!type.HasValue) {
                        continue;
                    }
                    var tilesetIndex = (x+xOffset) + (y+yOffset) * tilestColumns;
                    types[tilesetIndex] = type.Value;
                }
            }
        }

        public static Hitbox? getHitbox(int ID,float x,float y) {
            if(!types.ContainsKey(ID)) {
                return null;
            }
            var hitbox = new Hitbox();
            var collisionType = types[ID];
            hitbox.X = x + collisionType.X;
            hitbox.Y = y + collisionType.Y;
            hitbox.Width = collisionType.Width;
            hitbox.Height = collisionType.Height;
            return hitbox;
        }
    }
}
