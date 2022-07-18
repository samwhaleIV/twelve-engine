using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TwelveEngine.Shell;

namespace Porthole.Collision {

    public sealed class TileCollisionTypes {

        private readonly Dictionary<int,TileCollisionType> types = new Dictionary<int,TileCollisionType>();

        private readonly int tileSize;
        private readonly string textureName;

        private readonly Color color;
        private readonly Rectangle area;

        public TileCollisionTypes(   
            Rectangle area,
            Color color,           
            int? tileSize = null,
            string textureName = null
        ) {
            if(string.IsNullOrWhiteSpace(textureName)) {
                textureName = TwelveEngine.Constants.Config.Tileset;
            }
            if(!tileSize.HasValue) {
                tileSize = TwelveEngine.Constants.Config.TileSize;
            }
            this.tileSize = tileSize.Value;
            this.textureName = textureName;
            this.area = area;
            this.color = color;
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

        private TileCollisionType? getCollisionType(Color[,] pixels,int xOffset,int yOffset) {

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

            return new TileCollisionType(new Point(minX,minY),new Point(maxX - minX + 1,maxY - minY + 1),tileSize);
        }

        internal bool IsLoaded { get; private set; }

        internal void Load() {
            if(IsLoaded) return;

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
            IsLoaded = true;
        }

        public Hitbox? GetHitbox(int ID,Vector2 location) {
            if(!types.ContainsKey(ID)) {
                return null;
            }

            var collisionType = types[ID];

            var hitbox = new Hitbox(
                location + collisionType.Location,collisionType.Size
            );
            return hitbox;
        }

        public Hitbox? GetHitbox(int ID,Point location) {
            return GetHitbox(ID,location.ToVector2());
        }
    }
}
