using Microsoft.Xna.Framework;
using Porthole.PuzzleGame;

namespace Porthole.Collision {
    public sealed class TileCollision {

        private readonly PuzzleGrid owner;
        public TileCollision(PuzzleGrid owner) => this.owner = owner;

        public TileCollisionTypes Types => owner.CollisionTypes;
        public int Layer { get; set; } = TwelveEngine.Constants.CollisionLayerIndex;

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

        private bool InRange(Point location) => owner.UnitArea.Contains(location);

        private Hitbox? GetTileHitbox(int value,Point location) {
            return Types?.GetHitbox(value,location);
        }

        private readonly Hitbox[] collisionBuffer = new Hitbox[MATRIX_SIZE];
        private int collisionBufferLength = 0;

        private void PopulateCollisionBuffer(int[,] layer,Hitbox hitbox) {

            Point center = (hitbox.Position + hitbox.Size * 0.5f).ToPoint();

            collisionBufferLength = 0;
            for(int i = 0;i<MATRIX_SIZE;i++) {
                var offset = new Point(surroundingMatrix[i,0],surroundingMatrix[i,1]);
                var location = center + offset;

                if(!InRange(location)) {
                    continue;
                }

                int layerValue = layer[location.X,location.Y];
                Hitbox? tileHitbox = GetTileHitbox(layerValue,location);

                if(!tileHitbox.HasValue) {
                    continue;
                }
                int index = collisionBufferLength;
                collisionBufferLength += 1;
                collisionBuffer[index] = tileHitbox.Value;
            }

        }

        public Hitbox? Collides(Hitbox source) {
            PopulateCollisionBuffer(owner.GetLayer(Layer),source);

            for(int i = 0;i<collisionBufferLength;i++) {
                var hitbox = collisionBuffer[i];
                if(source.Collides(hitbox)) {
                    return hitbox;
                }
            }
            return null;
        }
    }
}
