using Microsoft.Xna.Framework;
using TwelveEngine.PuzzleGame.Components;
using TwelveEngine.Game2D;
using System.Collections.Generic;

namespace TwelveEngine.PuzzleGame {
    public sealed class ComponentFactory {

        private readonly Grid2D grid;
        private readonly Queue<WorldComponent> components;

        public ComponentFactory(Grid2D grid) {
            this.grid = grid;
            components = new Queue<WorldComponent>();
        }
        private T Export<T>(T component) where T:WorldComponent {
            components.Enqueue(component);
            return component;
        }

        public WorldComponent[] Export() => components.ToArray();

        private PulseButton getPulseButton(int x,int y,bool positive) {
            var location = new Point(x,y);
            var component = new PulseButton(grid,location,positive);
            return Export(component);
        }

        public PulseButton GetMinusSwitch(int x,int y) {
            return Export(getPulseButton(x,y,false));
        }
        public PulseButton GetPlusButton(int x,int y) {
            return Export(getPulseButton(x,y,true));
        }

        public Switch GetSwitch(int x,int y,bool facingLeft) {
            var location = new Point(x,y);
            var component = new Switch(grid,location,facingLeft);
            return Export(component);
        }

        public LaserGate GetLaserGate(int x1,int y1,int x2,int y2) {
            bool horizontal = (x1 != x2) && y1 == y2;
            var location1 = new Point(x1,y1);
            var location2 = new Point(x2,y2);
            var component = new LaserGate(grid,location1,location2,horizontal);
            return Export(component);
        }

    }
}
