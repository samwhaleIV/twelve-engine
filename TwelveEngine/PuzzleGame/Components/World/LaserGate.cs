using Microsoft.Xna.Framework;

namespace TwelveEngine.PuzzleGame.Components {
    public class LaserGate:WorldInterface {

        private readonly Point recepticle1;
        private readonly Point recepticle2;
        private readonly bool horizontal;

        public LaserGate(

            int[,] objectLayer, int[,] collisionLayer,
            Point recepticle1, Point recepticle2,bool horizontal

        ) : base(objectLayer,collisionLayer) {
            this.recepticle1 = recepticle1;
            this.recepticle2 = recepticle2;
            this.horizontal = horizontal;
        }

        private void updateRecepticles(int x1,int y1,int x2,int y2,bool active) {
            if(active) {
                if(horizontal) {
                    ObjectLayer[x1,y1] = Tiles.LaserLeftOn;
                    ObjectLayer[x2,y2] = Tiles.LaserRightOn;
                } else {
                    ObjectLayer[x1,y1] = Tiles.LaserUpOn;
                    ObjectLayer[x2,y2] = Tiles.LaserDownOn;
                }
            } else {
                if(horizontal) {
                    ObjectLayer[x1,y1] = Tiles.LaserLeftOff;
                    ObjectLayer[x2,y2] = Tiles.LaserRightOff;

                } else {
                    ObjectLayer[x1,y1] = Tiles.LaserUpOff;
                    ObjectLayer[x2,y2] = Tiles.LaserDownOff;
                }
            }
        }

        private void updateLasers(int x1,int y1,int x2,int y2,bool active) {
            if(horizontal) {
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
            } else {
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

        public override void OnChange(SignalState state) {
            int x1 = recepticle1.X, x2 = recepticle2.X,
                y1 = recepticle1.Y, y2 = recepticle2.Y;
            var active = state.Value();

            updateRecepticles(x1,y1,x2,y2,active);
            updateLasers(x1,y1,x2,y2,active);
        }
    }
}
