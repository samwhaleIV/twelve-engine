using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Shell;
using Elves.Battle;

namespace Elves {
    public static class Program {
        public static void Main(GameManager game) {
            game.SetState(new BattleScene("Backgrounds/checkerboard"));
        }
    }
}
