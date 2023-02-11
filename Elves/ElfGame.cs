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

namespace Elves {
    public static class ElfGame {

        private static readonly TimeSpan TransitionDuration = AnimationTiming.TransitionDuration;

        /// <summary>
        /// Start the game! Everything that happens (not engine wise) stems from here. The entry point... of doom.
        /// </summary>
        /// <returns>The start state for the game.</returns>
        public static GameState Start() => GetCarouselMenu();

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

        private static GameState GetStartSceneForSave() {
            if(Program.Save.HasFlag(SaveKeys.PlayedIntro)) {
                return GetCarouselMenu();
            } else {
                return GetIntroScene();
            }
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
            Program.Save.SetFlag(SaveKeys.PlayedIntro);
            scene.TransitionOut(new() {
                Generator = () => {
                    Program.Save.TrySave();
                    return GetCarouselMenu();
                },
                Data = StateData.FadeIn(TransitionDuration),
                Duration = quickExit ? AnimationTiming.QuickTransition : AnimationTiming.IntroFadeOutDuration,
            });
        }

        private static void SplashSceneExit(GameState scene) => scene.TransitionOut(new() {
            Generator = GetSaveSelectScene,
            Data = StateData.FadeIn(TransitionDuration),
            Duration = TransitionDuration
        });

        private static GameState GetBattleScene(ElfID elfID) {
            Elf elf = ElfManifest.Get(elfID);
            BattleScript battleScript = elf.ScriptGenerator.Invoke();
            battleScript.ElfSource = elf;
            BattleSequencer battleSequencer = new(battleScript);
            battleSequencer.OnBattleEnd += BattleSequencerExit;
            return battleSequencer;
        }

        private static void BattleSequencerExit(GameState scene,BattleResult result,ElfID ID) {
            bool animateProgress = false;
            var save = Program.Save;
            if(save is null) {
                Logger.WriteLine("Cannot update save data because a save file is not referenced at this time.",LoggerLabel.Save);
                animateProgress = true;
            } else {
                save.SetValue(SaveKeys.LastCompletedBattle,(int)ID);
                if(result == BattleResult.PlayerWon) {
                    animateProgress = true;
                    save.TryGetInt(SaveKeys.HighestCompletedBattle,out int oldValue,-1);
                    if((int)ID > oldValue) {
                        save.SetValue(SaveKeys.HighestCompletedBattle,(int)ID);
                    }
                }
                save.IncrementCounter(result == BattleResult.PlayerWon ? SaveKeys.WinCount : SaveKeys.LoseCount);
                Program.Save.TrySave();
            }
            if((int)ID == ElfManifest.Count - 1) {
                throw new NotImplementedException();
            }
            scene.TransitionOut(new() {
                Generator = animateProgress ? GetCarouselMenuAnimatedProgress : GetCarouselMenu,
                Data = StateData.FadeIn(TransitionDuration),
                Duration = TransitionDuration
            });
        }

        private static void CarouselMenuExit(GameState scene,bool backToMenu,ElfID elfID = ElfID.None) {
            if(backToMenu) {
                scene.TransitionOut(new TransitionData() {
                    Generator = GetSaveSelectScene,
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
