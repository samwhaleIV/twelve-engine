using System;
using Microsoft.Xna.Framework;
using Porthole.Collision;
using TwelveEngine.Game2D.Entity;

namespace Porthole.PuzzleGame.Components {
    public class PulseButton:WorldComponent,IInteract {
        private readonly TimeSpan timeout = TimeSpan.FromMilliseconds(500);

        private readonly Point location;
        private readonly bool positive;

        public PulseButton(

            PuzzleGrid grid,
            Point location, bool positive

        ) : base(grid) {
            this.location = location;
            this.positive = positive;
        }

        public Hitbox GetHitbox() {
            int x = location.X, y = location.Y;
            return Grid.CollisionTypes.GetHitbox(CollisionLayer[x,y],location).Value;
        }

        private bool updating = false;

        private void endPulse() {
            SendSignal();
            updating = false;
        }

        public void Interact(Entity2D entity) {
            if(updating) {
                return;
            }
            updating = true;
            SignalState = positive ? SignalState.Positive : SignalState.Negative;
            SendSignal();
            SignalState = SignalState.Neutral;
            Grid.SetTimeout(endPulse,timeout);
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
