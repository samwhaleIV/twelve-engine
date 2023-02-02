using System.Threading.Tasks;
using Elves.Battle;
using Elves.ElfData;

namespace Elves.Battles {
    public sealed class DebugBattle:BattleScript {

        public override void Setup() {
            CreatePlayer();
            CreateActor(ElfManifest.Get(ElfID.HarmlessElf));
        }

        public override async Task<BattleResult> Main() {     
            await Tag("Press any button to win.");
            int result = await GetButton("No","No","No","Yes");
            if(result != Button4) {
                return BattleResult.PlayerLost;
            } else {
                return BattleResult.PlayerWon;
            }
        }

        public override async Task Exit(BattleResult battleResult) {
            if(battleResult == BattleResult.PlayerWon) {
                SetTag("Good job.");
                await Continue();
                Actor.Kill();
            } else {
                SetTag("I lied.");
                await Continue();
                Player.Kill();
            }
            await base.Exit(battleResult);
        }
    }
}
