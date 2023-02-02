using TwelveEngine.Shell;
using System;
using Elves.Scenes.Carousel;
using Elves.Scenes.Intro;
using Elves.Scenes.SaveSelect;
using Elves.Scenes.SplashMenu;
using Elves.Scenes;
using Elves.Battle;
using Elves.ElfData;
using static Elves.Constants;

namespace Elves {
    public static class ElfGame {

        private static readonly TimeSpan TransitionDuration = AnimationTiming.TransitionDuration;

        /// <summary>
        /// Start the game! Everything that happens (not engine wise) stems from here. The entry point... of doom.
        /// </summary>
        /// <returns>The start state for the game.</returns>
        public static GameState Start() => GetSplashMenu();

        /* Christ almighty... */
        private static TBase GetScene<TBase, TSuper>(Action<TBase,ExitValue> onSceneEnd) where TBase : GameState, IScene<TBase> where TSuper : TBase, new() {
            TSuper state = new();
            state.OnSceneEnd += onSceneEnd;
            return state;
        }

        private static GameState GetSplashMenu() => GetScene<Scene3D,SplashMenuState>(SplashSceneExit);
        private static GameState GetSaveSelectScene() => GetScene<Scene,SaveSelectScene>(SaveSelectExit);
        private static GameState GetCarouselMenu() => GetScene<Scene3D,CarouselMenu>(CarouselMenuExit);
        private static GameState GetIntroScene() => GetScene<Scene,IntroScene>(IntroExit);

        private static GameState GetStartSceneForSave() {
            if(Program.Save.HasFlag(SaveKeys.PlayedIntro)) {
                return GetCarouselMenu();
            } else {
                return GetIntroScene();
            }
        }

        private static void SaveSelectExit(Scene scene,ExitValue data) {
            Program.Save = Program.Saves[data.SaveID];
            scene.TransitionOut(new() {
                Generator = GetStartSceneForSave,
                Data = new() {
                    Flags = StateFlags.ForceGC | StateFlags.FadeIn,
                    TransitionDuration = TransitionDuration
                },
                Duration = TransitionDuration
            });
        }

        private static void IntroExit(Scene scene,ExitValue data) {
            Program.Save.SetFlag(SaveKeys.PlayedIntro);
            scene.TransitionOut(new() {
                Generator = () => {
                    Program.Save.TrySave();
                    return GetCarouselMenu();
                },
                Data = StateData.FadeIn(TransitionDuration),
                Duration = data.QuickExit ? AnimationTiming.QuickTransition : AnimationTiming.IntroFadeOutDuration,
            });
        }

        private static void SplashSceneExit(Scene3D scene,ExitValue data) => scene.TransitionOut(new() {
            Generator = GetSaveSelectScene,
            Data = StateData.FadeIn(TransitionDuration),
            Duration = TransitionDuration
        });

        private static GameState GetBattleScene(ElfID elfID) {
            Elf elf = ElfManifest.Get(elfID);
            BattleScript battleScript = elf.ScriptGenerator.Invoke();
            battleScript.ElfSource = elf;
            BattleSequencer battleSequencer = new(battleScript);
            battleSequencer.OnSceneEnd += BattleSequencerExit;
            return battleSequencer;
        }

        private static void BattleSequencerExit(Scene3D scene,ExitValue data) {
            BattleResult result = data.BattleResult;
            //todo.. update save data
            scene.TransitionOut(new() {
                Generator = GetCarouselMenu,
                Data = StateData.FadeIn(TransitionDuration),
                Duration = TransitionDuration
            });
        }

        private static void CarouselMenuExit(Scene3D scene,ExitValue data) {
            ElfID elfID = data.BattleID;
            scene.TransitionOut(new TransitionData() {
                Generator = () => GetBattleScene(elfID),
                Data = StateData.FadeIn(TransitionDuration),
                Duration = TransitionDuration
            });               
        }
    }
}
