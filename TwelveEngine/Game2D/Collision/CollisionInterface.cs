using System;
using System.Collections.Generic;

namespace TwelveEngine.Game2D {
    public sealed class CollisionInterface {

        private const float WALK_BEHIND_HEIGHT = 4 / 16f;
        private const float WALK_BEHIND_Y = 10 / 16f;

        private readonly Grid2D grid;
        public CollisionInterface(Grid2D grid) {
            this.grid = grid;
        }

        private const int MATRIX_SIZE = 9;

        private int[,] surroundingMatrix = new int[MATRIX_SIZE,2] {
            {0,0},{0,-1},{0,1},{-1,0},{1,0},{-1,-1},{1,-1},{-1,1},{1,1}
        };

        private bool inRange(int x, int y) {
            return x >= 0 && y >= 0 && x < grid.Width && y < grid.Height;
        }

        private Hitbox? getTileHitbox(int value,int x,int y) {
            return CollisionTypes.getHitbox(value,x,y);
        }

        private List<Hitbox> getSurroundingArea(int[,] layer,Hitbox hitbox) {
            var centerX = (int)Math.Floor(hitbox.X + hitbox.Width / 2);
            var centerY = (int)Math.Floor(hitbox.Y + hitbox.Height / 2);

            var hitboxes = new List<Hitbox>();

            for(int i = 0;i<MATRIX_SIZE;i++) {
                int xOffset = surroundingMatrix[i,0];
                int yOffset = surroundingMatrix[i,1];

                int x = centerX + xOffset;
                int y = centerY + yOffset;

                if(!inRange(x,y)) {
                    continue;
                }

                int layerValue = layer[x,y];
                Hitbox? tileHitbox = getTileHitbox(layerValue,x,y);

                if(!tileHitbox.HasValue) {
                    continue;
                }
                hitboxes.Add(tileHitbox.Value);
            }

            return hitboxes;
        }

        private bool collides(Hitbox a,Hitbox b) {
            return a.X <= b.X + b.Width && a.X + a.Width >= b.X && a.Y <= b.Y + b.Height && a.Height + a.Y >= b.Y;
        }

        public List<Hitbox> Collides(Hitbox source) {
            int[,] layer = this.grid.GetLayer(Constants.CollisionLayerIndex);

            List<Hitbox> surroundingHitboxes = getSurroundingArea(layer,source);
            var outputList = new List<Hitbox>();

            foreach(Hitbox hitbox in surroundingHitboxes) {
                if(collides(source,hitbox)) {
                    outputList.Add(hitbox);
                }
            }

            return outputList;
        }
    }
}
