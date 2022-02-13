using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TwelveEngine.Game2D.Collision {
    public sealed class CollisionInterface {

        private readonly Grid2D owner;
        public CollisionInterface(Grid2D owner) => this.owner = owner;

        public CollisionTypes Types => owner.CollisionTypes;
        public int CollisionLayer { get; set; } = Constants.CollisionLayerIndex;

        public Hitbox? GetHitbox(int ID,Vector2 location) {
            return Types?.GetHitbox(ID,location);
        }
        public Hitbox? GetHitbox(int ID,Point location) {
            return Types?.GetHitbox(ID,location);
        }

        private const int MATRIX_SIZE = 9;
        private static readonly int[,] surroundingMatrix = new int[MATRIX_SIZE,2] {
            {0,0},{0,-1},{0,1},{-1,0},{1,0},{-1,-1},{1,-1},{-1,1},{1,1}
        };

        private bool inRange(Point location) {
            return location.X >= 0 && location.Y >= 0 && location.X < owner.Columns && location.Y < owner.Rows;
        }

        private Hitbox? getTileHitbox(int value,Point location) {
            return Types?.GetHitbox(value,location);
        }

        private List<Hitbox> getSurroundingArea(int[,] layer,Hitbox hitbox) {

            Point center = (hitbox.Position + hitbox.Size * 0.5f).ToPoint();

            var hitboxes = new List<Hitbox>();

            for(int i = 0;i<MATRIX_SIZE;i++) {
                var offset = new Point(surroundingMatrix[i,0],surroundingMatrix[i,1]);
                var location = center + offset;

                if(!inRange(location)) {
                    continue;
                }

                int layerValue = layer[location.X,location.Y];
                Hitbox? tileHitbox = getTileHitbox(layerValue,location);

                if(!tileHitbox.HasValue) {
                    continue;
                }
                hitboxes.Add(tileHitbox.Value);
            }

            return hitboxes;
        }

        public List<Hitbox> Collides(Hitbox source) {
            int[,] layer = owner.GetLayer(CollisionLayer);

            List<Hitbox> surroundingHitboxes = getSurroundingArea(layer,source);
            var outputList = new List<Hitbox>();

            foreach(Hitbox hitbox in surroundingHitboxes) {
                if(source.Collides(hitbox)) outputList.Add(hitbox);
            }

            return outputList;
        }
    }
}
