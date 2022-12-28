using System;
using TwelveEngine.Shell;
using Microsoft.Xna.Framework;
using System.Threading;

namespace Elves.TestStates {
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

        public StateTransitionTest(bool fadeIn = false,bool sleep = false) {
            Name = "State Transition Test";
            ClearColor = GetSceneColor();
            OnUpdate += UpdateInputs;
            Input.OnAcceptDown += Input_OnAcceptDown;
            if(sleep) {
                OnLoad += () => Thread.Sleep(1000);
            }
            if(fadeIn) {
                TransitionIn(TimeSpan.FromSeconds(0.125f));
            }
        }

        private void Input_OnAcceptDown() {
            TransitionOut(new TransitionData() {
                Generator = () => new StateTransitionTest(fadeIn: true),
                Duration = TimeSpan.FromSeconds(0.125f),
                Data = StateData.CarryInput
            });
        }
    }
}
