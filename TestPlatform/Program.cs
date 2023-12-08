using System;
using System.IO;
using TwelveEngine.Shell;
using TwelveEngine;

namespace TestPlatform {
    public sealed class Program:EntryPoint {

        [STAThread]
        public static void Main(string[] args) {
            var program = new Program();
            program.StartEngine(args);
        }

        private static GameState GetStartState() {
            return new TileStressTest();
        }

        private void StartEngine(string[] args) {
            string saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"TwelveEngineTest");
            EngineMain(saveDirectory,args);
        }

        protected override void OnGameLoad(GameStateManager game,string saveDirectory) {
            //TODO: load save data
            //TODO: load audio banks
            game.SetState(GetStartState());
        }

        protected override void OnGameCrashed() {
            //TODO: Save data
        }
    }
}
