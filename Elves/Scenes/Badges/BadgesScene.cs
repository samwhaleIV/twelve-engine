using System;
using System.Collections.Generic;
using TwelveEngine;
using TwelveEngine.Shell;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Elves.Scenes.Badges {
    using static Constants.AnimationTiming;
    public class BadgesScene:GameState {

        private readonly Timeline _timeline = new();

        public event Action OnSceneEnd;

        private readonly Badge[] _badges;

        private readonly Queue<(int Stage, TimeSpan duration)> _timelineStages = new();
        private readonly Dictionary<int,(int BadgeIndex,BadgeRenderState State)> _stageLookup = new();

        private enum BadgeRenderState { FadeIn, Hold, FadeOut, None }

        private (Badge Badge,BadgeRenderState State) GetCurrentBadge() {
            var (BadgeIndex,State) = _stageLookup[_timeline.Stage];
            return (_badges[BadgeIndex],State);
        }

        public BadgesScene(Badge[] badges) {
            _badges = badges;
            OnUpdate.Add(UpdateTimeline);
            OnRender.Add(RenderBadges);

            int stage = 1;
            void AddStage(TimeSpan duration,int badgeIndex,BadgeRenderState state) {
                var id = stage++;
                _timelineStages.Enqueue((id,duration));
                _stageLookup[id] = (badgeIndex,state);
            }

            _stageLookup[0] = (0, BadgeRenderState.None);

            for(int i = 0;i<badges.Length;i++) {
                AddStage(BadgeFadeInDuration,i,BadgeRenderState.FadeIn);
                AddStage(BadgeHoldDuration,i,BadgeRenderState.Hold);
                if(i == badges.Length-1) {
                    break;
                }
                AddStage(BadgeFadeOutDuration,i,BadgeRenderState.FadeOut);
            }
            AddStage(IntroFadeOutDuration,badges.Length-1,BadgeRenderState.Hold);

            _timeline.SetTimelineAutoDuration(_timelineStages);

            OnWriteDebug.Add(_timeline.WriteDebug);
        }

        private void ExitScene() {
            OnSceneEnd?.Invoke();
        }

        private bool exiting = false;

        private void UpdateTimeline() {
            _timeline.Update(LocalRealTime);
            if(_timeline.Stage >= _timeline.StageCount - 1 || exiting) {
                return;
            }
            exiting = true;
            ExitScene();
        }

        private void RenderBadges() {
            var badge = GetCurrentBadge();
            if(badge.State == BadgeRenderState.None) {
                return;
            }
            float colorStrength = 1;
            if(badge.State != BadgeRenderState.Hold) {
                colorStrength = _timeline.LocalT;
            }
            if(badge.State == BadgeRenderState.FadeOut) {
                colorStrength = 1 - colorStrength;
            }
            Texture2D texture = badge.Badge.Texture;

            FloatRectangle badgeArea = new(Viewport);
            badgeArea.Position = badgeArea.Center;
            badgeArea.Size = new Vector2(texture.Width,texture.Height) * badge.Badge.Scale;
            badgeArea.Position -= badgeArea.Size * 0.5f;

            SpriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            SpriteBatch.Draw(texture,badgeArea.ToRectangle(),Color.FromNonPremultiplied(new Vector4(colorStrength)));
            SpriteBatch.End();
        }
    }
}
