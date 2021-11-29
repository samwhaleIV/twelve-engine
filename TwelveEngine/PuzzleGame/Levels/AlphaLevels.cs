using TwelveEngine.PuzzleGame.Components;

namespace TwelveEngine.PuzzleGame.Levels {
    public static class AlphaLevels {
        public static Level Level1 = new Level() {
            MapName = "level1",
            PlayerX = 5.5f,
            PlayerY = 3.5f,
            ComponentGenerator = factory => {
                var button1 = factory.GetSwitch(2,4,false);
                var button2 = factory.GetSwitch(9,10,true);

                var gate1 = factory.GetLaserGate(4,6,8,6);
                var gate2 = factory.GetLaserGate(11,5,11,9);

                new OR().SetInputs(button1.Link(new NOT()),button2).Link(gate1);

                button2.Link(new NOT()).Link(gate2);

                return new WorldInterface[] { button1,button2 };
            }
        };
        public static Level Level2 = new Level() {
            MapName = "level2",
            PlayerX = 6.5f,
            PlayerY = 4.5f,
            ComponentGenerator = factory => {
                var button1 = factory.GetSwitch(3,11,false);
                var button2 = factory.GetSwitch(13,12,true);
                var button3 = factory.GetSwitch(7,18,true);

                var gate1 = factory.GetLaserGate(5,6,8,6);
                var gate2 = factory.GetLaserGate(5,8,8,8);

                var gate3 = factory.GetLaserGate(8,10,13,10);
                var gate4 = factory.GetLaserGate(10,8,10,13);

                var gate5 = factory.GetLaserGate(4,14,8,14);
                var gate6 = factory.GetLaserGate(4,16,8,16);

                button1.Link(gate2);
                button1.Link(new NOT()).Link(gate5);
                gate5.Link(gate4);

                button2.Link(gate1);
                button2.Link(new NOT()).Link(gate6);

                button3.Link(new NOT()).Link(gate3);

                return new WorldInterface[] { button1,button2,button3 };
            }
        };
    }
}
