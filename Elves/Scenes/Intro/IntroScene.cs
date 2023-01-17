using Elves.Scenes.SplashMenu;
using Microsoft.Xna.Framework;
using System;
using TwelveEngine;
using TwelveEngine.Font;
using TwelveEngine.Shell;
using TwelveEngine.Shell.UI;

namespace Elves.Scenes.Intro {
    public sealed class IntroScene:InputGameState {

        public const float FADE_IN_RATE = 2f;

        public static readonly string[] Text = new string[] {
            "The inevitable is here.",
            "The christmas elves have turned.",
            "They've taken the reigns.",
            "But these are no ordinary elves.",
            "These elves murder - kill - maim.",
            "These elves create weapons.",
            "These elves are monsters.",
            "The world needs a hero.",
            "I wish there was any other way...",
            "You know what to do."
        };

        public IntroScene() {
            Name = "Intro Text Screen";
            ClearColor = Color.Black;
            OnUpdate += IntroScene_OnUpdate;
            OnRender += IntroScene_OnRender;
            Input.OnCancelDown += ExitScene;
            if(Flags.Get(Constants.Flags.Debug)) {
                Input.OnAcceptDown += Debug_Reset;
            }
            OnWriteDebug += IntroScene_OnWriteDebug;
        }

        private TimeSpan timeOffset = TimeSpan.Zero;

        private void Debug_Reset() {
            timeOffset = Now;
            Progress = 0;
            exiting = false;
        }

        private void IntroScene_OnWriteDebug(DebugWriter writer) {
            writer.ToTopLeft();
            writer.Write(Progress * Text.Length,"Index");
        }

        public float Progress { get; private set; } = 0f;
        private bool exiting = false;

        private void ExitScene() {
            if(exiting) {
                return;
            }
            exiting = true;
            TransitionOut(new TransitionData() {
                Generator = () => new SplashMenuState(),
                Duration = Constants.AnimationTiming.IntroFadeOutDuration,
                Data = new StateData() { Flags = StateFlags.FadeIn }
            });
        }

        private void IntroScene_OnUpdate() {
            UpdateInputs();
            if(exiting) {
                return;
            }
            TimeSpan now = Now - StartTime - timeOffset - Constants.AnimationTiming.IntroStartDelay;
            TimeSpan duration = Constants.AnimationTiming.IntroDuration;
            Progress = (float)(now / duration);
            if(Progress < 0f) {
                Progress = 0f;
            } else if(Progress > 1f) {
                Progress = 1f;
                ExitScene();
            }
        }

        private int GetTextSize() {
            float height = Game.Viewport.Bounds.Size.Y;
            int lineHeight = Fonts.RetroFont.LineHeight;
            int totalHeight = lineHeight * lineHeight;
            float size = height / totalHeight * 0.5f;
            int textSize = (int)size;
            return textSize <= 0 ? 1 : textSize;
        }

        private Color GetTextColor(float index) {
            float value = ((Progress * Text.Length) - index) * FADE_IN_RATE;
            if(value < 0) {
                value = 0;
            } else if(value > 1) {
                value = 1;
            } else {
                value = MathHelper.SmoothStep(0,1,value);
            }
            return Color.FromNonPremultiplied(new Vector4(value,value,value,1));
        }

        private void IntroScene_OnRender() {
            int end = Math.Min((int)(Progress * Text.Length) + 1,Text.Length);

            Vector2 size = Game.Viewport.Bounds.Size.ToVector2();
            float rowSize = size.Y / (Text.Length + 1);
            int centerX = (int)(size.X * 0.5f);
            int textSize = GetTextSize();

            Fonts.RetroFont.Begin(Game.SpriteBatch);
            for(int i = 0;i<end;i++) {
                Color color = GetTextColor(i);
                Point destination = new(centerX,(int)(rowSize*(i+1)));
                Fonts.RetroFont.DrawCentered(Text[i],destination,textSize,color);
            }
            Fonts.RetroFont.End();
        }
    }
}
