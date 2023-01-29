using System;
using System.Collections.Generic;

namespace Elves.Scenes.Battle {
    public static class BattleDictionary {
        private static readonly Dictionary<BattleID,Func<BattleScript>> scriptGenerators = new();

        public static void AddBattle<TBattleScript>(BattleID battleID) where TBattleScript: BattleScript, new() {
            if(scriptGenerators.ContainsKey(battleID)) {
                throw new ArgumentException($"Script for battle ID \"{battleID}\" already exists.");
            }
            scriptGenerators.Add(battleID,() => new TBattleScript());
        }

        public static BattleScript CreateScript(BattleID battleID) {
            if(!scriptGenerators.TryGetValue(battleID, out var scriptGenerator)) {
                throw new ArgumentException($"Battle script generator not found for battle ID \"{battleID}\".");
            }
            BattleScript script = scriptGenerator.Invoke();
            if(script is null) {
                throw new NullReferenceException($"Script generator for battle ID \"{battleID}\" has returned null.");
            }
            return script;
        }
    }
}
