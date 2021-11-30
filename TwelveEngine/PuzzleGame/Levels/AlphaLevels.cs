using TwelveEngine.PuzzleGame.Components;

namespace TwelveEngine.PuzzleGame.Levels {
    public static class AlphaLevels {
        public static Level Test = new Level() {
            MapName = "maps_alpha/test",
            Player = (7.5f,4.5f),
            GenerateComponents = factory => {
                new OR().SetInputs(
                    factory.GetSwitch(3,7,false),
                    factory.GetPlusButton(8,7)
                ).Link(
                    factory.GetLaserGate(4,10,7,10)
                );
            }
        };
    }
}
