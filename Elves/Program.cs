using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Shell;

namespace Elves {
    public static class Program {
        public static void Main(GameManager game) {
            game.SetState<Elves.States.BattleSequencer>();


        }
    }
}
