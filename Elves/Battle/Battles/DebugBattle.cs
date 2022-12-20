using Elves.Battle.Sprite.Elves;
using System.Threading.Tasks;
using Elves.Battle.Sprite.Animation;

namespace Elves.Battle.Battles {
    public class DebugBattle:Script {
        public override async Task<BattleResult> Main() {
            CreatePlayer();
            CreateActor(new HarmlessElf());
            while(EverybodyIsAlive) {
                await Continue();
                Actor.Hurt(10);
                ActorSprite.SetAnimation(AnimationType.Hurt);
            }
            ActorSprite.SetAnimation(AnimationType.Dead);
            return WinCondition;
        }
    }
}
