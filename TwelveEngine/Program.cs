using System;
using TwelveEngine.UI;
using TwelveEngine.UI.Elements;
using Microsoft.Xna.Framework;
using TwelveEngine.PuzzleGame;
using TwelveEngine.Serial;
using TwelveEngine.Game2D;

namespace TwelveEngine {
    public static partial class Program {

        private static bool loadedPuzzleGameData = false;

        private static void tryLoadPuzzleGameData() {
            if(loadedPuzzleGameData) {
                return;
            }
            MapDatabase.LoadMaps();
            EntityFactory.InstallDefault();
            loadedPuzzleGameData = true;
        }

        public static GameState GetPuzzleGameTest() {
            tryLoadPuzzleGameData();
            return PuzzleFactory.GetLevel("CounterTest2");
        }

        public static GameState GetUITestState() {
            return UIGameState.Create(UI => {

                var width = 400;
                var panelHeight = 200;

                var panels = new Element[100];
                for(int i = 0;i < panels.Length;i++) {
                    Color color;
                    switch(i % 3) {
                        default: case 0: color = Color.Red; break;
                        case 1: color = Color.Green; break;
                        case 2: color = Color.Orange; break;
                    }
                    panels[i] = new Panel(color) {
                        Area = new Rectangle(0,i*panelHeight,width,panelHeight)
                    };
                }

                var scrollBox = new ScrollBox() {
                    Sizing = Sizing.PercentY,
                    Width = width,
                    Height = 100,
                    Positioning = Positioning.CenterParentX,
                    Anchor = Anchor.TopRight
                };

                foreach(var panel in panels) {
                    scrollBox.Target.AddChild(panel);
                }
               
                UI.AddChild(scrollBox);
            });
        }

        public static GameState GetStartState() {
            return GetUITestState();
        }

        public static void StartGame(GameManager game) {
            game.SetState(GetStartState());
        }
    }
}
