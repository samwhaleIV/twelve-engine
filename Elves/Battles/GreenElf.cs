using System;
using System.Threading.Tasks;
using Elves.Battle.Scripting;
using TwelveEngine;

namespace Elves.Battles {
    public sealed class GreenElf:BattleScript {

        public override async Task<BattleResult> Main() {
            await Continue();
            return BattleResult.Stalemate;
        }
    }
}
