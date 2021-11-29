using Microsoft.Xna.Framework;
using TwelveEngine.Game2D;

namespace TwelveEngine.PuzzleGame.Components {
    public class Switch:WorldInterface, IInteract {
        private readonly Point location;
        private readonly bool facingLeft;

        public Switch(

            int[,] objectLayer,
            int[,] collisionLayer,
            Point location,
            bool facingLeft

        ) : base(objectLayer,collisionLayer) {
            this.location = location;
            this.facingLeft = facingLeft;
        }

        public void Interact(Entity entity) {
            SignalState = SignalState.NOT();
            SendSignal();
        }

        public Hitbox GetHitbox() {
            int x = location.X, y = location.Y;
            return CollisionTypes.getHitbox(CollisionLayer[x,y],x,y).Value;
        }

        public override void OnChange(SignalState state) {
            int newTile;
            if(state.Value()) {
                if(facingLeft) {
                    newTile = Tiles.SwitchLeftOn;
                } else {
                    newTile = Tiles.SwitchRightOn;
                }
            } else {
                if(facingLeft) {
                    newTile = Tiles.SwitchLeftOff;
                } else {
                    newTile = Tiles.SwitchRightOff;
                }
            }
            ObjectLayer[location.X,location.Y] = newTile;
        }
    }
}
