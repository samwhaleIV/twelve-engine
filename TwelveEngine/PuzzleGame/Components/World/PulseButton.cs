using Microsoft.Xna.Framework;
using TwelveEngine.Game2D;
using System.Threading.Tasks;

namespace TwelveEngine.PuzzleGame.Components {
    public class PulseButton:WorldInterface,IInteract {
        private const int PULSE_TIME = 500;

        private readonly Point location;
        private readonly bool positive;

        public PulseButton(

            int[,] objectLayer,
            int[,] collisionLayer,
            Point location, bool positive

        ) : base(objectLayer,collisionLayer) {
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
            SignalState = SignalState.Neutral;
            SendSignal();
            updating = false;
        }

        public void Interact(Entity entity) {
            updating = true;
            if(updating) {
                return;
            }
            SignalState = positive ? SignalState.Positive : SignalState.Negative;
            SendSignal();
            endPulse();
        }

        public override void OnChange(SignalState state) {
            int newTile;

            if(state.Value()) {
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
