using Microsoft.Xna.Framework;
using Porthole.PuzzleGame.Components;
using System.Collections.Generic;

namespace Porthole.PuzzleGame {
    public partial class ComponentFactory {

        public ComponentFactory(PuzzleGrid grid) {
            _grid = grid;
            _components = new Queue<Component>();
        }

        private readonly PuzzleGrid _grid;
        private readonly Queue<Component> _components;

        private T _export<T>(T component) where T : Component {
            _components.Enqueue(component);
            return component;
        }
        private T _gate<T>(Component input1,Component input2) where T : Gate, new() {
            var gate = new T();
            gate.SetInputs(input1,input2);
            return _export(gate);
        }

        public Component[] Export() => _components.ToArray();

        public AND AND(Component input1,Component input2) => _gate<AND>(input1,input2);
        public XOR XOR(Component input1,Component input2) => _gate<XOR>(input1,input2);
        public OR OR(Component input1,Component input2) => _gate<OR>(input1,input2);

        public NOT NOT() => _export(new NOT());

        private PulseButton _pulseButton(int x,int y,bool positive) {
            var location = new Point(x,y);
            var component = new PulseButton(_grid,location,positive);
            return _export(component);
        }

        public PulseButton MinusButton(int x,int y) {
            return _export(_pulseButton(x,y,false));
        }
        public PulseButton PlusButton(int x,int y) {
            return _export(_pulseButton(x,y,true));
        }

        public Switch Switch(int x,int y,bool facingLeft) {
            var location = new Point(x,y);
            var component = new Switch(_grid,location,facingLeft);
            return _export(component);
        }

        public LaserGate LaserGate(int x1,int y1,int x2,int y2) {
            bool horizontal = (x1 != x2) && y1 == y2;
            var location1 = new Point(x1,y1);
            var location2 = new Point(x2,y2);
            var component = new LaserGate(_grid,location1,location2,horizontal);
            return _export(component);
        }

        public Counter SmallCounter(int x,int y,bool horizontal) {
            return _export(new Counter(_grid,x,y,horizontal,true));
        }
        public Counter Counter(int x,int y,bool horizontal) {
            return _export(new Counter(_grid,x,y,horizontal,false));
        }

        public Counter SmallCounter_Lock(int x,int y,bool horizontal) {
            var counter = SmallCounter(x,y,horizontal);
            counter.StateLock = true;
            return counter;
        }
        public Counter Counter_Lock(int x,int y,bool horizontal) {
            var counter = Counter(x,y,horizontal);
            counter.StateLock = true;
            return counter;
        }
        public Switch Switch_Lock(int x,int y,bool facingLeft) {
            var toggleSwitch = Switch(x,y,facingLeft);
            toggleSwitch.StateLock = true;
            return toggleSwitch;
        }

    }
}
