using System.Threading.Tasks;
using Elves.Battle;
using Elves.Battle.Scripting;

namespace Elves.Battles {
    public sealed class RebootElfBattle:BattleScript {
        public override async Task<BattleResult> Main() {
            while(true) {
                await Threader.Tag("Ayy","Lmao","Ligma","Balls");
                await Continue();
            }
        }
        public override Task Exit(BattleResult battleResult) {
            return base.Exit(battleResult);
        }
    }
}
