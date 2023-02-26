using TwelveEngine.Shell;
using TwelveEngine.Font;
using TwelveEngine.Input.Binding;
using TwelveEngine.Audio;

namespace TwelveEngine {

    public abstract class EntryPoint {

        private GameStateManager game = null;

        protected abstract void OnGameLoad(GameStateManager game,string saveDirectory);
        protected abstract void OnGameCrashed();

        private void GameLoaded() {
            Fonts.Load(game);
            OnGameLoad(game,SaveDirectory);
        }

        private static void LogFatalException(Exception exception) {
            Logger.WriteLine($"Fatal game error: {exception}");
        }

        private void HandleSyncContextException(Exception exception) {
            LogFatalException(exception);
            OnGameCrashed();
            if(Flags.Get(Constants.Flags.NoFailSafe)) {
                game.ReroutedException = exception;
                return;
            }
            game.Exit();
        }

        private void HandleGameLoopException(Exception exception) {
            LogFatalException(exception);
            OnGameCrashed();
        }

        private void GameDisposed(object sender,EventArgs args) {
            Logger.CleanUp();
            AudioSystem.Unload();
        }

        private void UnhandledException(object sender,UnhandledExceptionEventArgs exceptionEventArgs) {
            LogFatalException(new Exception($"Fatal, unhandled exception. This one is really bad. Time of death, {DateTime.UtcNow}."));
        }

        private void RunGameWithExceptionHandling() {
            try {
                game.Run();
            } catch(Exception exception) {
                HandleGameLoopException(exception);
            }
        }

        private static GameStateManager CreateGameStateManager() => new(
            fullscreen: Flags.Get(Constants.Flags.Fullscreen),
            hardwareModeSwitch: Flags.Get(Constants.Flags.HardwareFullscreen),
            verticalSync: !Flags.Get(Constants.Flags.NoVsync),
            drawDebug: Flags.Get(Constants.Flags.DrawDebug)
        ) { SyncContext = new() };

        private void InitializeGameManager() {
            game = CreateGameStateManager();
            game.SyncContext.OnTaskException += HandleSyncContextException;

            game.Disposed += GameDisposed;
            game.OnLoad += GameLoaded;

            if(Flags.Get(Constants.Flags.NoFailSafe)) {
                game.Run();
            } else {
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;
                RunGameWithExceptionHandling();
            }
            game.SyncContext.Clear();

            game.Dispose();
            game = null;
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

            Flags.Load(args);

            SaveDirectory = saveDirectory;

            string logFile = GetSaveDirectoryPath(Constants.LogFile);
            string configFile = GetSaveDirectoryPath(Constants.ConfigFile);

            KeyBinds.Path = GetSaveDirectoryPath(Constants.KeyBindsFile);

            Logger.Initialize(logFile);
            Flags.WriteToLog();

            ValidateSaveDirectory(saveDirectory);

            Config.TryLoad(configFile);
            Config.WriteToLog();
            KeyBinds.TryLoad();

            AudioSystem.Load();

            GC.Collect(GC.MaxGeneration,GCCollectionMode.Forced,true);
            InitializeGameManager();
        }
    }
}
