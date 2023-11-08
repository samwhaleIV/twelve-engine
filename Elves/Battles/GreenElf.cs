using System;
using System.Threading.Tasks;
using Elves.Battle.Scripting;
using TwelveEngine;

namespace Elves.Battles {
    public sealed class GreenElf:BattleScript {

        public override async Task<BattleResult> Main() {
            await Tag($"You approach {Actor.Name}.",$"{Actor.Name} looks annoyed.");
            await Speech("Oh God, another person. What could you want?");
            return BattleResult.PlayerWon;
        }
    }
}
