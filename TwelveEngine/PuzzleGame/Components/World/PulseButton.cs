using Microsoft.Xna.Framework;
using TwelveEngine.Game2D;
using System.Threading.Tasks;

namespace TwelveEngine.PuzzleGame.Components {
    public class PulseButton:WorldComponent,IInteract {
        private const int PULSE_TIME = 500;

        private readonly Point location;
        private readonly bool positive;

        public PulseButton(

            Grid2D grid,
            Point location, bool positive

        ) : base(grid) {
            this.location = location;
            this.positive = positive;
        }

        public Hitbox GetHitbox() {
            int x = location.X, y = location.Y;
            return CollisionTypes.getHitbox(CollisionLayer[x,y],x,y).Value;
        }

        private bool updating = false;

        private async void endPulse() {
            await Task.Delay(PULSE_TIME);
            SendSignal();
            updating = false;
        }

        public void Interact(Entity entity) {
            if(updating) {
                return;
            }
            updating = true;
            SignalState = positive ? SignalState.Positive : SignalState.Negative;
            SendSignal();
            SignalState = SignalState.Neutral;
            endPulse();
        }

        protected override void OnChange() {
            int newTile;

            if(SignalState.Value()) {
                if(positive) {
                    newTile = Tiles.PulsePlusOn;
                } else {
                    newTile = Tiles.PulseMinusOn;
                }
            } else {
                if(positive) {
                    newTile = Tiles.PulsePlusOff;
                } else {
                    newTile = Tiles.PulseMinusOff;
                }
            }

            ObjectLayer[location.X,location.Y] = newTile;
        }
    }
}
