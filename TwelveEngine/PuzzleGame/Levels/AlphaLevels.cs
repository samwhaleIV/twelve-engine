namespace TwelveEngine.PuzzleGame {
    public partial class ComponentFactory {

        public Level Test() {
            return new Level() {
                Map = "maps_alpha/test",
                Player = (7.5f, 4.5f),
                Components = () => {
                    OR(Switch(3,7,false),PlusButton(8,7)).Link(LaserGate(4,10,7,10));
                }
            };
        }

        public Level CounterTest() {
            return new Level() {
                Player = (7.5f, 2.5f),
                Map = "maps_alpha/counter-test",
                Components = () => {
                    var counter = Counter(5,8,true);
                    OR(MinusButton(6,4),PlusButton(4,4)).Link(counter);
                    AND(counter,Switch_Lock(11,5,true)).Link(LaserGate(16,8,18,8));
                    counter.Link(LaserGate(10,8,12,8));
                }
            };
        }

        public Level CounterTest2() {
            return new Level() {
                Player = (6, 5),
                Map = "maps_alpha/counter-test-2",
                Components = () => {
                    OR(MinusButton(2,3),Switch(2,5,false)).Link(SmallCounter(1,6,false)).Link(LaserGate(2,8,5,8));
                    OR(MinusButton(10,3),Switch(10,5,true)).Link(SmallCounter_Lock(11,6,false)).Link(LaserGate(7,8,10,8));
                }
            };  
        }


    }
}
