﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game2D.Collision {

    public sealed class CollisionTypes {

        private readonly Dictionary<int,CollisionType> types = new Dictionary<int,CollisionType>();

        private readonly int tileSize;
        private readonly string textureName;

        private readonly Color color;
        private readonly Rectangle area;

        public CollisionTypes(
            int tileSize = Constants.DefaultTileSize,
            string textureName = Constants.Tileset,
            Rectangle? area = null,
            Color? color = null
        ) {
            this.tileSize = tileSize;
            this.textureName = textureName;
            this.area = area ?? Tiles.CollisionArea;
            this.color = color ?? Tiles.CollisionColor;
        }

        private (Color[,] pixels,int tilesetColumns) getCollisionSlice() {
            var texture = CPUTexture.Get(textureName);

            var width = texture.Width;
            var data = texture.Data;

            var sliceWidth = area.Width;
            var sliceHeight = area.Height;

            var pixels = new Color[sliceWidth,sliceHeight];
            var offset = area.Location;

            for(var x = 0;x<sliceWidth;x++) {
                for(var y = 0;y<sliceHeight;y++) {
                    pixels[x,y] = data[x+offset.X,y+offset.Y];
                }
            }

            return (pixels, width / tileSize);
        }

        private CollisionType? getCollisionType(Color[,] pixels,int xOffset,int yOffset) {

            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;

            for(var x = 0;x < tileSize;x++) {
                bool hadMatch = false;
                for(var y = 0;y < tileSize;y++) {
                    var color = pixels[x + xOffset,y + yOffset];
                    if(color != this.color) {
                        continue;
                    }
                    hadMatch = true;

                    /* At first glance, this might like like min-maxing the value, a-la 'Math.Max' / 'Math.Min': It's not.
                     * This is to find the smallest and largest values over multiple iterations */

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

            return new CollisionType(minX,minY,maxX - minX + 1,maxY - minY + 1,tileSize);
        }

        internal void LoadTypes() {
            var (pixels,tilestColumns) = getCollisionSlice();

            var width = pixels.GetLength(0);
            var height = pixels.GetLength(1);

            var tileWidth = width / tileSize;
            var tileHeight = height / tileSize;

            var xOffset = area.X / tileSize;
            var yOffset = area.Y / tileSize;

            for(var x = 0;x<tileWidth;x++) {
                for(var y = 0;y<tileHeight;y++) {
                    var type = getCollisionType(pixels,x*tileSize,y*tileSize);
                    if(!type.HasValue) {
                        continue;
                    }
                    var tilesetIndex = (x+xOffset) + (y+yOffset) * tilestColumns;
                    types[tilesetIndex] = type.Value;
                }
            }
        }

        public Hitbox? GetHitbox(int ID,float x,float y) {
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
