using System;
using System.IO;
using TwelveEngine.Shell;
using TwelveEngine;
using TwelveEngine.Audio;
using Microsoft.Xna.Framework;

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

        public static bool HasAudioBank { get; private set; }
        public static BankWrapper AudioBank { get; private set; }

        protected override void OnGameLoad(GameStateManager game,string saveDirectory) {
            game.Window.Title = "Where's John?";

            AudioSystem.TryLoadBank(Config.GetString(Config.Keys.FMODStrings),out _);
            AudioSystem.TryLoadBank(Config.GetString(Config.Keys.FMODMaster),out _);

            HasAudioBank = AudioSystem.TryLoadBank(Config.GetString(Config.Keys.FMODGame),out var audioBank);
            AudioBank = HasAudioBank ? audioBank : default;

            AudioSystem.BindVCAs();

            game.SetState(new CollectionGame());
        }

        protected override void OnGameCrashed() {
            return;
        }
    }
}
