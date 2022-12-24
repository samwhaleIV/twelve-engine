using System;
using TwelveEngine.Shell;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace Elves {
    public sealed class StateTransitionTest:InputGameState {

        private static int ColorIndex = 0;

        private static Color[] SceneColors = new Color[] {
            Color.Red, Color.Yellow, Color.Orange, Color.Blue, Color.Indigo, Color.Pink, Color.Green
        };

        private readonly Color color;

        private static Color GetSceneColor() {
            var color = SceneColors[ColorIndex];
            ColorIndex = (ColorIndex + 1) % SceneColors.Length;
            return color;
        }

        public StateTransitionTest(bool fadeIn = false,bool sleep = false) {
            Name = "State Transition Test";

            color = GetSceneColor();
            OnUpdate += UpdateInputs;
            Input.OnAcceptDown += Input_OnAcceptDown;
            if(sleep) {
                OnLoad += () => Thread.Sleep(1000);
            }
            if(fadeIn) {
                TransitionIn(TimeSpan.FromSeconds(0.125f));
            }
        }

        public override void ResetGraphicsState(GraphicsDevice graphicsDevice) {
            base.ResetGraphicsState(graphicsDevice);
            graphicsDevice.Clear(color);
        }

        private void Input_OnAcceptDown() {
            TransitionOut(new StateTransitionTest(fadeIn: true),TimeSpan.FromSeconds(0.125f));
        }
    }
}
