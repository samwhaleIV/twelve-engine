using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwelveEngine.Game2D {


    public struct CollisionType {
        public CollisionType(
            int x,int y,int width,int height,float tileSize
        ) {
            this.X = x / tileSize;
            this.Y = y / tileSize;
            this.Width = width / tileSize;
            this.Height = height / tileSize;
        }
        public float X;
        public float Y;
        public float Width;
        public float Height;
    }
    public static class CollisionTypes {

        private const int TILE_SIZE = 16;

        private static readonly Rectangle collisionTileRange = new Rectangle(256,0,112,80);

        private static readonly Dictionary<int,CollisionType> types = new Dictionary<int,CollisionType>();

        private static Color[,] getCollisionTileset(ContentManager contentManager) {
            var texture = contentManager.Load<Texture2D>(Constants.Tileset);

            var width = texture.Width;
            var height = texture.Height;

            var pixelsLinear = new Color[width * height];
            texture.GetData(pixelsLinear);
            texture.Dispose();

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

            return pixels;
        }


        public static void LoadTypes(ContentManager contentManager) {
            var tileset = getCollisionTileset(contentManager);
            var width = tileset.GetLength(0);
            var height = tileset.GetLength(1);

            //iterate tileset
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
