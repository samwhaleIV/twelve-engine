using Microsoft.Xna.Framework;
using TwelveEngine.Game2D;
using TwelveEngine.Porthole;

namespace TwelveEngine.Porthole.PuzzleGame.Components {
    public sealed class LaserGate:WorldComponent {

        private readonly int x1; private readonly int y1;
        private readonly int x2; private readonly int y2;

        private readonly bool horizontal;

        public LaserGate(

            Grid2D grid,
            Point recepticle1, Point recepticle2,bool horizontal

        ) : base(grid) {
            x1 = recepticle1.X; y1 = recepticle1.Y;
            x2 = recepticle2.X; y2 = recepticle2.Y;
            this.horizontal = horizontal;
        }

        protected override void OnChange() {
            var active = SignalState.Value();

            if(horizontal) {
                updateRecepticlesHorizontal(active);
                updateLasersHorizontal(active);
            } else {
                updateRecepticlesVertical(active);
                updateLasersVertical(active);
            }
        }

        private void updateRecepticlesHorizontal(bool active) {
            if(active) {
                ObjectLayer[x1,y1] = Tiles.LaserLeftOn;
                ObjectLayer[x2,y2] = Tiles.LaserRightOn;
            } else {
                ObjectLayer[x1,y1] = Tiles.LaserLeftOff;
                ObjectLayer[x2,y2] = Tiles.LaserRightOff;
            }
        }
        private void updateRecepticlesVertical(bool active) {
            if(active) {
                ObjectLayer[x1,y1] = Tiles.LaserUpOn;
                ObjectLayer[x2,y2] = Tiles.LaserDownOn;
            } else {
                ObjectLayer[x1,y1] = Tiles.LaserUpOff;
                ObjectLayer[x2,y2] = Tiles.LaserDownOff;
            }
        }
        private void updateLasersHorizontal(bool active) {
            for(int x = x1+1;x < x2;x++) {
                if(active) {
                    if(ObjectLayer[x,y1] == Tiles.VerticalLaser) {
                        ObjectLayer[x,y1] = Tiles.CrossLaser;
                    } else {
                        ObjectLayer[x,y1] = Tiles.HorizontalLaser;
                    }
                    CollisionLayer[x,y1] = Tiles.Collision.HorizontalLaser;
                } else {
                    if(ObjectLayer[x,y1] == Tiles.CrossLaser) {
                        ObjectLayer[x,y1] = Tiles.VerticalLaser;
                        CollisionLayer[x,y1] = Tiles.Collision.VerticalLaser;
                    } else {
                        ObjectLayer[x,y1] = 0;
                        CollisionLayer[x,y1] = 0;
                    }
                }
            }
        }
        private void updateLasersVertical(bool active) {
            for(int y = y1+1;y < y2;y++) {
                if(active) {
                    if(ObjectLayer[x1,y] == Tiles.HorizontalLaser) {
                        ObjectLayer[x1,y] = Tiles.CrossLaser;
                    } else {
                        ObjectLayer[x1,y] = Tiles.VerticalLaser;
                    }
                    CollisionLayer[x1,y] = Tiles.Collision.VerticalLaser;
                } else {
                    if(ObjectLayer[x1,y] == Tiles.CrossLaser) {
                        ObjectLayer[x1,y] = Tiles.HorizontalLaser;
                        CollisionLayer[x1,y] = Tiles.Collision.HorizontalLaser;
                    } else {
                        ObjectLayer[x1,y] = 0;
                        CollisionLayer[x1,y] = 0;
                    }
                }
            }
        }
    }
}
