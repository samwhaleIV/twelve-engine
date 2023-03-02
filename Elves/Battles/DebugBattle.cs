using System.Threading.Tasks;
using Elves.Battle;
using Elves.ElfData;

namespace Elves.Battles {
    public sealed class DebugBattle:BattleScript {

        public override async Task<BattleResult> Main() {     
            await Tag("Press any button to win.");
            int result = await GetButton("No","No","No","Yes");
            if(result != B4) {
                return BattleResult.PlayerLost;
            } else {
                return BattleResult.PlayerWon;
            }
        }

        public override async Task Exit(BattleResult battleResult) {
            if(battleResult == BattleResult.PlayerWon) {
                await Tag("Good job.");
                Actor.Kill();
            } else {
                Tag("I lied.");
                await Continue();
                Player.Kill();
            }
            await base.Exit(battleResult);
        }
    }
}
