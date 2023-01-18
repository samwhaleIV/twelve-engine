using Elves.Scenes.SplashMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using TwelveEngine;
using TwelveEngine.Font;
using TwelveEngine.Shell;
using TwelveEngine.Shell.UI;

namespace Elves.Scenes.Intro {
    public sealed class IntroScene:InputGameState {

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
            Input.OnCancelDown += Input_OnCancelDown;
            if(Flags.Get(Constants.Flags.Debug)) {
                Input.OnAcceptDown += Debug_Reset;
            }
            OnWriteDebug += IntroScene_OnWriteDebug;
            OnLoad += IntroScene_OnLoad;
            OnUnload += IntroScene_OnUnload;
        }

        private void Input_OnCancelDown() {
            ExitScene(quickExit: true);
        }

        private Song song;

        private void IntroScene_OnUnload() {
            song?.Dispose();
            song = null;
        }

        private TimeSpan sceneDuration;

        private enum TimelineStage {
            None, StartDelay, Text, FadeDelay, FadeDuration
        }

        private List<(TimelineStage Stage, double KeyFrame)> timeline = new() { (TimelineStage.None, 0) };

        private (TimelineStage Stage,float LocalT,float GlobalT) timelineStage = (TimelineStage.None, 0, 0);

        private (TimelineStage Stage, float LocalT, float GlobalT) GetTimelineStage() {
            double t = (Now - startTimeOffset) / sceneDuration;
            if(t >= 1) {
                return (timeline[^1].Stage,1,1);
            } else if(t < 0) {
                return (timeline[0].Stage,0,0);
            }
            for(int i = 1;i<timeline.Count;i++) {
                (TimelineStage Stage, double KeyFrame) item = timeline[i];
                double start = timeline[i-1].KeyFrame;
                double localT = (t - start) / (item.KeyFrame - start);
                if(localT <= 1) {
                    return (item.Stage,(float)localT, (float)t);
                }
            }
            return (TimelineStage.None, 0,(float)t);
        }

        private void AddTimelineValue(TimelineStage stage,double length) {
            timeline.Add((stage,length+timeline[^1].KeyFrame));
        }

        private void SetAnimationTimeline() {
            sceneDuration = song.Duration + Constants.AnimationTiming.IntroSongDurationOffset;

            double total = 1;

            double startDelayLength = Constants.AnimationTiming.IntroStartDelay / sceneDuration;
            total -= startDelayLength;

            double exitDuration = Constants.AnimationTiming.IntroFadeOutDuration / sceneDuration;
            total -= exitDuration;

            double endDelayLength = Constants.AnimationTiming.IntroFadeOutDelay / sceneDuration;
            total -= endDelayLength;

            if(total <= 0) {
                Logger.WriteLine($"Warning: The intro timeline is longer than '{song.Name}' ({sceneDuration})'. Is the song too short?");
                total = 0;
            }
            double textLength = total;

            AddTimelineValue(TimelineStage.StartDelay,startDelayLength);
            AddTimelineValue(TimelineStage.Text,textLength);
            AddTimelineValue(TimelineStage.FadeDelay,endDelayLength);
            AddTimelineValue(TimelineStage.FadeDuration,exitDuration);
        }

        private void IntroScene_OnLoad() {
            song = Game.Content.Load<Song>(Constants.Songs.Intro);
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = false;
            MediaPlayer.Volume = 1f;

            SetAnimationTimeline();
        }

        private TimeSpan startTimeOffset = TimeSpan.Zero;

        private void Debug_Reset() {
            startTimeOffset = Now;
            exiting = false;
        }

        private void IntroScene_OnWriteDebug(DebugWriter writer) {
            writer.ToTopLeft();
            writer.Write(timelineStage.GlobalT,"Global Time");
            writer.Write(timelineStage.LocalT,"Stage Time");
            writer.Write(timelineStage.Stage.ToString(),"Stage Name");
        }

        private bool exiting = false;

        private void ExitScene(bool quickExit = false) {
            if(exiting) {
                return;
            }
            exiting = true;
            if(quickExit) {
                MediaPlayer.Stop();
            }
            TransitionOut(new TransitionData() {
                Generator = () => new SplashMenuState(),
                Duration = quickExit ? Constants.AnimationTiming.QuickTransition : Constants.AnimationTiming.IntroFadeOutDuration,
                Data = new StateData() { Flags = StateFlags.FadeIn }
            });
        }

        private void IntroScene_OnUpdate() {
            UpdateInputs();
            timelineStage = GetTimelineStage();
            if(exiting) {
                return;
            }
            if(timelineStage.Stage >= TimelineStage.FadeDuration) {
                ExitScene(quickExit: false);
                timelineStage = GetTimelineStage();
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

        private Color GetTextColor(float t,float index) {

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
            timelineStage = GetTimelineStage();
            if(timelineStage.Stage < TimelineStage.Text) {
                return;
            }
            float t = timelineStage.Stage == TimelineStage.Text ? timelineStage.LocalT : 1;
            int end = Math.Min((int)(t * Text.Length) + 1,Text.Length);

            Vector2 size = Game.Viewport.Bounds.Size.ToVector2();
            float rowSize = size.Y / (Text.Length + 1);
            int centerX = (int)(size.X * 0.5f);
            int textSize = GetTextSize();

            Fonts.RetroFont.Begin(Game.SpriteBatch);
            for(int i = 0;i<end;i++) {
                Color color = GetTextColor(t,i);
                Point destination = new(centerX,(int)(rowSize*(i+1)));
                Fonts.RetroFont.DrawCentered(Text[i],destination,textSize,color);
            }
            Fonts.RetroFont.End();
        }
    }
}
