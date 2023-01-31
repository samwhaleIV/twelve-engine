using System;
using System.IO;
using TwelveEngine.Shell;
using TwelveEngine;

namespace ElvesWindows {
    public sealed class Program:EntryPoint {

        public static void Main(string[] args) {
            var program = new Program();
            program.StartEngine(args);
        }

        private void StartEngine(string[] args) {
            string saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),Elves.Constants.SaveFolder);
            EngineMain(saveDirectory,args);
        }

        protected override void OnGameLoad(GameManager game,string saveDirectory) {
            Elves.Program.Start(game,saveDirectory);
        }

        protected override void OnGameCrashed() {
            Elves.Program.OnGameCrashed();
        }
    }
}
