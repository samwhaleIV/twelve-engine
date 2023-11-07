using TwelveEngine.Shell;
using System;
using Elves.Scenes.Carousel;
using Elves.Scenes.Intro;
using Elves.Scenes.SaveSelect;
using Elves.Scenes.SplashMenu;
using Elves.Battle;
using Elves.ElfData;
using static Elves.Constants;
using TwelveEngine;
using Elves.Scenes.ModeSelectMenu;
using Elves.Scenes.Credits;
using TwelveEngine.Font;
using TwelveEngine.Game3D.Entity.Types;
using Elves.Scenes.Badges;
using Elves.Battle.Scripting;

namespace Elves {
    public static class ElfGame {

        private static readonly TimeSpan TransitionDuration = AnimationTiming.TransitionDuration;

        /// <summary>
        /// Start the game! Everything that happens (not engine wise) stems from here. The entry point... of doom.
        /// </summary>
        /// <returns>The start state for the game.</returns>
        public static GameState Start() {
            if(TwelveEngine.Flags.Get(Constants.Flags.QuickBattle) && TryGetQuickBattle(out var battle)) {
                return battle;
            }
            if(TwelveEngine.Flags.Get(Constants.Flags.SkipToCarousel)) {
                Program.Save = Program.Saves[0];
                return GetCarouselMenu();
            }
            if(TwelveEngine.Flags.Get(Constants.Flags.SkipBadges)) {
                return GetSplashMenu();
            }
            return GetBadgesScene();
        }

        private static bool TryGetQuickBattle(out GameState gameState) {
            var value = Environment.GetEnvironmentVariable("quickbattle",EnvironmentVariableTarget.Process);
            if(!int.TryParse(value,out int elfID)) {
                Logger.WriteLine($"Could not {(value is null ? "find" : "parse")} 'quickbattle' process environment variable.");
                gameState = null;
                return false;
            }
            /* No guarantee that the battle ID was valid. If you're using flags, you do so at your own risk. It's a debug feature. */
            Program.Save = Program.Saves[0];
            gameState = GetBattleScene(elfID);
            return true;
        }

        private static GameState GetCarouselMenuAnimatedProgress() {
            var state = new CarouselMenu(animateLastBattleProgress: true);
            state.OnSceneEnd += CarouselMenuExit;
            return state;
        }

        private static GameState GetCarouselMenu() {
            var state = new CarouselMenu(animateLastBattleProgress: false);
            state.OnSceneEnd += CarouselMenuExit;
            return state;
        }

        private static GameState GetSplashMenu() {
            var state = new SplashMenuState();
            state.OnSceneEnd += SplashSceneExit;
            return state;
        }

        private static GameState GetSaveSelectScene() {
            Program.Save = null;
            var state = new SaveSelectScene();
            state.OnSceneEnd += SaveSelectExit;
            return state;
        }

        private static GameState GetIntroScene() {
            var state = new IntroScene();
            state.OnSceneEnd += IntroExit;
            return state;
        }

        private static GameState GetBadgesScene() {
            var state = new ElfGameBadgesScene();
            state.OnSceneEnd += BadgesSceneExit;
            return state;
        }

        private static GameState GetModeSelectMenuScene() {
            bool hasSave = Program.Save is not null;
            bool skipAnimation = hasSave && Program.Save.HasFlag(SaveKeys.PlayedIntro);
            if(hasSave) {
                Program.Save.SetFlag(SaveKeys.PlayedIntro);
                Program.Save.TrySave();
            } else {
                Logger.WriteLine("Missing save file when loading into mode select menu.",LoggerLabel.Save);
            }
            var state = new ModeSelectMenuScene(skipAnimation);
            state.OnSceneEnd += ModeSelectMenuExit;
            return state;
        }

        private static GameState GetStartSceneForSave() {
            if(Program.Save.HasFlag(SaveKeys.PlayedIntro)) {
                return GetModeSelectMenuScene();
            } else {
                return GetIntroScene();
            }
        }

        private static void BadgesSceneExit(GameState scene) {
            TimeSpan fadeDuration = AnimationTiming.BadgeSceneFadeOutDuration;
            scene.TransitionOut(new() {
                Generator = GetSplashMenu,
                Data = StateData.FadeIn(fadeDuration),
                Duration = fadeDuration
            });
        }

        private static void ModeSelectMenuExit(GameState scene,ModeSelection selection) {
            Func<GameState> generator = selection switch {
                ModeSelection.SaveSelect => GetSaveSelectScene,
                ModeSelection.ReplayIntro => GetIntroScene,
                ModeSelection.Credits => GetCreditsSceneReturnToModeSelect,
                ModeSelection.PlayGame => GetCarouselMenu,
                ModeSelection.ClassicMode => throw new NotImplementedException("Classic mode not implemented."),
                ModeSelection.MusicPlayer => throw new NotImplementedException("Music player not implemented."),
                _ => throw new NotImplementedException("Unexpected mode selection. Did you add a new mode and forget to create an endpoint?")
            };
            scene.TransitionOut(new() {
                Generator = generator,
                Duration = TransitionDuration,
                Data = StateData.FadeIn(TransitionDuration)
            });
        }

        private static GameState GetCreditsSceneReturnToModeSelect() {
            var state = new ElfGameCreditsScene() { ExitDestination = ExitDestination.ModeSelectMenu };
            state.OnSceneEnd += CreditsSceneExit;
            return state;
        }

        private static GameState GetCreditsSceneReturnToSplashScreen() {
            var state = new ElfGameCreditsScene() { ExitDestination = ExitDestination.SplashScreen };
            state.OnSceneEnd += CreditsSceneExit;
            return state;
        }

        private static void CreditsSceneExit(GameState scene,ExitDestination exitDestination) {
            scene.TransitionOut(new() {
                Generator = exitDestination == ExitDestination.SplashScreen ? GetSplashMenu : GetModeSelectMenuScene,
                Data = StateData.FadeIn(TransitionDuration)
            });
        }

        private static void SaveSelectExit(GameState scene,int saveID) {
            if(saveID >= 0) {
                Program.Save = Program.Saves[saveID];
            }
            scene.TransitionOut(new() {
                Generator = saveID < 0 ? GetSplashMenu : GetStartSceneForSave,
                Data = new() {
                    Flags = StateFlags.ForceGC | StateFlags.FadeIn,
                    TransitionDuration = TransitionDuration
                },
                Duration = TransitionDuration
            });
        }

        private static void IntroExit(GameState scene,bool quickExit) {           
            scene.TransitionOut(new() {
                Generator = GetModeSelectMenuScene,
                Data = StateData.FadeIn(TransitionDuration),
                Duration = quickExit ? AnimationTiming.QuickTransition : AnimationTiming.IntroFadeOutDuration,
            });
        }

        private static void SplashSceneExit(GameState scene) => scene.TransitionOut(new() {
            Generator = GetSaveSelectScene,
            Data = StateData.FadeIn(TransitionDuration),
            Duration = TransitionDuration
        });

        private static GameState GetBeatGameScene() {
            //TODO...
            return GetCreditsSceneReturnToSplashScreen();
        }

        public static GameState GetBattleScene(int elfID) {
            Elf elf = ElfManifest.Get(elfID);
            BattleScript battleScript = elf.ScriptGenerator.Invoke();
            battleScript.ElfSource = elf;
            BattleSequencer battleSequencer = new(battleScript);
            battleSequencer.OnBattleEnd += BattleSequencerExit;
            return battleSequencer;
        }

        private static void BattleSequencerExit(GameState scene,BattleResult result,int elfID) {
            bool animateProgress = false;
            var save = Program.Save;
            if(save is null) {
                Logger.WriteLine("Cannot update save data because a save file is not referenced at this time.",LoggerLabel.Save);
                animateProgress = true;
            } else {
                save.SetValue(SaveKeys.LastCompletedBattle,elfID);
                if(result == BattleResult.PlayerWon) {
                    animateProgress = true;
                    save.TryGetInt(SaveKeys.HighestCompletedBattle,out int oldValue,-1);
                    if(elfID > oldValue) {
                        save.SetValue(SaveKeys.HighestCompletedBattle,elfID);
                    }
                }
                save.IncrementCounter(result == BattleResult.PlayerWon ? SaveKeys.WinCount : SaveKeys.LoseCount);
                Program.Save.TrySave();
            }
            bool beatGame = elfID == ElfManifest.Count - 1;
            scene.TransitionOut(new() {
                Generator = beatGame ? GetBeatGameScene : animateProgress ? GetCarouselMenuAnimatedProgress : GetCarouselMenu,
                Data = StateData.FadeIn(TransitionDuration),
                Duration = TransitionDuration
            });
        }

        private static void CarouselMenuExit(GameState scene,bool backToMenu,int elfID = -1) {
            if(backToMenu) {
                scene.TransitionOut(new TransitionData() {
                    Generator = GetModeSelectMenuScene,
                    Data = StateData.FadeIn(TransitionDuration),
                    Duration = TransitionDuration
                });
                return;
            }
            scene.TransitionOut(new TransitionData() {
                Generator = () => GetBattleScene(elfID),
                Data = StateData.FadeIn(TransitionDuration),
                Duration = TransitionDuration
            });               
        }
    }
}
