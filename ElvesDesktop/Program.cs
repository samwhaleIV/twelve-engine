using TwelveEngine.Shell.Config;
using TwelveEngine.Shell;
using System.IO;
using System;
using TwelveEngine;
using System.Collections.Generic;
using System.Text;

namespace ElvesDesktop {
    public static class Program {

        private static HashSet<string> flags;

        private static void Game_OnLoad(GameManager game) {

            game.Disposed += Game_Disposed;
            game.Exiting += Game_Exiting;

            Logger.AutoFlush = true;

            Elves.Program.StartGame(game,flags);
        }

        private static void Game_Exiting(object sender,EventArgs e) {
            Logger.AutoFlush = false;
        }

        private static void Game_Disposed(object sender,EventArgs e) {
            Logger.CleanUp();
        }

        private static void LogFlags() {
            var sb = new StringBuilder();
            sb.Append("[Elves Flags] { ");
            foreach(var flag in flags) {
                sb.Append(flag);
                sb.Append(", ");
            }
            sb.Remove(sb.Length-2,2);
            sb.Append(" }");
            Logger.WriteLine(sb);
        }

        public static void Main(string[] args) {
            flags = new HashSet<string>(args);
            LogFlags();

            ConfigLoader.LoadEngineConfig(new TwelveConfigSet() {
                HWFullScreenWidth = 1920,
                HWFullScreenHeight = 1080
            });
            string saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),Elves.Constants.SaveFolder);
            SaveDataManager.Initialize(Elves.Constants.SaveFile,saveDirectory,true);

            GC.Collect(GC.MaxGeneration,GCCollectionMode.Forced,true);

            using var game = new GameManager(
                fullscreen: flags.Contains(Elves.Constants.Flags.Fullscreen),
                hardwareModeSwitch: flags.Contains(Elves.Constants.Flags.HardwareFullscreen),
                verticalSync: !flags.Contains(Elves.Constants.Flags.NoVsync),
                drawDebug: flags.Contains(Elves.Constants.Flags.DrawDebug)
            );

            game.OnLoad += Game_OnLoad;
            game.Run();
        }
    }
}
