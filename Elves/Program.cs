using TwelveEngine.Shell;
using Elves.Battle;
using Elves.UI.Font;
using Elves.UI;
using System.Collections.Generic;
using Elves.Battle.Battles;
using Microsoft.Xna.Framework;

namespace Elves {
    public static class Program {

        public static GameState GetStartState() {
            return new DebugBattle();
        }

        private static HashSet<string> _flags;

        public static bool HasFlag(string flag) {
            return _flags.Contains(flag);
        }

        private static GameManager _game;
        public static GameManager Game => _game;

        private static void SetCustomCursor() {
            _game.CustomCursorTexture = UITextures.Panel;
            _game.CursorSources.Add(CursorState.Default,new Rectangle(64,0,8,8));
            _game.CursorSources.Add(CursorState.Interact,new Rectangle(64,8,8,8));
            _game.CursorSources.Add(CursorState.Pressed,new Rectangle(72,8,8,8));
            _game.CursorScale = 8;
        }

        public static void StartGame(GameManager game,HashSet<string> flags) {
            _game = game;
            _flags = flags;

            UITextures.Load(game);
            Fonts.Load();

            if(!flags.Contains(Constants.Flags.OSCursor)) {
                SetCustomCursor();
            }

            game.SetState(GetStartState);
        }
    }
}
