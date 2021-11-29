using Microsoft.Xna.Framework;
using TwelveEngine.PuzzleGame.Components;

namespace TwelveEngine.PuzzleGame {
    public sealed class ComponentFactory {
        private readonly int[,] objectLayer;
        private readonly int[,] collisionLayer;

        public ComponentFactory(int[,] objectLayer,int[,] collisionLayer) {
            this.objectLayer = objectLayer;
            this.collisionLayer = collisionLayer;
        }

        private PulseButton getPulseButton(int x,int y,bool positive) {
            var location = new Point(x,y);
            var component = new PulseButton(objectLayer,collisionLayer,location,positive);
            return component;
        }

        public PulseButton GetMinusSwitch(int x,int y) {
            return getPulseButton(x,y,false);
        }
        public PulseButton GetPlusButton(int x,int y) {
            return getPulseButton(x,y,true);
        }

        public Switch GetSwitch(int x,int y,bool facingLeft) {
            var location = new Point(x,y);
            var component = new Switch(objectLayer,collisionLayer,location,facingLeft);
            return component;
        }

        public LaserGate GetLaserGate(int x1,int y1,int x2,int y2) {
            bool horizontal = (x1 != x2) && y1 == y2;
            var location1 = new Point(x1,y1);
            var location2 = new Point(x2,y2);
            var component = new LaserGate(objectLayer,collisionLayer,location1,location2,horizontal);
            return component;
        }

    }
}
