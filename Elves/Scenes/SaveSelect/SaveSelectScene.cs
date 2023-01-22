using Elves.UI.SpriteUI;
using System;
using TwelveEngine;

namespace Elves.Scenes.SaveSelect {
    public sealed class SaveSelectScene:Scene {

        private SpriteBook UI;
        private ScrollingBackground background;

        public SaveSelectScene() {
            Name = "Save Selection";
            OnLoad += SaveSelectScene_OnLoad;
            OnRender += SaveSelectScene_OnRender;
        }
        protected override void UpdateGame() {
            background.Update(Now);
            VectorRectangle viewport = new(Game.Viewport.Bounds);
            UI.Update(Now,viewport);
            UpdateInputs();
            UI.UpdateMouseLocation(Mouse.Position);
            UI.Update(Now,viewport); /* Interaction can be delayed by 1 frame if we don't update the UI again */
            UpdateBackgroundScale();
            Game.CursorState = UI.CursorState;
            UpdateCameraScreenSize();
            Entities.Update();
            UpdateCamera();
        }
        
        private void UpdateBackgroundScale() {
            backgroundScaleAnimator.Update(Now);
            background.Scale = 1 + BACKGROUND_SCALE_AMOUNT * GetBackgroundScaleT();
        }

        private void SaveSelectScene_OnRender() {
            background.Render(Game.SpriteBatch,Game.Viewport);
            UI.Render(Game.SpriteBatch);
        }

        private void SaveSelectScene_OnLoad() {
            UI = new SaveSelectUI(this);
            background = new ScrollingBackground() {
                ScrollTime = TimeSpan.FromSeconds(160f),
                Scale = 1.25f,
                Rotation = -15f,
                Texture = Program.Textures.GiftPattern,
                Direction = new(1,0.27f),
            };
            background.Load(Game.Content);
            Input.OnAcceptDown += UI.AcceptDown;
            Input.OnAcceptUp +=Input_OnAcceptUp;
            Input.OnCancelDown += UI.CancelDown;
            Mouse.OnPress += UI.MouseDown;
            Mouse.OnRelease +=Mouse_OnRelease;
            Input.OnDirectionDown += UI.DirectionDown;
            Mouse.OnMove += UI.MouseMove;
        }

        private void Mouse_OnRelease() {
            UI.MouseUp(Now);
        }

        private void Input_OnAcceptUp() {
            UI.AcceptUp(Now);
        }

        private const float BACKGROUND_SCALE_AMOUNT = 1.25f;

        private bool backgroundZoomingIn = false;

        private float GetBackgroundScaleT() {
            return backgroundZoomingIn ? backgroundScaleAnimator.Value : 1 - backgroundScaleAnimator.Value;
        }

        public void BackgroundZoomIn(TimeSpan now,TimeSpan duration) {
            backgroundZoomingIn = true;
            backgroundScaleAnimator.Duration = duration;
            backgroundScaleAnimator.ResetCarryOver(now);
        }

        public void BackgroundZoomOut(TimeSpan now,TimeSpan duration) {
            backgroundZoomingIn = false;
            backgroundScaleAnimator.Duration = duration;
            backgroundScaleAnimator.ResetCarryOver(now);
        }

        private readonly AnimationInterpolator backgroundScaleAnimator = new();
    }
}
