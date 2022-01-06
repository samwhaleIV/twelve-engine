using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game2D.Collision {
    public sealed class CollisionInterface {

        private readonly Grid2D grid;
        public CollisionInterface(Grid2D grid) {
            this.grid = grid;
        }

        private CollisionTypes collisionTypes;
        internal CollisionTypes Types {
            get => collisionTypes;
            set => collisionTypes = value;
        }

        public Hitbox? GetHitbox(int ID,Vector2 location) {
            return collisionTypes?.GetHitbox(ID,location);
        }
        public Hitbox? GetHitbox(int ID,Point location) {
            return collisionTypes?.GetHitbox(ID,location);
        }

        private const int MATRIX_SIZE = 9;
        private int[,] surroundingMatrix = new int[MATRIX_SIZE,2] {
            {0,0},{0,-1},{0,1},{-1,0},{1,0},{-1,-1},{1,-1},{-1,1},{1,1}
        };

        private bool inRange(Point location) {
            return location.X >= 0 && location.Y >= 0 && location.X < grid.Columns && location.Y < grid.Rows;
        }

        private Hitbox? getTileHitbox(int value,Point location) {
            return collisionTypes?.GetHitbox(value,location);
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
            int[,] layer = grid.GetLayer(Constants.CollisionLayerIndex);

            List<Hitbox> surroundingHitboxes = getSurroundingArea(layer,source);
            var outputList = new List<Hitbox>();

            foreach(Hitbox hitbox in surroundingHitboxes) {
                if(source.Collides(hitbox)) {
                    outputList.Add(hitbox);
                }
            }

            return outputList;
        }
    }
}
