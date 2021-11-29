﻿using TwelveEngine.PuzzleGame.Components;

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

                return new WorldInterface[] { button1,button2,gate1,gate2 };
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
            };
        }
    }
}
