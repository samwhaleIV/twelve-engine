using System.Threading.Tasks;
using Elves.Battle.Scripting;

namespace Elves.Battles {
    public sealed class RebootElfBattle:BattleScript {
        public override async Task<BattleResult> Main() {
            await Tag("Elide me!","Round 2, electric boogaloo.","Did you fix it yet? Keep trying...");
            await Tag("Round 2, electric boogaloo.");
            await Tag("Did you fix it yet? Keep trying...");
            await Tag("Elide me!");
            await Tag("Round 2, electric boogaloo.");
            await Tag("Did you fix it yet? Keep trying...");
            await Tag("Elide me!");
            await Tag("Round 2, electric boogaloo.");
            await Tag("Did you fix it yet? Keep trying...");

            await Speech("Hello, you are a baka.");
            SetTag("Are you a baka?");
            await Button("No, I am not",async () => {
                await Tag("You lied.");
                await Speech("Don't lie to me. I've always known the truth.");
            },"Yes, I am",async () => {
                await Tag("You told the truth.");
                await Speech("See. I knew it! I could smell the baka from a mile away.");
            });
            await Speech("You know, I hate bakas...");
            Player.HurtPercent(0.25f);
            await Tag($"{Actor.Name} slapped you.",$"{Actor.Name} became infected.");
            await Speech("Oh my God, what have you done?");
            await Tag($"The baka has spread to {Actor.Name}.");
            Actor.HurtPercent(0.25f);
            await Tag($"{Actor.Name} begins to die.");
            await Speech("This is never how I expected to die.");
            Actor.HurtPercent(0.5f);
            await Speech("Tell my family I hated them.");
            await Tag($"{Actor.Name} is on the brink of death.");
            Actor.Kill();
            await Tag($"{Actor.Name} died.");
            return BattleResult.PlayerWon;
        }
    }
}
