using System.Threading.Tasks;
using Elves.Battle;

namespace Elves.Battles {
    public sealed class RebootElfBattle:BattleScript {

        private async Task Suicide() {
            Player.Kill();
            SetTag("You died.");
            await Speech("Fighting for the rebellion is going to be easier than I thought.","I had no idea humans were so weak.","I didn't even kick you yet.");
            await Tag($"{Actor.Name} still kicked you anwyay.");
            battleResult = BattleResult.PlayerLost;
        }

        private async Task ElfWillKick() {
            SetTag($"{Actor.Name} is about to kick you.");
            var result = await GetButton("Do nothing","Brace for Impact","Scream","Die");
            bool bracedYourself = false, screamed = false;
            switch(result) {
                case Button1:
                    break;
                case Button2:
                    bracedYourself = true;
                    SetTag("You braced yourself.");
                    await Continue();
                    break;
                case Button3:
                    TODO();
                    SetTag("You screamed.");
                    await Continue();
                    SetTag("Everyone's ears are ringing.");
                    await Continue();
                    HideTag();
                    await Speech("Ow! My ears. Now I'm going to kick you even harder!");
                    screamed = true;
                    break;
                case Button4:
                    await Suicide();
                    return;
            }
            Player.Hurt(Player.Health*0.1f*(bracedYourself?0.5f:1)*(screamed?2:1));
            SetTag($"{Actor.Name} kicked you{(screamed ? " really hard" : "")}.");
            await Continue();
            if(bracedYourself) {
                SetTag("Bracing yourself took the edge off.");
                await Continue();
            }
            HideTag();
            ShowSpeech("My only regret is having to touch you to do that.");
            await Continue();
            if(playerIsALiar) {
                ShowSpeech("Especially since you are a liar. I hope it's not contagious.");
            }
            HideSpeech();

            TODO();
        }

        private bool playerIsALiar;

        private async Task PlayerIsALiar() {
            playerIsALiar = true;
            await Speech("Typical human, lying your way through life.");
            if(await GetButton("Apologize","Double Down") == Button1) {
                await Speech("Save it. Your words are empty.");
            } else {
                await Speech("First the lies, then the arrogance. Just like clockwork.");
            }
            await ElfWillKick();
        }

        private async Task HeadInjury() {
            TODO();
        }

        private async Task DrugAddictSafeSpace() {
            await Speech("Look human, I'm not a cop. This is an addiction safe space.");
            if(await GetButton("Gratitude","Get Defensive") == Button1) {
                TODO();
            } else {
                TODO();
            }
        }

        private async Task PlayerDoesntKnowTheirName() {
            await Speech("How strange! Have you had a head injury?");
            if(await YesOrNo("Are you brain damaged?")) {
                await HeadInjury();
            }
            await Speech("No? No brain damage? Okay, are you taking any medications?");
            switch(await GetButton("No","Me? Drugs?","All of them","HIPAA Violation")) {
                case Button1:
                    break;
                case Button2:
                    await DrugAddictSafeSpace();
                    break;
                case Button3:
                    TODO();
                    break;
                case Button4:
                    TODO();
                    break;
            }
        }

        private async Task PlayerHasNoName() {
            await Speech("Really? Are you lying?");
            if(!await YesOrNo("Are you lying?")) {
                await PlayerIsALiar();
            }

        }

        private BattleResult battleResult = BattleResult.Stalemate;

        public override async Task<BattleResult> Main() {
            await Speech("Hello, I'm Reboot Elf.");
            await GetButton("Reboot elf?");
            await Speech("Yes, my name is very unusual. My parents were Holllywood writers.","What's your name?");

            SetTag("What's your name?");
            var result = await GetButton("I don't know","I don't have one","Player","Elf");
            HideTag();

            switch(result) {
                case Button1:
                    await PlayerDoesntKnowTheirName();
                    break;
                case Button2:
                    await PlayerHasNoName();
                    break;
                case Button3:
                    TODO();
                    break;
                case Button4:
                    TODO();
                    break;
            }

            return battleResult;
            
        }

    }
}
