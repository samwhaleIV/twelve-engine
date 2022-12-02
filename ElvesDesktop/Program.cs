using TwelveEngine.Shell.Config;
using TwelveEngine.Shell;
using System.IO;
using System;
using System.Runtime.InteropServices;

#if DEBUG
[DllImport("kernel32.dll",SetLastError = true)]
[return: MarshalAs(UnmanagedType.Bool)]
static extern bool AllocConsole();
AllocConsole();
#endif

ConfigLoader.LoadEngineConfig(new TwelveConfigSet());
using var game = new GameManager();
game.OnLoad += Game_OnLoad;


void Game_OnLoad(GameManager game) {
    string saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"ElvesGame");
    var program = new Elves.Program(game,saveDirectory,true);
}

game.Run();
