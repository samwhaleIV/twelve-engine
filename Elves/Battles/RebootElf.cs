using System;
using System.Threading.Tasks;
using Elves.Battle.Scripting;
using TwelveEngine;

namespace Elves.Battles {
    public sealed class RebootElf:BattleScript {

        private const int familiarityThreshold = 2;
        private const int unfamiliarityThreshold = 2;

        public override async Task<BattleResult> Main() {

            int familiarity = 0;
            bool tookPills = false;

            bool skipBlackout = false;

            await Tag($"You approach {Actor.Name}.",$"{Actor.Name} is alone.","There is a bottle of pills nearby.");

            while(true) {
                skipBlackout = false;

                await Threader.Speech(ThreadMode.Repeat,"Hello, do I know you?","Sorry, do I know you?","Hey, do I know you?","Have we met before?");

                SetTag($"Do you know {Actor.Name}?");

                int familiar = await Button("Yes","No");
                if(familiar == 0) {
                    familiarity++;
                } else {
                    familiarity--;
                }

                if(familiarity > 0) {
                    if(familiarity >= familiarityThreshold) {
                        await Threader.Speech(ThreadMode.Random,"Oh, hey! How's it going, old pal!","My bad, long time no see, bestie.","My mistake, always happy to see a friend.");
                        await Speech("As you know, my memory is terrible and my head is killing me.","Do you know where my pills are?");
                        SetTag("Do you know where the pills are?");
                        if(await Button("Yes","No") == 0) {
                            await Tag("You point to a nearby table.");
                            await Speech("Ah! Of course that's where I left them!","Thanks a ton.");
                            await Tag($"{Actor.Name} grabs the pill bottle.",$"{Actor.Name} reads the label.");
                            await Speech("...","Wait, have I taken these recently?","I can never remember, but I trust you.");
                            SetTag($"Should {Actor.Name} take pills?");
                            if(await Button("Yes","No") == 0) {
                                await Speech("You're a real pal, thanks.","I hope this helps my headache.");
                                await Tag($"{Actor.Name} swallowed the pills.");
                                if(tookPills) {
                                    await Tag($"{Actor.Name} looks uneasy.");
                                    await Speech("Hmm... That's odd.","This doesn't feel right.");
                                    await Tag($"{Actor.PossessiveName} stomach growls.",$"{Actor.Name} counts the remaining pills.");
                                    await Speech("You... You lied to me?");
                                    await Tag($"{Actor.Name} stares at you blankly.");
                                    await Speech("How could you?");
                                    ActorSprite.SetAnimation(Animation.AnimationType.Dead);
                                    await Tag($"{Actor.Name} passed out.","You make a quick exit.");
                                    return BattleResult.PlayerWon;
                                } else {
                                    await Speech("Mmm. Feeling better already. Thanks, pal.");
                                    ActorSprite.SetAnimation(Animation.AnimationType.Dead);
                                    await Tag($"{Actor.Name} fell over suddenly, hitting their head.");
                                    ActorSprite.SetAnimation(Animation.AnimationType.Idle);
                                    await Tag($"{Actor.Name} Stood back up.");
                                    tookPills = true;
                                    skipBlackout = true;
                                }
                            } else {
                                await Speech("Good call, friend. If I had taken them again, something bad could have happened!","What would I do without you :)");
                            }
                        } else {
                            await Speech("Darn, thanks anyway. I can't remember the last time I took them.","My headaches get worse every day.");
                        }
                    } else {
                        //familiar
                        await Threader.Speech(ThreadMode.Random,"Oh, hey.","Oh yeah, how's it hanging?","Yo, what's up?","Yeah, you do seem a bit familiar.","Wait, are you sure?");
                    }
                } else if(familiarity < 0) {
                    if(familiarity <= -unfamiliarityThreshold) {
                        await Threader.Speech(ThreadMode.Random,"Yeah, you're right. I have no idea who you are.","Oh, yeah, you're right.","Oh, yikes! Stranger danger.");
                        await Speech("I should probably take my pills. but my memory is so terrible.");
                        await Tag($"{Actor.Name} looks around for their pills.");
                        await Speech("Ah, there they are.");
                        await Tag($"{Actor.Name} picks up the pill bottle.");
                        await Speech("... Wait.","How do I know these haven't been tampered with?");
                        await Tag($"{Actor.Name} thinks carefully.");
                        await Speech("So here I am, a stranger in front of me, alone with my pills.","Don't you find that a bit suspicious?");
                        await Tag($"{Actor.Name} shakes the bottle around.");
                        await Speech("Maybe you should take one, you know, just in case. I'm sure it won't hurt.");
                        bool takePill = false;
                        while(!takePill) {
                            SetTag("Take a pill?");
                            if(await Button("Yes","No") == 0) {
                                takePill = true;
                                break;
                            }
                            if(!await Threader.Speech(ThreadMode.StopAtEnd,"No, please, I insist!","Prove yourself trustworthy, take a pill.","Come on, just one pill.","Don't be a baby.","Take the pill.","The first one is free.")) {
                                await Speech("Ugh! Fine! No pills for now, I have enough of a headache as it is.");
                                break;
                            }
                        }
                        if(takePill) {
                            await Tag("You took a pill.","It didn't go down easy.");
                            await Speech("Well? How are you feeling?");
                            await Tag("Your skin turns blue.");
                            await Speech("Hmm, you're not looking so hot.");
                            await Tag("You lose control of your limbs.","You are on the ground.");
                            await Speech("Just as I suspected.","People always try and take advantage of me because of my poor memory.","Well guess what? Not today. Eat dirt.");
                            await Tag("You have suffered multiple organ failures.","You are dead.");
                            return BattleResult.PlayerLost;
                        }      
                    } else {
                        //unfamiliar
                        await Threader.Speech(ThreadMode.Random,"You're probably right.","Are you sure? I can't remember.","I'll take your word for it.","Sounds about right. My memory is shotty.");
                    }
                } else {
                    //neutral
                    await Speech(familiar == 0 ? "Sorry, it takes me a few times to remember someone." : "Sorry, I'll try and forget you.");
                }

                if(!skipBlackout) {
                    await Threader.Tag(ThreadMode.Repeat,$"{Actor.Name} isn't looking so good.",$"{Actor.Name} grabs their head.",$"{Actor.Name} looks pained.",$"{Actor.Name} is hurting.");

                    await Threader.Speech(ThreadMode.Repeat,"My head is killing me.","My head is spinning.","Where are my pills?","My brain hurts.","Where are my painkillers?","The world is spinning.");

                    ActorSprite.SetAnimation(Animation.AnimationType.Dead);
                    await Tag($"{Actor.Name} momentarily blacked out.");
                    ActorSprite.SetAnimation(Animation.AnimationType.Idle);
                }

                await Speech("Ah, excuse me. I don't know what happened.");
            }
        }
    }
}
