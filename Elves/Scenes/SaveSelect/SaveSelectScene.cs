using Microsoft.Xna.Framework;
using System;
using TwelveEngine;

namespace Elves.Scenes.SaveSelect {
    public sealed class SaveSelectScene:UIScene {

        private ScrollingBackground background;

        public const int DRAWING_FRAME_WIDTH = 256, DRAWING_FRAME_HEIGHT = 64;

        private readonly DrawingFrame[] drawingFrames = new DrawingFrame[3];
        public DrawingFrame[] DrawingFrames => drawingFrames;


        public event Action<SaveSelectScene,int> OnSceneExit;

        private bool _saveDrawingFramesOnUnload = false;

        public void OpenSaveFile(int saveFileID) {
            _saveDrawingFramesOnUnload = true;
            OnSceneExit?.Invoke(this,saveFileID);
        }

        public SaveSelectScene() {
            Name = "Save Selection";
            OnLoad += SaveSelectScene_OnLoad;
            OnRender += SaveSelectScene_OnRender;
            OnUpdate += SaveSelectScene_OnUpdate;
            OnUnload += SaveSelectScene_OnUnload;
        }

        private void SaveSelectScene_OnUnload() {
            if(!_saveDrawingFramesOnUnload) {
                return;
            }
            SaveDrawingFrames();
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
                drawingFrames[i] = drawingFrame;
            }

            /* Not the most elegant solution, but the fixed 3 save states is so static in the design of the game (and this UI) that it is not expected to ever change. */


            if(Program.GlobalSaveFile.TryGetBytes(SaveKeys.SaveImage1,out byte[] imageData)) {
                drawingFrames[0].Import(imageData);
            }

            if(Program.GlobalSaveFile.TryGetBytes(SaveKeys.SaveImage2,out imageData)) {
                drawingFrames[1].Import(imageData);
            }

            if(Program.GlobalSaveFile.TryGetBytes(SaveKeys.SaveImage3,out imageData)) {
                drawingFrames[2].Import(imageData);
            }
        }

        private void SaveDrawingFrames() {
            Program.GlobalSaveFile.SetBytes(SaveKeys.SaveImage1,drawingFrames[0].Export());
            Program.GlobalSaveFile.SetBytes(SaveKeys.SaveImage2,drawingFrames[1].Export());
            Program.GlobalSaveFile.SetBytes(SaveKeys.SaveImage3,drawingFrames[2].Export());
        }

        public void DeleteSave(int ID) {
            drawingFrames[ID].Reset(Game);
            Program.SaveFiles[ID].Clear();
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
