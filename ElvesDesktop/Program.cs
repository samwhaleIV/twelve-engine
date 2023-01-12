using System;
using System.IO;
using TwelveEngine.Shell;
using TwelveEngine;

namespace ElvesDesktop {
    public sealed class Program:EntryPoint {

        public static void Main(string[] args) {
            var program = new Program();
            program.StartEngine(args);
        }

        private void StartEngine(string[] args) {
            string saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),Elves.Constants.SaveFolder);

            var data = new EntryPointData() {
                Args = args,
                SaveDirectory = saveDirectory,
                SaveFile = Elves.Constants.SaveFile
            };

            EngineMain(data);
        }

        protected override void OnGameLoad(GameManager game) {
            Elves.Program.Main(game,SaveData);
        }
    }
}
