using System;
using System.IO;
using TwelveEngine.Shell;
using TwelveEngine;

namespace John {
    public sealed class Program:EntryPoint {

        [STAThread]
        public static void Main(string[] args) {
            var program = new Program();
            program.StartEngine(args);
        }

        private void StartEngine(string[] args) {
            string saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"JohnCollectionGame");
            Config.SetBool(Config.Keys.LimitFrameDelta,false);
            EngineMain(saveDirectory,args);
        }

        protected override void OnGameLoad(GameStateManager game,string saveDirectory) {
            game.SetState(new CollectionGame());
        }

        protected override void OnGameCrashed() {
        }
    }
}
