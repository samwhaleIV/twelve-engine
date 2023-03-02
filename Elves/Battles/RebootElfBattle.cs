using System.Threading.Tasks;
using Elves.Battle.Scripting;
using TwelveEngine;

namespace Elves.Battles {
    public sealed class RebootElfBattle:BattleScript {
        
        private async Task OtherQuestions(string message) {
            await Speech(message);
            SetTag("What do you want to know?");
            await Button(
                "Yes",async () => {
                    await Threader.Speech(ThreadMode.HoldLast,new(
                        (
                            "Huh? What?",
                            "That's not a question, I can't answer this."
                        ),
                            "That's still not a question! I still can't answer this!",
                            "You are wasting your time!",
                        (
                            "Please stop. Yes is not a question.",
                            "The second the laws of conversation change, you will be the first to know.",
                            "But until that day comes, please stop asking me \"yes\"."
                        ),
                        (
                            "Wow, okay. You are more stubborn than I thought.",
                            "We elves spend all our childhoods reading stories about little stubborn human kids.",
                            "I always thought they were just stories... Until now."         
                        ),
                        (
                            "Okay. Fine. I've got an answer for you.",
                            "The answer... It's around here somewhere."
                        ),
                        new(async () => {
                            await Speech("Okay... I have the answer. Are you ready?");
                            SetTag("Are you ready?");
                            await Button("Yes","No");
                            await Speech("Doesn't matter, it was a hypothetical question.","Here is your answer...","Yes.","Yes yes.","A resounding yes.");
                        }),
                        "*rolls eyes* Yes..."
                    ));
                    await OtherQuestions("Anything else you want to know?");
                },
                "Why am I here?",async () => {
                    await Speech("That's a question I ask myself every day.","Unfortunately, I don't have an answer for you.");
                },
                "Meaning of life.",async () => {
                    await Speech("Look, human. We elves have a lot on our plate.","We don't have any time to waste answering pointless questions.");
                },
                "No",async () => await Speech("Okay, moving on.")
            );
        }

        private async Task OtherQuestionsDefault() {
            await OtherQuestions("What do you want to know?");
        }

        private async Task HelpImLost() {
            await Speech("You're in the North Pole. Any other questions?");
            SetTag("Any other questions?");
            await Button("Yes",OtherQuestionsDefault,"No",async () => await Speech("Okay, moving on..."));
        }

        public override async Task<BattleResult> Main() {
            await Speech("So, you finally made it.");
            await Speech("But, somehow, I can't shake the feeling that this all seems very familiar.");
            await Speech("Let's get started. Do you know how to use buttons?");

            SetTag("Do you know how to use buttons?");
            await Button(
                "Yes",async () => await Speech("Ah, an old pro. Perhaps we have done this before?"),
                "No", async () => await Speech("Ah, a quick learner."),
                "Help, I'm lost", HelpImLost
            );

            Actor.Kill();
            await Tag($"{Actor.Name} killed themselves.");

            return BattleResult.Stalemate;
        }
    }
}
