using TwelveEngine.Shell;
using Elves.Battle;
using Elves.UI.Font;
using Elves.UI;
using System.Collections.Generic;

namespace Elves {
    public static class Program {

        public static GameState GetStartState() {
            return new BattleScene();
        }

        private static HashSet<string> _flags;

        public static bool HasFlag(string flag) {
            return _flags.Contains(flag);
        }

        private static GameManager _game;
        public static GameManager Game => _game;

        public static void StartGame(GameManager game,HashSet<string> flags) {
            _game = game;
            _flags = flags;

            UITextures.Load(game);
            Fonts.Load();

            game.SetState(GetStartState);
        }
    }
}
