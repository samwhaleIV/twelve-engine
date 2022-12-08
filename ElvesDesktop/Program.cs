using TwelveEngine.Shell.Config;
using TwelveEngine.Shell;
using System.IO;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using TwelveEngine;
using TwelveEngine.EntitySystem;

namespace ElvesDesktop {
    public static class Program {
        private static void Game_OnLoad(GameManager game) {

            string saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"ElvesGame");
            game.Disposed += Game_Disposed;
            game.Exiting += Game_Exiting;
            var program = new Elves.Program(game,saveDirectory,true);
        }

        private static void Game_Exiting(object sender,EventArgs e) {
            Logger.AutoFlush = false;
        }

        private static void Game_Disposed(object sender,EventArgs e) {
            Logger.CleanUp();
        }

        public static void Main(string[] args) {
            ConfigLoader.LoadEngineConfig(new TwelveConfigSet());
            using var game = new GameManager(fullscreen: false,verticalSync: true);
            game.OnLoad += Game_OnLoad;
            game.Run();
        }

    }
}