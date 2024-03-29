﻿using System;
using TwelveEngine.Shell;
using Microsoft.Xna.Framework;
using System.Threading;

namespace Elves.Scenes.Test {
    public sealed class StateTransitionTest:InputGameState {

        private static int ColorIndex = 0;

        private static readonly Color[] SceneColors = new Color[] {
            Color.Red, Color.Yellow, Color.Orange, Color.Blue, Color.Indigo, Color.Pink, Color.Green
        };

        private static Color GetSceneColor() {
            var color = SceneColors[ColorIndex];
            ColorIndex = (ColorIndex + 1) % SceneColors.Length;
            return color;
        }

        private readonly bool testLoadSleep;

        public StateTransitionTest(bool testLoadSleep = false) {
            this.testLoadSleep = testLoadSleep;

            Name = "State Transition Test";
            ClearColor = GetSceneColor();
            ImpulseHandler.Router.OnAcceptDown += AcceptDown;
            OnLoad.Add(Load);
        }

        private void Load() {
            if(!testLoadSleep) {
                return;
            }
            OnLoad.Add(() => Thread.Sleep(1000));
        }

        private void AcceptDown() {
            TransitionOut(new TransitionData() {
                Generator = () => new StateTransitionTest(),
                Duration = TimeSpan.FromSeconds(0.125f),
                Data = new StateData() {
                    Flags = StateFlags.CarryInput & StateFlags.FadeIn,
                    TransitionDuration = TimeSpan.FromSeconds(0.125f)
                }
            });
        }
    }
}
