using TwelveEngine.Shell;
using System;
using Elves.Scenes.Carousel;
using Elves.Scenes.Intro;
using Elves.Scenes.SaveSelect;
using Elves.Scenes.SplashMenu;

namespace Elves {
    public static class ElfGame {

        /// <summary>
        /// Start the game! Everything that happens (not engine wise) stems from here. The entry point... of doom.
        /// </summary>
        /// <returns>The start state for the game.</returns>
        public static GameState Start() {
            Program.Game.OnStateLoaded += Game_OnStateLoaded;
            return GetSplashMenu();
        }

        private static void Game_OnStateLoaded(GameState state) {
            if(!state.FadeInIsFlagged) {
                return;
            }
            state.TransitionIn(Constants.AnimationTiming.TransitionDuration);
        }

        private static GameState State => Program.Game.State;

        private static GameState GetSplashMenu() {
            var scene = new SplashMenuState();
            scene.OnSceneExit += SplashMenu_OnSceneExit;
            return scene;
        }

        private static GameState GetSaveSelectScene() {
            var scene = new SaveSelectScene();
            scene.OnSceneExit += SaveSelect_OnSceneExit;
            return scene;
        }

        private static GameState GetCarouselMenu() {
            var scene = new CarouselMenu();
            scene.OnStartElfBattle += CarouselMenu_OnStartElfBattle;
            return scene;
        }

        private static GameState GetIntroScene() {
            var scene = new IntroScene();
            scene.OnSceneExit += IntroScene_OnSceneEnd;
            return scene;
        }


        private static GameState GetStartSceneForSave() {
            if(Program.SaveFile.HasFlag(SaveKeys.IntroHasPlayed)) {
                return GetCarouselMenu();
            } else {
                return GetIntroScene();
            }
        }

        private static void SaveSelect_OnSceneExit(SaveSelectScene scene,int saveID) {
            Program.SaveFile = Program.SaveFiles[saveID];
            scene.TransitionOut(new() {
                Generator = GetStartSceneForSave,
                Data = new() { Flags = StateFlags.ForceGC | StateFlags.FadeIn },
                Duration = Constants.AnimationTiming.TransitionDuration
            });
        }

        private static void IntroScene_OnSceneEnd(bool quickExit) {
            State.TransitionOut(new() {
                Generator = GetCarouselMenu,
                Data = StateData.FadeIn,
                Duration = quickExit ? Constants.AnimationTiming.QuickTransition : Constants.AnimationTiming.IntroFadeOutDuration,
            });
        }

        private static void SplashMenu_OnSceneExit() {
            State.TransitionOut(new() {
                Generator = GetSaveSelectScene,
                Data = StateData.FadeIn,
                Duration = Constants.AnimationTiming.TransitionDuration
            });
        }

        private static void CarouselMenu_OnStartElfBattle(int elfID) {
            throw new NotImplementedException();
        }
    }
}
