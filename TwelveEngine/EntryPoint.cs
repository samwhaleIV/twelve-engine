using TwelveEngine.Shell;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TwelveEngine.Input;

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

            OnGameLoad(game);
        }

        private void Game_Exiting(object sender,EventArgs e) {
            Logger.AutoFlush = false;
        }

        private void Game_Disposed(object sender,EventArgs e) {
            Logger.CleanUp();
        }

        private static void LogFlags(HashSet<string> flags) {
            var sb = new StringBuilder();
            sb.Append($"[Flags] {{ ");
            if(flags.Count <= 0) {
                sb.Append(Constants.Logging.None);
                sb.Append(" }");
                Logger.WriteLine(sb);
                return;
            }
            foreach(var flag in flags) {
                sb.Append(string.IsNullOrWhiteSpace(flag) ? Constants.Logging.Empty : flag);
                sb.Append(", ");
            }
            sb.Remove(sb.Length-2,2);
            sb.Append(" }");
            Logger.WriteLine(sb);
        }

        protected static void LoadFlags(string[] args) {
            IEnumerable<string> flagList;
            var configFlags = Config.GetStringArray(Config.Keys.Flags);
            if(configFlags is null) {
                flagList = args;
            } else {
                flagList = configFlags;
            }
            var flagSet = new HashSet<string>();
            if(flagList is null) {
                Flags.SetFlags(flagSet);
                LogFlags(flagSet);
                return;
            }
            foreach(var flag in flagList) {
                if(string.IsNullOrWhiteSpace(flag)) {
                    continue;
                }
                flagSet.Add(flag);
            }
            Flags.SetFlags(flagSet);
            LogFlags(flagSet);
        }

        protected void InitializeGameManager() {
            using var game = new GameManager(
                fullscreen: Flags.Get(Constants.Flags.Fullscreen),
                hardwareModeSwitch: Flags.Get(Constants.Flags.HardwareFullscreen),
                verticalSync: !Flags.Get(Constants.Flags.NoVsync)
            ) {
                DrawDebug = Flags.Get(Constants.Flags.DrawDebug)
            };
            game.OnLoad += Game_OnLoad;
            game.Run();
        }

        protected static bool ValidateSaveDirectory(string directory) {
            if(Directory.Exists(directory)) {
                return true;
            }
            bool success = false;
            try {
                Directory.CreateDirectory(directory);
                success = true;
            } catch(Exception exception) {
                Logger.WriteLine($"Failure creating save directory \"{directory}\": {exception}");
            }
            if(success) {
                Logger.WriteLine($"Created save directory \"{directory}\".");
            }
            return success;
        }

        private readonly SaveData _saveData = new();
        public SaveData SaveData => _saveData;


        protected string SaveDataFile { get; private set; } = null;
        protected string KeyBindsFile { get; private set; } = null;

        public bool TryLoadKeyBinds() {
            return KeyBinds.TryLoad(KeyBindsFile);
        }

        public bool TryLoadSaveData() {
            return SaveData.TryLoad(SaveDataFile);
        }

        public bool TrySaveSaveData() {
            return SaveData.TrySave(SaveDataFile);
        }

        protected void EngineMain(EntryPointData data) {
            _ = "Hello, world!";

            string logFile = data.GetDirectoryPath(Constants.LogFile);
            string configFile = data.GetDirectoryPath(Constants.ConfigFile);

            SaveDataFile = data.GetDirectoryPath(data.SaveFile);
            KeyBindsFile = data.GetDirectoryPath(Constants.KeyBindsFile);

            Logger.Initialize(logFile);
            ValidateSaveDirectory(data.SaveDirectory);

            Config.TryLoad(configFile);
            Config.WriteToLog();

            LoadFlags(data.Args);
            TryLoadKeyBinds();

            TryLoadSaveData();
            TrySaveSaveData();

            GC.Collect(GC.MaxGeneration,GCCollectionMode.Forced,true);
            InitializeGameManager();
        }
    }
}
