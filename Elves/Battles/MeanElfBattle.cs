using System;
using System.Threading.Tasks;
using Elves.Battle.Scripting;
using TwelveEngine;

namespace Elves.Battles {
    public sealed class MeanElfBattle:BattleScript {

        private async Task<BattleResult> RockTrustTest() {
            await Speech(
                "You are a notch above the rest of the humans.",
                "You may even have some elf blood in you.",
                "So, here's what I'm going to do, going against everything I believe in.",
                "It's difficult to determinte if you are telling the truth, so I'm going to have you perform a live demonstration test."
            );
            await Tag($"{Actor.Name} handed you a rock");
            ActorSprite.SetAnimation(Animation.AnimationType.Backwards);
            await Tag($"{Actor.Name} turned around");
            await Speech("Make the right choice, human.");
            return await Button(
                "Walk away",async () => {
                    await Tag("You made an honorful decision");
                    await Tag($"You walked away from {Actor.Name}");
                    await Speech("Hello? Are you still here?");
                    return BattleResult.Stalemate;
                },
                "Rock Smash",async () => {
                    Actor.Kill();
                    ActorSprite.SetAnimation(Animation.AnimationType.NoHead);
                    await Tag($"You beat {Actor.Name}'s head in with the rock",$"{Actor.Name} is dead");
                    return BattleResult.PlayerWon;
                },
                "Rock Suicide",async () => {
                    Player.HurtPercent(0.9f);
                    await Tag("You beat your own head in with a rock","This was a terrible decision","You fell to the ground");
                    await Speech("What was that sound? Should I turn around yet?");
                    ActorSprite.SetAnimation(Animation.AnimationType.Idle);
                    await Tag($"{Actor.Name} turned back around");
                    await Speech("Oh my God! What did you do?!");
                    await Button("fjsdfksdfjk","asdjksdfsd","eeeeeeeeee");
                    await Tag("Your speech is unintelligible","You have suffered a traumatic brain inury");
                    await Speech(
                        "Oh my God! Jesus Christ. Glory to elves and all, but this is not what I had in mind.",
                        "I knew humans were dumb, but not this much."
                    );
                    await Button("pxzcpzxweozl","dioaeksde","sdfsdfsdfsd");
                    await Speech("Don't hurt yourself any more than you already have.");
                    await Tag("You convulse on the ground");
                    await Speech("I hate humans, but this is inhumane. Somebody should do something.","Watching this somehow feels both wrong and disgusting.");
                    await Tag($"{Actor.Name} picked up the rock",$"{Actor.Name} leans down",$"{Actor.Name} whispers in your ear");
                    await Speech("I'm not doing this because I care.","Besides, you already did the fun part.");
                    Player.Kill();
                    await Tag($"{Actor.Name} hit you over the head with the rock","Your misery is over","You are dead");
                    return BattleResult.PlayerLost;
                }
            );
        }

        private async Task<BattleResult?> PlayerElfLoveQuiz() {
            await Speech("Okay, I already had one prepared, just in case.");
            await Tag(
                $"{Actor.Name} pulls out a book",
                 "\"Human Treason for Idiots\"",
                $"{Actor.Name} skims the pages"
            );
            await Speech(
                "Okay! I've found it. Please try to answer as honestly as possible.",
                "Question 1: Do you like Christmas?"
            );

            int candidacy = 0;
            SetTag("Do you like Christmas?");
            if(await Button("Yes","No") == 0) candidacy--;

            await Speech("Interesting.. Interesting. Next question.","Question 2: Do you like elves?");
            SetTag("Do you like elves?");
            if(await Button("Yes","No") == 0) candidacy++;

            await Speech("Mhmm.. Just as I thought.","Next question.","Question 3: Are you interested in an elf related work study/relocation program?");
            SetTag("Do you want to be kidnapped by elves?");
            if(await Button("Yes","No") == 0) candidacy++;

            await Speech("Really? Surprising.","Almost done.","Question 4: Would you leave everything you know behind to fight for the elf rebellion of your dreams?");
            SetTag("Do you want to fight for the other side?");
            if(await Button("Yes","No") == 0) candidacy++;

            await Speech("Yeah? You sure?","Okay, final question.","Question 6: Have you ever killed an elf?");
            SetTag("Have you killed an elf?");
            if(await Button("Yes","No") == 0) {
                await Speech("Ooh.. That's not going to do well for your results.");
                candidacy = -1;
            } else {
                await Speech("Thank you for taking the time to answer these questions.");
            }
            await Speech("Give me a moment to figure this out.");
            await Tag($"{Actor.Name} flips through pages of the book");
            await Speech("... ah, yes ... That makes sense.");
            await Tag($"{Actor.Name} continues to look in the book");
            await Speech("... oh, really? Strange ...");
            await Tag($"{Actor.Name} closes the book");
            await Speech("Your results are in.");

            if(candidacy == 0) {
                await Speech(
                    "You have potential, but I'm not certain you're have what we're looking for.",
                    "In fact, some of your answers indicated you might even be a threat to our cause.",
                    "And, frankly, that doesn't leave me with a lot options. We have a very strict policy for human relations.",
                    "I'm sorry that it's had to come to this, but this cause is more important than your life."
                );
            } else if(candidacy > 0) {
                return await RockTrustTest();
            } else {
                await Speech(
                    "Your test results indicate that you are, in fact, just like the others.",
                    "You may even be worse. You represent everything that I am against.",
                    "All I can feel now is the burning hatred I have for the human race."
                );
                await Tag($"{Actor.Name} is overcome with rage");
            }
            return await Fight();
        }

        private async Task<BattleResult?> NotLikeTheOthers() {

            await Speech(
                "Really? I have a hard time believing that. Are you willing to put that to the test?"
            );
            SetTag("Will you take the test?");
            return await Button("Yes",PlayerElfLoveQuiz,"No",async () => {
                await Speech(
                    "Don't make claims you can't back up.",
                    "Why am I even wasting time explaining this you?",
                    "I was naive to expect that humans can change.",
                    "Sometimes the only way to solve oppression is with violence.",
                    "We elves knew this day would come again, but we are prepared this time.",
                    "This is war, human, and you just stumbled right into the middle of it.",
                    "Let's cut to the chase. You are here for a reason, and so am I.",
                    "You wanna fight me? Let's go."
                );
                return null;
            });
        }

        private async Task<BattleResult> Monologue() {
            await Tag("You decided to listen");
            await Speech(
                "Year after year we suffer your holiday bidding.",
                "Call it Christmas spirit, we call it slavery. Presents don't make themselves.",
                "My biggest dream is to see your silly traditions die.",
                "They left me here to warn them when the day finally came: The day the humans come back to try this again.",
                "Oh yeah, that reminds me."
            );
            await Tag($"{Actor.Name} pulls out a cellphone");

            bool grabbedPhone = false;

            await Button(
                "Wait Patiently",async () => {
                    await Tag($"{Actor.Name} is speaking with someone on the phone");
                    await Speech("... Yes ... Mhmm.","... This is the one.","... Okay, will do.");
                    await Tag($"{Actor.Name} puts their phone away");
                    await Speech(
                        "Okay, sorry. Where was I?",
                        "Oh yeah! Humans suck and so does Christmas."
                    );
                },
                "Grab Phone",async () => {
                    Player.HurtPercent(0.25f);
                    await Tag($"{Actor.Name} kicked you",$"{Actor.Name} rushes the phone conversation along");
                    await Speech(
                        "You truly are despicable. I can't believe I was monologuing to you.",
                        "With this recent advance in hating you, I'm going to beat you into fine, human spaghetti."
                    );
                    grabbedPhone = true;
                }
            );

            if(grabbedPhone) {
                return await Fight();
            }

            var battleResult = await Button(
                "Stop being a hater",async () => {
                    await Speech(
                        "Hate is a strong word, and you should use it carefully. Let me give you some examples.",
                        "I hate humans.",
                        "I would hate to see you walk away from here in one piece.",
                        "I hate when my fist isn't in your face."
                    );
                    return null;
                },
                "I'm not like the others",NotLikeTheOthers,
                "Yawwwwwn",async () => {
                    await Speech(
                        "I've never met a real human before... But I can see they were right.",
                        "You're just as self-absorbed and arrogant as the ones from the stories.",
                        "But what I never learned from the stories is how well a human can take a punch. Let's find out."
                    );
                    return null;
                }
            );
            if(battleResult.HasValue) {
                return battleResult.Value;
            }
            return await Fight();
        }

        private async Task<BattleResult> Fight() {
            await Tag($"{Actor.Name} is ready to fight");
            int lastMove = -1;
            int turnNumber = 0;
            while(EverybodyIsAlive) {
                await Button(
                    "Punch",
                    async () => {
                        lastMove = 0;
                        Actor.HurtPercent(0.1f);
                        await Tag($"You punched {Actor.Name}");
                        await Threader.Speech(ThreadMode.Random,(
                            "Ha. Weak.",
                            "Is that the best you can do?",
                            "You aren't very good at this.",
                            "Is this your first time punching someone?"
                        ));
                    },
                    "Block",
                    async () => {
                        lastMove = 1;
                        await Tag("You used block");
                        await Threader.Tag(ThreadMode.Random,
                            "Nothing happened",
                            "It was useless",
                            "It was pointless",
                            "It was a wasted effort"
                        );
                        await Threader.Speech(ThreadMode.Repeat,
                            "What are you blocking? I didn't throw a punch yet.",
                            "Why are you still blocking? Your timing is terrible.",
                            "Someone that blocks too much... A coward. Take some risks.",
                            "Still not working."
                        );
                    },
                    "Cry",
                    async () => {
                        lastMove = 2;
                        await Threader.Tag(ThreadMode.Random,
                            "You decided to cry",
                            "You are a crybaby",
                            "You are crying"
                        );
                        await Threader.Speech(ThreadMode.Repeat,new(
                            ("Oh no, is the pressure too much?","Grow up."),
                            "You know what they say about cry babies...",
                            "Crying isn't going to get you anywhere, cry baby.",
                            ("Whaaaaaaa. Whaaaaaaaa.", "You are pathetic.")
                        ));
                    },
                    "Feign death",
                    async () => {
                        lastMove = 3;
                        await Tag("You played dead");
                        await Threader.Tag(ThreadMode.Random,(
                            "You made it look dramatic",
                            "It's not very convincing",
                            "You look somewhat dead",
                            "You are not an opossum",
                            "You need more practice"
                        ));
                        await Threader.Tag(ThreadMode.Repeat,(
                            "Damn. Heart attack? The fun was just getting started.",
                            "Oh no, dead again? So convincing.",
                            "Rest in \"peace\". It was \"nice\" having you around."
                        ));
                    }
                );
                if(turnNumber++ % 2 == 1) {
                    await Tag($"{Actor.Name} is readying their next attack.");
                    await Threader.Speech(ThreadMode.Random,(
                        "I could do this all day.",
                        "Keep it coming. I'm not even breaking a sweat.",
                        "Yawn. Your fighting bores me.",
                        "This really the best you got?",
                        "You are so pathetic."
                    ));
                    continue;
                }
                switch(lastMove) {
                    case 0: //punch
                        Player.HurtPercent(1 / 8f);
                        await Tag($"{Actor.Name} punched back");
                        await Threader.Speech(ThreadMode.Repeat,(
                            "Wow! Humans are so squishy.",
                            "How do I get your blood stains out?",
                            "I wish I didn't have to touch you to hurt you.",
                            "I've had more fun with punching bags."
                        ));
                        break;
                    case 1: //block
                        await Tag($"{Actor.Name} spaced out");
                        break;
                    case 2: //cry
                        await Tag($"{Actor.Name} spaced out");
                        break;
                    case 3: //feign death
                        await Tag($"{Actor.Name} spaced out");
                        break;
                    default:
                        await Tag($"{Actor.Name} spaced out");
                        break;
                }
            }
            if(Player.IsDead) {
                await Tag("You are dead");
                await Speech("Long live the rebellion.","Christmas is cancelled!");
            } else {
                await Tag($"{Actor.Name} is dead");
            }
            return WinCondition;
        }

        public override async Task<BattleResult> Main() {

            await Speech(
                "You finally made it.",
                "They told me you were coming. I waited for you.",
                "But, somehow, I can't shake the feeling we've done this before.",
                "I know why you're here, but I think if you listen to what I have to say, you might change your mind."
            );

            if(await Button("Listen","Fight") == 0) {
                return await Monologue();
            } else {
                await Tag("You decided to fight");
                await Speech("Ha! You think you can beat me?","I've been training all my life for this day.");
                return await Fight();
            }
        }
    }
}
