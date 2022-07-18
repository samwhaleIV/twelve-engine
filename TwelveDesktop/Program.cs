using System;
using Microsoft.Xna.Framework;
using TwelveEngine.Shell;
using TwelveEngine.Shell.Config;

namespace TwelveDesktop {

    internal static class Program {
        [STAThread]
        internal static void Main() {

            ConfigLoader.LoadEngineConfig(new TwelveConfigSet() {
                CPUTextures = new string[] {  }
            });

            using var game = new GameManager();
            game.OnLoad += Game_OnLoad;
            game.Run(GameRunBehavior.Synchronous);
        }

        private static void Game_OnLoad(GameManager game) {
            //game.SetState(Porthole.Program.GetPuzzleGameTest());
            //game.SetState<ElfGame.FacialAnimationViewer>();
            //game.SetState(new CRTTest());
            //game.SetState<JewelEditor.Editor>();
            //game.SetState(ModelViewer.CreateTextureTest("Test/cat-test-picture"));
            //game.SetState(new TileGenViewer());
            game.SetState(EntropyGame.Program.GetStartState());
        }
    }
}
