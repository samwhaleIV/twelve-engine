using Microsoft.Xna.Framework;
using TwelveEngine.Game2D;

namespace TwelveEngine.PuzzleGame.Components {
    public sealed class Switch:WorldComponent, IInteract {
        private readonly Point location;
        private readonly bool facingLeft;

        public Switch(
            Grid2D grid,
            Point location,
            bool facingLeft

        ) : base(grid) {
            this.location = location;
            this.facingLeft = facingLeft;
        }

        public void Interact(Entity2D entity) {
            SignalState = SignalState.NOT();
            SendSignal();
        }

        public Hitbox GetHitbox() {
            int x = location.X, y = location.Y;
            return Grid.Collision.GetHitbox(CollisionLayer[x,y],x,y).Value;
        }

        protected override void OnChange() {
            int newTile;
            if(SignalState.Value()) {
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

        public override void Import(SerialFrame frame) {
            base.Import(frame);
        }
    }
}
