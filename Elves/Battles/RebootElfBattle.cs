using System;
using System.Threading.Tasks;
using Elves.Battle.Scripting;

namespace Elves.Battles {
    public sealed class RebootElfBattle:BattleScript {
        public override async Task<BattleResult> Main() {
            while(true) {
                await Button("Hello",async () => {
                    await Threader.Speech("One","Two","Three");
                },"World",async () => {
                    await Threader.Speech("One","Two","Three");
                });
            }
        }
    }
}
