using Elves.Battle.Sprite.Elves;
using Elves.UI.Battle;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elves.Battle.Battles {
    public class DebugBattle:BattleSequencer {

        private class DebugScript:Script {
            public override async Task<int> Main() {
                while(true) {
                    var pressedButton = await GetButton("EAT","MY","ASS","PLEASE");
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
                    //await Speech("The developer has lost his mind.");
                }
            }
        }

        public DebugBattle():base(new DebugScript(),"Backgrounds/checkerboard") {
            OnLoad += DebugBattle_OnLoad;
        }

        private void DebugBattle_OnLoad() {
            Player.Name.Append("PLAYER");

            var elf = new HarmlessElf();
            Entities.Add(elf);
            Target.Color = elf.Color;
            Target.Name.Append(elf.Name.ToUpperInvariant());
            SetBackgroundColor(elf.Color);

            Script.BattleActor = elf;
        }
    }
}
