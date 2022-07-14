using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Shell;
using EntropyGame.States;

namespace EntropyGame {
    internal static class Program {
        public static GameState GetStartState() {
            GameState startState = null;

            //startState = new MainMenu();
            startState = new TestWorld();

            return startState;
        }
    }
}
