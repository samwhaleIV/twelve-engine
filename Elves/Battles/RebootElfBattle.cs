using System.Threading.Tasks;
using Elves.Battle;

namespace Elves.Battles {
    public sealed class RebootElfBattle:BattleScript {
        public override async Task<BattleResult> Main() {
            while(true) {
                await Threader.Speech(new ("Ayy","Lmao","Ligma","Balls"));
                await Continue();
            }
        }
        public override Task Exit(BattleResult battleResult) {
            return base.Exit(battleResult);
        }
    }
}
