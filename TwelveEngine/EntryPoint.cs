using TwelveEngine.Shell;
using System;
using System.IO;
using TwelveEngine.Input;
using Microsoft.Xna.Framework.Input;
using System.Text;
using TwelveEngine.Font;
using Microsoft.Xna.Framework.Media;

namespace TwelveEngine {

    public struct EntryPointData {
        public string SaveDirectory { get; set; }
        public string SaveFile { get; set; }
        public string[] Args { get; set; }

        public readonly string GetDirectoryPath(string path) {
            return Path.Combine(SaveDirectory,path);
        }
    }

    public abstract class EntryPoint {

        protected abstract void OnGameLoad(GameManager game);

        private void Game_OnLoad(GameManager game) {

            game.Disposed += Game_Disposed;
            game.Exiting += Game_Exiting;

            Logger.AutoFlush = true;

            Fonts.Load(game);
            OnGameLoad(game);
        }

        private void Game_Exiting(object sender,EventArgs e) {
            Logger.AutoFlush = false;
        }

        private void Game_Disposed(object sender,EventArgs e) {
            Logger.CleanUp();
        }

        private void InitializeGameManager() {
            using var game = new GameManager(
                fullscreen: Flags.Get(Constants.Flags.Fullscreen),
                hardwareModeSwitch: Flags.Get(Constants.Flags.HardwareFullscreen),
                verticalSync: !Flags.Get(Constants.Flags.NoVsync)
            ) {
                DrawDebug = Flags.Get(Constants.Flags.DrawDebug)
            };
            game.OnLoad += Game_OnLoad;
            if(Flags.Get(Constants.Flags.NoFailSafe)) {
                game.Run();
            } else {
                try {
                    game.Run();
                } catch(Exception exception) {
                    SaveData.TrySave();
                    Logger.WriteLine($"An unexpected error has occurred: {exception}");
                }
            }
        }

        private static bool ValidateSaveDirectory(string directory) {
            if(Directory.Exists(directory)) {
                return true;
            }
            bool success = false;
            try {
                Directory.CreateDirectory(directory);
                success = true;
            } catch(Exception exception) {
                Logger.WriteLine($"Failure creating save directory \"{directory}\": {exception}",LoggerLabel.Save);
            }
            if(success) {
                Logger.WriteLine($"Created save directory \"{directory}\".",LoggerLabel.Save);
            }
            return success;
        }

        private readonly SaveData _saveData = new();
        protected SaveData SaveData => _saveData;

        protected void EngineMain(EntryPointData data) {
            _ = "Hello, world!";

            string logFile = data.GetDirectoryPath(Constants.LogFile);
            string configFile = data.GetDirectoryPath(Constants.ConfigFile);

            SaveData.Path = data.GetDirectoryPath(data.SaveFile);
            KeyBinds.Path = data.GetDirectoryPath(Constants.KeyBindsFile);

            Logger.Initialize(logFile);

            ValidateSaveDirectory(data.SaveDirectory);

            Config.TryLoad(configFile);
            Flags.Load(data.Args);

            Config.WriteToLog();
            KeyBinds.TryLoad();

            SaveData.TryLoad();
            SaveData.TrySave();

            GC.Collect(GC.MaxGeneration,GCCollectionMode.Forced,true);
            InitializeGameManager();
        }
    }
}
