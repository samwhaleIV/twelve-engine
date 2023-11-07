using System.Threading.Tasks;
using Elves.Battle.Scripting;

namespace Elves.Battles {
    public sealed class AnimationTest:BattleScript {

        public override async Task<BattleResult> Main() {
            while(true) {
                switch(await Button("Hurt","Heal","Kill","Revive")) {
                    case 0:
                        Actor.HurtPercent(0.25f);
                        break;
                    case 1:
                        Actor.HealPercent(0.25f);
                        break;
                    case 2:
                        Actor.Kill();
                        break;
                    case 3:
                        Actor.Health = Actor.MaxHealth;
                        break;
                }
            }
        }
    }
}
