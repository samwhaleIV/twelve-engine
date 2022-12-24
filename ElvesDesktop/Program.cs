using TwelveEngine.Shell.Config;
using TwelveEngine.Shell;
using System.IO;
using System;
using TwelveEngine;
using System.Collections.Generic;

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

        public static void Main(string[] args) {
            flags = new HashSet<string>(args);
            ConfigLoader.LoadEngineConfig(new TwelveConfigSet());
            string saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),Elves.Constants.SaveFolder);
            SaveDataManager.Initialize(Elves.Constants.SaveFile,saveDirectory,true);

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
