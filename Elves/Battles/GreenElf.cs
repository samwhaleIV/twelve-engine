using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elves.Battle;
using Elves.Battle.Scripting;
using TwelveEngine;

namespace Elves.Battles {
    public sealed class GreenElf:BattleScript {

        private enum Affinity {
            SuperBad = -3,
            Bad = -2,
            KindaBad = -1,
            Neutral = 0,
            KindaGood = 1,
            Good = 2,
            SuperGood = 3,
            Elf1 = 4,
            Elf2 = 5,
            Elf3 = 6,
            Elf4 = 7,
            Unknown = 8
        };

        private const string THREAT_LEVEL = "Eco-Threat Level";
        private const string ALLY_LEVEL = "Eco-Ally Level";

        private readonly Dictionary<Affinity,string> threatLevelDescriptions = new() {
            { Affinity.SuperBad, $"{THREAT_LEVEL}: Online Shopper" },
            { Affinity.Bad, $"{THREAT_LEVEL}: Combustion Engine" },
            { Affinity.KindaBad, $"{THREAT_LEVEL}: Litterer" },
            { Affinity.Neutral, $"{THREAT_LEVEL}: Carbon Neutral" },
            { Affinity.KindaGood, $"{ALLY_LEVEL}: Tree" },
            { Affinity.Good, $"{ALLY_LEVEL}: Renewable Energy" },
            { Affinity.SuperGood, $"{ALLY_LEVEL}: Eco-Terrorist" },
            { Affinity.Elf1, $"{ALLY_LEVEL}: Seasoned" },
            { Affinity.Elf2, $"{ALLY_LEVEL}: Jaded" },
            { Affinity.Elf3, $"{ALLY_LEVEL}: Enraged" },
            { Affinity.Elf4, $"{ALLY_LEVEL}: Nuclear Fusion" },
            { Affinity.Unknown, $"{THREAT_LEVEL}: Unknown" }
        };

        private Affinity playerAffinity;
        private Affinity elfAffinity;

        private enum AffinityChange { None, Drop, Raise }

        private const Affinity MinPlayerAffinity = Affinity.SuperBad;
        private const Affinity MaxPlayerAffinity = Affinity.SuperGood;

        private const Affinity MinElfAffinity = Affinity.Elf1;
        private const Affinity MaxElfAffinity = Affinity.Elf3;

        private void UpdatePlayerAffinity(Affinity affinity) {
            playerAffinity = affinity;
            Player.Info = threatLevelDescriptions[affinity];
        }

        private void UpdateElfAffinity(Affinity affinity) {
            elfAffinity = affinity;
            Actor.Info = threatLevelDescriptions[affinity];
        }

        private Affinity UpdateAffinity(UserData target,Affinity affinity,Affinity min,Affinity max,AffinityChange change) {
            if(affinity != MinPlayerAffinity && change == AffinityChange.Raise) {
                affinity += 1;
            } else if(affinity != MaxPlayerAffinity && change == AffinityChange.Drop) {
                affinity -= 1;
            }
            target.Info = threatLevelDescriptions[affinity];
            return affinity;
        }

        private static AffinityChange CompareAffinities(Affinity before,Affinity after) {
            if(before == after) {
                return AffinityChange.None;
            } else if(after > before) {
                return AffinityChange.Raise;
            } else {
                return AffinityChange.Drop;
            }
        }

        private AffinityChange UpdateAffinityPlayer(AffinityChange affinityChange) {
            Affinity startAffinity = playerAffinity;
            playerAffinity = UpdateAffinity(Player,playerAffinity,MinPlayerAffinity,MaxPlayerAffinity,affinityChange);
            return CompareAffinities(startAffinity,playerAffinity);
        }

        private AffinityChange UpdateAffinityElf(AffinityChange affinityChange) {
            Affinity startAffinity = elfAffinity;
            elfAffinity = UpdateAffinity(Actor,elfAffinity,MinElfAffinity,MaxElfAffinity,affinityChange);
            return CompareAffinities(startAffinity,elfAffinity);
        }

        private readonly struct Quizlet {
            public string ElfPrompt { get; init; }
            public string TagPrompt { get; init; }
            public QuizletOption[] Options { get; init; }
        }

        private readonly struct QuizletOption {
            public string Text { get; init; }
            public string[] Speeches { get; init; }
            public AffinityChange PlayerAffinityChange { get; init; }
            public AffinityChange ElfAffinityChange { get; init; }
        }

        private enum BattleEndState {
            Continue,
            PlayerIsBad,
            PlayerIsGood,
            ElfIsAngry
        };

        private BattleEndState GetEndState() {
            return BattleEndState.Continue;
        }

        private async Task BroadcastAffinityModificationPlayer(AffinityChange affinityChange) {
            Affinity affinity = playerAffinity;
            if(affinityChange == AffinityChange.None) {
                if(affinity != MinPlayerAffinity && affinity != MaxPlayerAffinity) {
                    return;
                }
                await Tag("Your Eco-Affinity is unchanged.");
                if(affinity == MinPlayerAffinity) {
                    await Threader.Tag(ThreadMode.Random,new("You are still a terrible person.","You still suck.","You are still a bad person.","You're worse than the oil industry.",("You're making history.","... Not in a good way.")));
                } else if(affinity == MaxPlayerAffinity) {
                    await Threader.Tag(ThreadMode.Random,"Keep doing what you're doing.","You are a tree hugger.","Your eyes sparkle like a solar panel.","You eat kale for breakfast.","Your ancestors regale you.","Everyone is celebrating.");
                }
                return;
            } else if(affinityChange == AffinityChange.Raise) {
                await Tag("Your Eco-Affinity improved.");
                await Threader.Tag(ThreadMode.Random,"The climate thanks you.","You taste the fresh air.","The trees are happy.","Another car is taken off the road.","Your carbon footprint is lessening.");
            } else {
                await Tag("Your Eco-Affinity worsened.");
                await Threader.Tag(ThreadMode.Random,"The planet didn't like that.","Your ancestors won't be thanking you.","The environment cried.","You're only hurting yourself.");
            }
        }

        private async Task BroadcastAffinityModificationElf(AffinityChange affinityChange) {
            if(affinityChange == AffinityChange.None) {
                await Threader.Tag(ThreadMode.Random,$"{Actor.Name} can't take it much longer.",$"{Actor.Name} is ready to explode.",$"{Actor.Name} needs to save the planet.");
                return;
            } else if(affinityChange == AffinityChange.Raise) {
                await Tag($"{Actor.PossessiveName} Eco-Affinity increased.");
            } else {
                await Tag($"{Actor.PossessiveName} Eco-Affinity decreased.");
            }
            await Threader.Tag(ThreadMode.Random,"This much rage is unhealthy.","The trees cower in fear.","The climate is changing.");
        }

        public override async Task<BattleResult> Main() {

            UpdateElfAffinity(MinElfAffinity);

            await Tag($"You approach {Actor.Name}.");
            await Speech("Oh great, another person on this overpopulated rock.");
            await Tag($"{Actor.Name} looks annoyed.");
            await Speech("The environement is very important to me ... Which your disgusting, all-consuming kind has destroyed.","You're not the only one who lives on this planet, you know.");
            await Tag($"{Actor.Name} looks around inquisitively.");
            await Speech("So, are you like me or are you just another idling gas guzzler?");
            SetTag("Do you care about the environment?");

            if(await Button("Yes","No") == 0) {
                await Speech("Really? You sure? We'll see.");
            } else {
                await Speech("Not surprising, but I respect your honesty.","The first step to change is acceptance.");
            }

            await Tag($"{Actor.Name} hands you a mysterious device.");

            await Speech($"My beloved 'Eco-Tracker,' patent pending.","On the surface, it measures environmental sentiment and impact. (Powered by the tears of oil companies.)","If you work against me or our lovely planet, let's just say I'm something of a radical.","Press the button to begin.");

            bool setDownPrompt = false;
            while(true) {
                SetTag("Activate Eco-Tracker?");
                if(await Button("Yes","No") == 0) {
                    break;
                }
                Player.Hurt(20);
                await Threader.Tag(ThreadMode.Repeat,"The Eco-Tracker electrocuted you.","The Eco-Tracker zapped you.","The Eco-Tracker shocked you.");
                if(Player.IsAlive && !setDownPrompt) {
                    SetTag("Let go of Eco-Tracker?");
                    if(await Button("Yes","No") == 0) {
                        await Tag("You tried to set it down but it's stuck to your hand.");
                        Player.Hurt(5);
                        await Tag("The Eco-Tracker stabbed you with a sharp needle.");
                        setDownPrompt = true;
                    }
                }
                if(Player.IsDead) {
                    await Tag("You are dead.");
                    await Speech("Ahh, carbon neutral at last.","Return to the soil, dirt.","Bye bye.");
                    return BattleResult.PlayerLost;
                }
                await Threader.Speech(ThreadMode.StopAtEnd,"I have a few special features built in.","The Eco-Tracker will improve the climate at any cost.","If you don't activate the tracker, its default precepts are to kill the offending user.","Ultimately, the choice is yours.");
            }

            UpdatePlayerAffinity(Affinity.Unknown);
            await Tag("Your Eco-Tracker is now active.","The Eco-Tracker is calculating your Eco-Affinity.");

            UpdatePlayerAffinity(Affinity.Neutral);
            await Tag("Your Eco-Affinity is neutral.");

            await Speech("Now that the device is fully operational, let's ask you a few questions.","Think of this as a kind of practice for both of us.");

            List<Quizlet> quizlets = this.quizlets.ToList();
            List<string> buttonBuffer = new();

            while(quizlets.Count > 0) {
                int quizletIndex = Random.Next(0,quizlets.Count);
                Quizlet quizlet = quizlets[quizletIndex];
                quizlets.RemoveAt(quizletIndex);
                foreach(var option in quizlet.Options) {
                    buttonBuffer.Add(option.Text);
                }
                await Speech(quizlet.ElfPrompt);
                SetTag(quizlet.TagPrompt);
                int optionIndex = await Button(buttonBuffer);
                buttonBuffer.Clear();
                QuizletOption selectedOption = quizlet.Options[optionIndex];
                foreach(var speech in selectedOption.Speeches) {
                    await Speech(speech);
                }
                await BroadcastAffinityModificationPlayer(
                    UpdateAffinityPlayer(selectedOption.PlayerAffinityChange)
                );
                await BroadcastAffinityModificationElf(
                    UpdateAffinityElf(selectedOption.ElfAffinityChange)
                );

                //todo
            }

            await Button("BOOB");

            return BattleResult.PlayerWon;
        }

        private readonly Quizlet[] quizlets = {
            new Quizlet() {
                ElfPrompt = "",
                TagPrompt = "",
                Options = new QuizletOption[] {
                    new() {
                        Text = "",
                        Speeches = new string[] { },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.None
                    },
                }
            },
        };
    }
}
