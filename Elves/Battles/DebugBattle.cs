using System.Threading;
using System.Threading.Tasks;
using Elves.Battle;
using Elves.ElfData;

namespace Elves.Battles {
    public sealed class DebugBattle:BattleScript {
        public override async Task<BattleResult> Main() {
            CreatePlayer();
            CreateActor(ElfManifest.Get(ElfID.HarmlessElf));
            while(EverybodyIsAlive) {
                await Continue();
                await GetButton("EAT","MY","ASS","PLEASE");
                await Tag("THIS IS NOT VERY EFFECTIVE");
                await Speech("The text rendering in this speech box is still a little fucked up... But you get the idea.".ToUpperInvariant());
                await Tag("THEYRE LIKE ANIMALS","AND I SLAUGHTERED THEM LIKE ANIMALS");
                await GetButton("ONE","TWO");
                await Tag("LOOK SEE","WE SUPPORT DYNAMIC BUTTON LAYOUT");
                await GetButton("ONE","TWO","THREE");
                await GetButton("ONE","TWO");
                await GetButton("ONE");
                await GetButton("ONE","TWO","THREE","FOUR");
                await GetButton("SUCK");
                await GetButton("MY");
                await GetButton("DICK");
                await Speech("also I still need to add periods and commans and apostrophres to the font lmao".ToUpperInvariant());
            }
            return WinCondition;
            //CreatePlayer();
            //CreateActor(new HarmlessElf());
            //while(EverybodyIsAlive) {
            //    await Continue();
            //    Actor.Hurt(10);
            //    ActorSprite.SetAnimation(AnimationType.Hurt);
            //}
            //ActorSprite.SetAnimation(AnimationType.Dead);
            //return WinCondition;
        }
    }
}
