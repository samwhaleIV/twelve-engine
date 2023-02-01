using TwelveEngine.Shell;
using TwelveEngine.Font;
using TwelveEngine.Input.Binding;

namespace TwelveEngine {

    public abstract class EntryPoint {

        protected abstract void OnGameLoad(GameStateManager game,string saveDirectory);
        protected abstract void OnGameCrashed();

        private void Game_OnLoad(GameStateManager game) {
            game.Disposed += Game_Disposed;
            Fonts.Load(game);
            OnGameLoad(game,SaveDirectory);
        }

        private void Game_Disposed(object sender,EventArgs args) {
            Logger.CleanUp();
        }

        private void RunGameWithExceptionHandling(GameStateManager game) {
            try {
                game.Run();
            } catch(Exception exception) {
                Logger.WriteLine($"An unexpected error has occurred: {exception}");
                OnGameCrashed();
            }
        }

        private void InitializeGameManager() {
            using GameStateManager game = new(
                fullscreen: Flags.Get(Constants.Flags.Fullscreen),
                hardwareModeSwitch: Flags.Get(Constants.Flags.HardwareFullscreen),
                verticalSync: !Flags.Get(Constants.Flags.NoVsync),
                drawDebug: Flags.Get(Constants.Flags.DrawDebug)
            );
            game.OnLoad += Game_OnLoad;
            if(Flags.Get(Constants.Flags.NoFailSafe)) {
                game.Run();
            } else {
                RunGameWithExceptionHandling(game);
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

        public string GetSaveDirectoryPath(string path) {
            return Path.Combine(SaveDirectory,path);
        }

        public string SaveDirectory { get; private set; }

        protected void EngineMain(string saveDirectory,string[] args) {
            _ = "Hello, world!";

            ProxyTime.Start();

            SaveDirectory = saveDirectory;

            string logFile = GetSaveDirectoryPath(Constants.LogFile);
            string configFile = GetSaveDirectoryPath(Constants.ConfigFile);

            KeyBinds.Path = GetSaveDirectoryPath(Constants.KeyBindsFile);

            Logger.Initialize(logFile);

            ValidateSaveDirectory(saveDirectory);

            Config.TryLoad(configFile);
            Flags.Load(args);

            Config.WriteToLog();
            KeyBinds.TryLoad();

            GC.Collect(GC.MaxGeneration,GCCollectionMode.Forced,true);
            InitializeGameManager();
        }
    }
}
