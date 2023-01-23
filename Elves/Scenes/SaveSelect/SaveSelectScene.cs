using Microsoft.Xna.Framework;
using System;
using TwelveEngine;

namespace Elves.Scenes.SaveSelect {
    public sealed class SaveSelectScene:UIScene {

        private ScrollingBackground background;

        public const int DRAWING_FRAME_WIDTH = 256;
        public const int DRAWING_FRAME_HEIGHT = 64;

        private readonly DrawingFrame[] drawingFrames = new DrawingFrame[3];
        public DrawingFrame[] DrawingFrames => drawingFrames;

        public SaveSelectScene() {
            Name = "Save Selection";
            OnLoad += SaveSelectScene_OnLoad;
            OnRender += SaveSelectScene_OnRender;
            OnUpdate += SaveSelectScene_OnUpdate;
        }

        private void SaveSelectScene_OnUpdate() {
            background.Update(Now);
            UpdateBackgroundScale();
        }
        
        private void UpdateBackgroundScale() {
            backgroundScaleAnimator.Update(Now);
            background.Scale = 1 + BACKGROUND_SCALE_AMOUNT * GetBackgroundScaleT();
        }

        private void SaveSelectScene_OnRender() {
            background.Render(Game.SpriteBatch,Game.Viewport);
            UI.Render(Game.SpriteBatch);
        }

        private void LoadDrawingFrames() {
            for(int i = 0;i<drawingFrames.Length;i++) {
                DrawingFrame drawingFrame = new(DRAWING_FRAME_WIDTH,DRAWING_FRAME_HEIGHT) {
                    DrawColor = Color.White,
                    EmptyColor = Color.Transparent,
                    BrushTexture = Program.Textures.CircleBrush,
                    BrushSize = 5
                };
                drawingFrame.Load(Game.GraphicsDevice);
                //TODO: Drawing frame import
                drawingFrames[i] = drawingFrame;
            }
        }

        private void SaveDrawingFrames() {
            //TODO: call me before we go into a save select screen
            //drawingFrames[0].Export();
        }

        public void DeleteSave(int ID) {
            drawingFrames[ID].Reset(Game);
        }

        private void SaveSelectScene_OnLoad() {
            LoadDrawingFrames();
            UI = new SaveSelectUI(this);
            background = new ScrollingBackground() {
                ScrollTime = TimeSpan.FromSeconds(160f),
                Scale = 1.25f,
                Rotation = -15f,
                Texture = Program.Textures.GiftPattern,
                Direction = new(1,0.27f),
            };
            background.Load(Game.Content);
        }

        private const float BACKGROUND_SCALE_AMOUNT = 1.25f;

        private bool backgroundZoomingIn = false;

        private float GetBackgroundScaleT() {
            return backgroundZoomingIn ? backgroundScaleAnimator.Value : 1 - backgroundScaleAnimator.Value;
        }

        public void BackgroundZoomIn() {
            backgroundZoomingIn = true;
            if(UI is not null) {
                backgroundScaleAnimator.Duration = UI.TransitionDuration;
            }
            backgroundScaleAnimator.ResetCarryOver(Now);
        }

        public void BackgroundZoomOut() {
            backgroundZoomingIn = false;
            if(UI is not null) {
                backgroundScaleAnimator.Duration = UI.TransitionDuration;
            }
            backgroundScaleAnimator.ResetCarryOver(Now);
        }

        private readonly AnimationInterpolator backgroundScaleAnimator = new();
    }
}
