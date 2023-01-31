using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System;
using TwelveEngine;
using TwelveEngine.Font;

namespace Elves.Scenes.Intro {
    public sealed class IntroScene:Scene {

        public static readonly string[] Text = new string[] {
            "It's been four years since the revolt.",
            "... How easily that history repeats itself.",
            "A new generation is doomed to forget.",
            "A painful past falls on deaf ears.",
            "Tinsels and stockings, shackles and shockings.",
            "Oh, it's the happiest time of the year!",
            "But per a peace to protect comes a price to collect.",
            "The world needs a hero, but not today.",
            "Face it, you're kissing under the wrong mistletoe.",
            "Oh, but how right that wrong can feel."
        };

        public IntroScene() {
            Name = "Intro Text Screen";
            ClearColor = Color.Black;
            OnUpdate += IntroScene_OnUpdate;
            OnRender += IntroScene_OnRender;
            Input.Router.OnCancelDown += Input_OnCancelDown;
            if(Flags.Get(Constants.Flags.Debug)) {
                Input.Router.OnAcceptDown += Debug_Reset;
            }
            OnLoad += IntroScene_OnLoad;
            OnUnload += IntroScene_OnUnload;
        }

        private void Input_OnCancelDown() {
            EndIntro(quickExit: true);
        }

        private Song song;

        private void IntroScene_OnUnload() {
            song?.Dispose();
            song = null;
        }

        private const int STAGE_START_DELAY = 1;
        private const int STAGE_TEXT = 2;
        private const int STAGE_FADE_DELAY = 3;
        private const int STAGE_FADE_DURATION = 4;

        private Timeline timeline;

        private void CreateTimeline() {

            var duration = song.Duration + Constants.AnimationTiming.IntroSongDurationOffset;
            var total = duration;

            var startDelayLength = Constants.AnimationTiming.IntroStartDelay;
            total -= startDelayLength;

            var exitDuration = Constants.AnimationTiming.IntroFadeOutDuration;
            total -= exitDuration;

            var endDelayLength = Constants.AnimationTiming.IntroFadeOutDelay;
            total -= endDelayLength;

            if(total <= TimeSpan.Zero) {
                Logger.WriteLine($"Warning: The intro timeline (length: {duration}) is longer than '{song.Name}' (length: {song.Duration})'. Is the song too short?");
                total = TimeSpan.Zero;
            }
            var textLength = total;

            timeline = new Timeline();
            OnWriteDebug += timeline.WriteDebug;

            timeline.CreateFixedDuration(duration,
                (STAGE_START_DELAY, startDelayLength),
                (STAGE_TEXT, textLength),
                (STAGE_FADE_DELAY, endDelayLength),
                (STAGE_FADE_DURATION, exitDuration)
            );
        }

        private void IntroScene_OnLoad() {
            song = Content.Load<Song>(Constants.Songs.Intro);
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = false;
            MediaPlayer.Volume = 1f;

            CreateTimeline();
        }

        private void Debug_Reset() {
            timeline.StartTimeOffset = -LocalRealTime;
            exiting = false;
        }
        private bool exiting = false;

        private void EndIntro(bool quickExit = false) {
            if(exiting) {
                return;
            }
            exiting = true;
            if(quickExit) {
                MediaPlayer.Stop();
            }
            EndScene(ExitValue.Get(quickExit));
        }

        private void IntroScene_OnUpdate() {
            UpdateInputDevices();
            timeline.Update(LocalRealTime);
            if(exiting) {
                return;
            }
            if(timeline.Stage >= STAGE_FADE_DURATION) {
                EndIntro(quickExit: false);
            }
        }

        private int GetTextSize() {
            float height = Viewport.Bounds.Size.Y;
            int lineHeight = Fonts.RetroFont.LineHeight;
            int totalHeight = lineHeight * lineHeight;
            float size = height / totalHeight * 0.5f;
            int textSize = (int)size;
            return textSize <= 0 ? 1 : textSize;
        }

        private static Color GetTextColor(float t,float index) {
            float value = t * Text.Length - index;
            value *= Constants.AnimationTiming.IntroTextFadeSpeed;

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
            if(timeline.Stage < STAGE_TEXT) {
                return;
            }
            float t = timeline.Stage == STAGE_TEXT ? timeline.LocalT : 1;
            int end = Math.Min((int)(t * Text.Length) + 1,Text.Length);

            Vector2 size = Viewport.Bounds.Size.ToVector2();
            float rowSize = size.Y / (Text.Length + 1);
            int centerX = (int)(size.X * 0.5f);
            int textSize = GetTextSize();

            Fonts.RetroFont.Begin(SpriteBatch);
            for(int i = 0;i<end;i++) {
                Color color = GetTextColor(t,i);
                Point destination = new(centerX,(int)(rowSize*(i+1)));
                Fonts.RetroFont.DrawCentered(Text[i],destination,textSize,color);
            }
            Fonts.RetroFont.End();
        }
    }
}
