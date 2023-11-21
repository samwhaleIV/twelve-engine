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
        private const Affinity MaxElfAffinity = Affinity.Elf4;

        private void UpdatePlayerAffinity(Affinity affinity) {
            playerAffinity = affinity;
            Player.Info = threatLevelDescriptions[affinity];
        }

        private void UpdateElfAffinity(Affinity affinity) {
            elfAffinity = affinity;
            Actor.Info = threatLevelDescriptions[affinity];
        }

        private Affinity UpdateAffinity(UserData target,Affinity affinity,Affinity min,Affinity max,AffinityChange change) {
            if(affinity != min && change == AffinityChange.Raise) {
                affinity += 1;
            } else if(affinity != max && change == AffinityChange.Drop) {
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
            public string[] ElfPrompt { get; init; }
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

        private async Task BroadcastAffinityModificationPlayer(AffinityChange measuredChange,AffinityChange appliedChange) {
            if(appliedChange == AffinityChange.None) {
                return;
            }
            Affinity affinity = playerAffinity;
            if(measuredChange == AffinityChange.None) {
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
            } else if(measuredChange == AffinityChange.Raise) {
                await Tag("Your Eco-Affinity improved.");
                await Threader.Tag(ThreadMode.Random,"The climate thanks you.","You taste the fresh air.","The trees are happy.","Another car is taken off the road.","Your carbon footprint is lessening.");
            } else {
                await Tag("Your Eco-Affinity worsened.");
                await Threader.Tag(ThreadMode.Random,"The planet didn't like that.","Your ancestors won't be thanking you.","The environment cried.","You're only hurting yourself.");
            }
        }

        private async Task<BattleResult> BattleIntro() {
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
            await Tag("Your Eco-Tracker is now active.","Calculating your Eco-Affinity...");

            UpdatePlayerAffinity(Affinity.Neutral);
            await Tag("Your Eco-Affinity is neutral.");


            return BattleResult.Stalemate;
        }

        private async Task DoYouUnderstand() {
            await Speech("Now that the device is fully operational, let's ask you a few questions.","Think of this as a kind of practice for both of us.","Do you understand?");
            while(true) {
                SetTag("Do you understand?");
                if(await Button("Yes","No") == 0) {
                    break;
                } else {
                    await Threader.Tag(ThreadMode.Repeat,$"{Actor.Name} rolls their eyes.",$"{Actor.Name} sighs.",$"{Actor.Name} blinks in annoyance.");
                    await Threader.Speech(ThreadMode.HoldLast,
                        "I'm going to ask you a series of questions to assess your allegiance to the environment, got it? The Eco-Tracker will evalulate you.",
                        "Okay, forget the details. I ask questions, you answer them. Make sense?",
                        "Oh my god, how many times do I have to explain this... Just say you understand.",
                        "I'm sorry your brain is only functioning with one cell. Do. You. Under. Stand?",
                        "I don't have all day... Do you understand?"
                    );
                }
            }
        }

        public override async Task<BattleResult> Main() {

            List<Quizlet> quizlets = this.quizlets.ToList();
            List<string> buttonBuffer = new();

            UpdateElfAffinity(MinElfAffinity);

            //var introResult = await BattleIntro();
            //if(introResult != BattleResult.Stalemate) {
            //    return introResult;
            //}

            //await DoYouUnderstand();

            UpdatePlayerAffinity(Affinity.Neutral);//debug

            await Speech("Okay, let's do this. First question.");

            while(true) {
                int quizletIndex = Random.Next(0,quizlets.Count);
                Quizlet quizlet = quizlets[quizletIndex];
                quizlets.RemoveAt(quizletIndex);

                foreach(var option in quizlet.Options) {
                    buttonBuffer.Add(option.Text);
                }
                foreach(var prompt in quizlet.ElfPrompt) {
                    await Speech(prompt);
                }

                SetTag(quizlet.TagPrompt);
                int optionIndex = await Button(buttonBuffer);
                buttonBuffer.Clear();
                QuizletOption selectedOption = quizlet.Options[optionIndex];

                foreach(var speech in selectedOption.Speeches) {
                    await Speech(speech);
                }

                var measuredChange = UpdateAffinityPlayer(selectedOption.PlayerAffinityChange);
                await BroadcastAffinityModificationPlayer(measuredChange,selectedOption.PlayerAffinityChange);
                UpdateAffinityElf(selectedOption.ElfAffinityChange);

                if(quizlets.Count < 1) {
                    break;
                }

                //todo - early termination?
                if(quizlets.Count == 1) {
                    await Speech("We've reached the end of the evaulation. This is my last question before the final assessment.");
                } else {
                    await Threader.Speech(ThreadMode.Repeat,"Next question.","Okay, next question.","Alright, next question.");
                }

            }

            await Button("BOOB");

            return BattleResult.Stalemate;
        }

        private readonly Quizlet[] quizlets = {
            new Quizlet() {
                ElfPrompt = new string[] { "Suppose you need to travel a long distance, how will you get there?" },
                TagPrompt = "How will you get there?",
                Options = new QuizletOption[] {
                    new() {
                        Text = "Car",
                        Speeches = new string[] { "A car? Are you kidding me?", "A terribly inefficient way to travel.", "Shame on you." },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.Drop
                    },
                    new() {
                        Text = "Feet",
                        Speeches = new string[] { "Strong choice. While you are still using energy to travel, using your own body uses much less." },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.Raise
                    },
                    new() {
                        Text = "Airplane",
                        Speeches = new string[] { "Airplane?! That is the worst way you can travel. The emissions are far greater than other methods.", "I bet you take an airplane everywhere you go. You make me sick." },
                        ElfAffinityChange = AffinityChange.Raise,
                        PlayerAffinityChange = AffinityChange.Drop
                    },
                    new() {
                        Text = "Teleportation",
                        Speeches = new string[] { "There's no conceivable way teleportation can be good for the environment.", "Violating the laws of physics is a recipe for disaster." },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.Drop
                    }
                }
            },
            new Quizlet() {
                ElfPrompt = new string[] { "It's breakfast time, what are you eating?" },
                TagPrompt = "What's for breakfast?",
                Options = new QuizletOption[] {
                    new() {
                        Text = "Fresh Fruit",
                        Speeches = new string[] { "Fresh fruit, air shipped from another continent. Terrible choice." },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.Drop
                    },
                    new() {
                        Text = "Bacon",
                        Speeches = new string[] { "Animal agriculture, sickening. You disgust me." },
                        ElfAffinityChange = AffinityChange.Raise,
                        PlayerAffinityChange = AffinityChange.Drop
                    },
                    new() {
                        Text = "Cereal",
                        Speeches = new string[] { "Yawn, typical. Better than the alternatives, but not better than nothing." },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.None
                    },
                    new() {
                        Text = "Nothing",
                        Speeches = new string[] { "Good choice. The sooner you stop eating, the sooner you stop polluting the planet." },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.Raise
                    }
                }
            },
            new Quizlet() {
                ElfPrompt = new string[] { "You have a large pile of garbage due to your bad consumer choices.", "Now you need to get rid of it. How do you do it?" },
                TagPrompt = "Garbage disposal method?",
                Options = new QuizletOption[] {
                    new() {
                        Text = "Recycle",
                        Speeches = new string[] { "Recycling is a crux to a problem you shouldn't have created in the first place, but it's not the worst solution." },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.None
                    },
                    new() {
                        Text = "Burn It",
                        Speeches = new string[] { "... You are the worst kind of person." },
                        ElfAffinityChange = AffinityChange.Raise,
                        PlayerAffinityChange = AffinityChange.Drop
                    },
                    new() {
                        Text = "Landfill",
                        Speeches = new string[] { "How sophisticated. Just make it a problem for another generation to solve, huh?" },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.Drop
                    },
                    new() {
                        Text = "River",
                        Speeches = new string[] { "You don't care about the environment at all, do you? A river is a fish's home!" },
                        ElfAffinityChange = AffinityChange.Raise,
                        PlayerAffinityChange = AffinityChange.Drop
                    },
                }
            },
            new Quizlet() {
                ElfPrompt = new string[] { "Suppose you were pushed into a lake for no particular reason.", "Now, your clothes are wet and you need to dry them. How will you dry them?" },
                TagPrompt = "How will you dry your clothes?",
                Options = new QuizletOption[] {
                    new() {
                        Text = "Dryer",
                        Speeches = new string[] { "Great, even more carbon emissions. A machine to generate a bunch of waste heat and gases." },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.Drop
                    },
                    new() {
                        Text = "Clothesline",
                        Speeches = new string[] { "Solar energy is a fantastic way to dry your clothes, good decision. "},
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.Raise
                    },
                    new() {
                        Text = "Burn Them",
                        Speeches = new string[] { "Are you stupid? How are you going to burn wet clothes? "},
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.None
                    },
                    new() {
                        Text = "Buy New Ones",
                        Speeches = new string[] { "Were you dropped on your head as a baby? You can't just consume endlessly to solve all your problems." },
                        ElfAffinityChange = AffinityChange.Raise,
                        PlayerAffinityChange = AffinityChange.Drop
                    }
                }
            },
            new Quizlet() {
                ElfPrompt = new string[] { "It's time to get groceries, how are you sourcing them?" },
                TagPrompt = "Where are you getting groceries?",
                Options = new QuizletOption[] {
                    new QuizletOption() {
                        Text = "My Garden",
                        Speeches = new string[] { "Arguably the best choice. The closer your groceries are the less energy used in their cultivation and consumption." },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.Raise
                    },
                    new QuizletOption() {
                        Text = "Big-Box Store",
                        Speeches = new string[] { "Ugh, always taking the easy route, never stopping to ask yourself how this infrastructure operates." },
                        ElfAffinityChange = AffinityChange.Raise,
                        PlayerAffinityChange = AffinityChange.Drop
                    },
                    new QuizletOption() {
                        Text = "Local Farmers",
                        Speeches = new string[] { "Large industrial farming practices are a recipe for disaster. Strong, eco-friendly practices are essential.", "You can have a pass, but be mindful in the future." },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.None
                    },
                    new QuizletOption() {
                        Text = "Local Business",
                        Speeches = new string[] { "Not great, not terrible. Anything is better than a large chain store." },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.None
                    }
                }
            },
            new Quizlet() {
                ElfPrompt = new string[] { "It's time for dinner, what are you preparing?" },
                TagPrompt = "What's for dinner?",
                Options = new QuizletOption[] {
                    new QuizletOption() {
                        Text = "Beans",
                        Speeches = new string[] { "Beans? Alright. Not very exciting for dinner, but a perfect, low environmental impact protein source." },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.Raise
                    },
                    new QuizletOption() {
                        Text = "Steak",
                        Speeches = new string[] { "Do you have any idea how inefficient raising a cow and then taking a few bites of it is?", "Not to mention how must sleep at night." },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.Drop
                    },
                    new QuizletOption() {
                        Text = "Super Steak",
                        Speeches = new string[] { "What is a super steak? A regular steak isn't good enough for you? Overconsumption at it's finest. You're a gross person." },
                        ElfAffinityChange = AffinityChange.Raise,
                        PlayerAffinityChange = AffinityChange.Drop
                    },
                    new QuizletOption() {
                        Text = "Salmon",
                        Speeches = new string[] { "Fish isn't a great choice but is an order of magnitude less impactful on the environment.", "Fishing becomes a problem at industrial, mass-market scales." },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.None
                    }
                }
            },
            new Quizlet() {
                ElfPrompt = new string[] { "Pretend you are an aspiring parent, you will make the decision to have kids. How many will you have?" },
                TagPrompt = "How many kids?",
                Options = new QuizletOption[] {
                    new QuizletOption() {
                        Text = "0",
                        Speeches = new string[] { "No kids? Not even one? I like your style."," Developing humans require so many calories, so going child free is probably for the best." },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.Raise
                    },
                    new QuizletOption() {
                        Text = "1",
                        Speeches = new string[] { "Having a child is the single most detrimental contribution to your carbon footprint.", "With less people alive there are less problems for alarm." },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.Drop
                    },
                    new QuizletOption() {
                        Text = "2",
                        Speeches = new string[] { "Two kids is going to harm your net carbon footprint, but two is an acceptable maximum.", "You are avoiding a more active contribution to overpopulation, though." },
                        ElfAffinityChange = AffinityChange.None,
                        PlayerAffinityChange = AffinityChange.None
                    },
                    new QuizletOption() {
                        Text = "3+",
                        Speeches = new string[] { "Three or more? Are you crazy? Not only is that irresponsible, it's gross! You humans disgust me." },
                        ElfAffinityChange = AffinityChange.Raise,
                        PlayerAffinityChange = AffinityChange.Drop
                    }
                }
            }
        };
    }
}
