using Microsoft.Xna.Framework;
using System;
using TwelveEngine;
using TwelveEngine.Effects;
using TwelveEngine.Shell;

namespace Elves.Scenes.SaveSelect {
    public sealed class SaveSelectScene:UIScene {

        private ScrollingBackground background;

        public const int DRAWING_FRAME_WIDTH = 256, DRAWING_FRAME_HEIGHT = 64;

        private readonly DrawingFrame[] drawingFrames = new DrawingFrame[3];
        public DrawingFrame[] DrawingFrames => drawingFrames;

        public void OpenSaveFile(int saveFileID) {
            EndScene(ExitValue.Get(saveFileID));
        }

        public SaveSelectScene() {
            Name = "Save Selection";
            OnLoad += SaveSelectScene_OnLoad;
            OnRender += SaveSelectScene_OnRender;
            OnUpdate += SaveSelectScene_OnUpdate;
            OnUnload += SaveSelectScene_OnUnload;
        }

        private readonly bool[] modifiedSaves = new bool[3] { false, false, false };

        private void SaveSelectScene_OnUnload() {

            bool hasModifiedSave = false;

            for(int i = 0;i<modifiedSaves.Length;i++) {
                if(!modifiedSaves[i]) {
                    continue;
                }
                Program.Saves[i].TrySave();
                hasModifiedSave = true;
            }

            if(!hasModifiedSave) {
                return;
            }

            SaveDrawingFrames();
            Program.GlobalSave.TrySave();
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
            background.Render(SpriteBatch,Viewport);
            UI.Render(SpriteBatch);
        }

        private void LoadDrawingFrames() {
            for(int i = 0;i<drawingFrames.Length;i++) {
                DrawingFrame drawingFrame = new(DRAWING_FRAME_WIDTH,DRAWING_FRAME_HEIGHT) {
                    DrawColor = Color.White,
                    EmptyColor = Color.Transparent,
                    BrushTexture = Program.Textures.CircleBrush,
                    BrushSize = 5
                };
                drawingFrame.Load(GraphicsDevice);
                drawingFrames[i] = drawingFrame;
            }

            for(int i = 0;i<3;i++) {
                if(!Program.GlobalSave.TryGetBytes(SaveKeys.SaveImage1+i,out byte[] data)) {
                    continue;
                }
                drawingFrames[i].Import(data);
            }
        }

        private void SaveDrawingFrames() {
            for(int i = 0;i<3;i++) {
                if(!modifiedSaves[i] || !Program.Saves[i].HasFlag(SaveKeys.DoIExist)) {
                    continue;
                }
                Program.GlobalSave.SetBytes(SaveKeys.SaveImage1+i,drawingFrames[i].Export());
            }
        }

        public void DeleteSave(int ID) {
            modifiedSaves[ID] = true;
            drawingFrames[ID].Reset(this);
            Program.Saves[ID].Clear();
            Program.GlobalSave.RemoveKey(SaveKeys.SaveImage1+ID);
        }

        public void CreateSave(int ID) {
            modifiedSaves[ID] = true;
            Program.Saves[ID].SetFlag(SaveKeys.DoIExist);
        }

        public bool HasSaveFile(int ID) {
            return Program.Saves[ID].HasFlag(SaveKeys.DoIExist);
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
            background.Load(Content);
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

        private readonly Interpolator backgroundScaleAnimator = new();
    }
}
