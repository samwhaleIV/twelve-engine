using TwelveEngine.Shell.Config;
using TwelveEngine.Shell;

ConfigLoader.LoadEngineConfig(new TwelveConfigSet());
using var game = new GameManager();
game.OnLoad += Game_OnLoad;

void Game_OnLoad(GameManager game) {
    Elves.Program.Main(game);
}

game.Run();
