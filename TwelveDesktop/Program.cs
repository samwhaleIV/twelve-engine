using System;
using Microsoft.Xna.Framework;
using TwelveEngine.Shell;
using TwelveEngine.Shell.Config;
using Elves;
using Elves.ElfScript;
using Elves.BattleSequencer;
using System.IO;
using System.Linq;

namespace TwelveDesktop {

    internal static class Program {
        [STAThread]
        internal static void Main() {

            var functions = Tokenizer.GetFunctions(File.ReadAllLines(@"C:\Users\pinks\Desktop\elfscript.txt"));
            var script = Script.Compile(functions);

            var sequencer = new ConsoleBattleInterface();
            script.Execute("Main",sequencer,null);
            return;

            ConfigLoader.LoadEngineConfig(new TwelveConfigSet() {
                CPUTextures = new string[] { }
            });

            using var game = new GameManager();
            game.OnLoad += Game_OnLoad;
            game.Run(GameRunBehavior.Synchronous);
        }

        private static void Game_OnLoad(GameManager game) {
            Elves.Program.Main(game);
        }
    }
}
