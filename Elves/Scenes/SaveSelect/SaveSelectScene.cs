using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Input;
using TwelveEngine.Shell;

namespace Elves.Scenes.SaveSelect {
    public sealed class SaveSelectScene:Scene {

        private ScrollingBackground background;

        public const float Row1 = 1 / 5f;
        public const float Row2 = 2 / 4f;
        public const float Row3 = 4 / 5f;

        public const float FOCUS_BACKGROUND_SCALE = 0.5f;
        public const float FOCUS_SCALE_COEFFICIENT = 0.35f;

        public static readonly float[] SelectionRows = new float[] { Row1, Row2, Row3 };

        private SaveTag capturingTag = null;

        public SaveSelectScene() {
            OnLoad += SaveSelectScene_OnLoad;
            OnPreRender += SaveSelectScene_OnPreRender;
            OnUnload += SaveSelectScene_OnUnload;
            Camera.Orthographic = !Debug;
            Input.OnDirectionDown += Input_OnDirectionDown;
            Input.OnAcceptDown += Input_OnAcceptDown;
            Input.OnCancelDown += Input_OnCancelDown;
            OnRender += SaveSelectScene_OnRender;
            Mouse.OnMove += Mouse_OnMove;
            Mouse.OnPress += Mouse_OnPress;
            Mouse.OnRelease += Mouse_OnRelease;
        }

        private void SaveSelectScene_OnRender() {
            saveActionButtons.Render(Game.SpriteBatch);
        }

        private void UpdateSaveActionButtons() {
            float height = 0f;

            if(focusedTag is not null) {
                height = focusedTag.RenderScale * SaveTag.BaseSize.Y;
            }

            saveActionButtons.Scale = GetFocusT();
            saveActionButtons.Update(height,Game.Viewport.Bounds);
        }

        private void Input_OnCancelDown() {
            ExitSelectionTag();
        }

        private void Input_OnAcceptDown() {
            FocusSelectionTag();
        }

        private readonly List<SaveTag> tags = new();

        private SaveTag SaveTag1 => tags[0];
        private SaveTag SaveTag2 => tags[1];
        private SaveTag SaveTag3 => tags[2];

        private SelectionFinger finger;

        private readonly AnimationInterpolator saveTagFocusAnimator = new(Constants.AnimationTiming.SaveTagFocus);

        private void UpdateUIPositions() {
            var bounds = Game.Viewport.Bounds.Size.ToVector2();
            float twoThirdsX = bounds.X * (2 / 3f);
            float scale = bounds.Y / SaveTag.BaseSize.Y * 0.26f;

            float horizontalStep = scale * SaveTag.BaseSize.Y * 0.11f;

            SaveTag1.Origin = new(twoThirdsX+horizontalStep,bounds.Y * Row1);
            SaveTag2.Origin = new(twoThirdsX,bounds.Y * Row2);
            SaveTag3.Origin = new(twoThirdsX-horizontalStep,bounds.Y * Row3);

            saveTagFocusAnimator.Update(Now);

            var focusT = GetFocusT();

            background.Scale = 1 + focusT * FOCUS_BACKGROUND_SCALE;
            finger.OffscreenDelta = focusT;

            var tagScale = scale * (1 - focusT);

            for(int i = 0;i<tags.Count;i++) {
                var tag = tags[i];
                tag.LocalAnimationStrength = 1 - focusT;

                if(tag == focusedTag) {
                    tag.Origin = Vector2.Lerp(tag.Origin,new(bounds.X*0.5f,bounds.Y*(2/7f)),focusT);
                    tag.RenderScale = MathHelper.Lerp(scale,(bounds.Y / SaveTag.BaseSize.Y) * FOCUS_SCALE_COEFFICIENT,focusT);
                    tag.Depth = Constants.Depth.Middle;
                } else {
                    tag.RenderScale = tagScale;
                    tag.Depth = Constants.Depth.MiddleFar;
                }

                tag.IsVisible = tag.RenderScale > 0;
            }
        }

        private readonly SaveActionButtonSet saveActionButtons = new(Program.Textures.SaveSelect);

        private CursorState cursorState = CursorState.Default;

        protected override void UpdateGame() {
            cursorState = CursorState.Default;
            UpdateUIPositions();
            UpdateSaveActionButtons();
            UpdateInputs();
            UpdateMouse(Mouse.Position);
            UpdateCameraScreenSize();
            Entities.Update();
            UpdateCamera();
            Game.CursorState = cursorState;
        }

        private SaveTag focusedTag = null;
        private bool focusingIn;

        private float GetFocusT() {
            if(focusingIn) {
                return saveTagFocusAnimator.Value;
            } else {
                return 1 - saveTagFocusAnimator.Value;
            }
        }

        private void CreateSaveActionButtons() {
            saveActionButtons.ClearButtons();
            switch(focusedTag.State) {
                case TagState.NoSave:
                case TagState.Delete:
                    throw new InvalidOperationException("Tags should not be focused into with NoSave or Delete states.");
                case TagState.CreateNew:
                    saveActionButtons.AddButton(SaveButtonType.Back);
                    saveActionButtons.AddButton(SaveButtonType.Yes);
                    break;
                case TagState.Customized:
                    saveActionButtons.AddButton(SaveButtonType.Back);
                    saveActionButtons.AddButton(SaveButtonType.Yes);
                    saveActionButtons.AddButton(SaveButtonType.Delete);
                    break;
            }
        }

        private void FocusSelectionTag() {
            if(focusingIn) {
                return;
            }
            focusedTag = tags[selectionRow];
            if(focusedTag.State == TagState.NoSave) {
                focusedTag.State = TagState.CreateNew;
            }
            focusingIn = true;
            CreateSaveActionButtons();
            saveTagFocusAnimator.ResetCarryOver(Now);
        }

        private void ExitSelectionTag() {
            if(!focusingIn) {
                return;
            }
            if(focusedTag is not null && focusedTag.State == TagState.CreateNew) {
                focusedTag.State = TagState.NoSave;
            }
            focusingIn = false;
            saveTagFocusAnimator.ResetCarryOver(Now);
        }

        private int selectionRow = 0;
        public int SelectionRow {
            get => selectionRow;
            private set {
                if(selectionRow == value || value < 0 || value >= SelectionRows.Length) {
                    return;
                }
                tags[selectionRow].ShiftRight();
                tags[value].ShiftLeft();
                selectionRow = value;
                finger.AnimateTo(SelectionRows[value]);
            }
        }

        private void Input_OnDirectionDown(Direction direction) {
            if(capturingTag is not null || focusingIn || !saveTagFocusAnimator.IsFinished) {
                return;
            }
            SelectionRow += GetUIDirection(direction);
        }

        private void SaveSelectScene_OnUnload() {
            background?.Unload();
            background = null;
        }

        private void SaveSelectScene_OnPreRender() {
            background.Update(Now);
            background.Render(Game.SpriteBatch,Game.Viewport);
        }

        private SaveTag GetTagAtMouse() {
            foreach(var tag in tags) {
                if(tag.Contains(Mouse.Position)) {
                    return tag;
                }
            }
            return null;
        }

        private void UpdateMouse(Point location,bool setSelection = false) {
            if(!saveTagFocusAnimator.IsFinished) {
                return;
            }
            if(focusingIn) {
                //todo
            } else {
                var tag = GetTagAtMouse();
                if(tag == null) {
                    return;
                }
                if(capturingTag is not null) {
                    cursorState = capturingTag == tag ? CursorState.Pressed : CursorState.Default;
                } else {
                    if(setSelection) {
                        SelectionRow = tag.ID;
                    }
                    cursorState = CursorState.Interact;
                }
            }
        }

        private void Mouse_OnMove(Point location) {
            UpdateMouse(location,setSelection: true);
        }

        private void Mouse_OnPress(Point location) {
            UpdateMouse(location,setSelection: true);
            if(cursorState == CursorState.Interact) {
                capturingTag = tags[selectionRow];
            }
        }

        private void Mouse_OnRelease(Point location) {
            if(capturingTag is not null && GetTagAtMouse() == capturingTag) {
                FocusSelectionTag();
            }
            capturingTag = null;
            UpdateMouse(location,setSelection: true);
        }

        private void SaveSelectScene_OnLoad() {
            background = new ScrollingBackground() {
                ScrollTime = TimeSpan.FromSeconds(160f),
                Scale = 1.25f,
                Rotation = -15f,
                Texture = Program.Textures.GiftPattern,
                Direction = new(1,0.27f),
            };
            background.Load(Game.Content);
            for(int i = 0;i<3;i++) {
                var saveTag = new SaveTag(i,Program.Textures.SaveSelect);
                tags.Add(saveTag);
                Entities.Add(saveTag);
            }
            finger = new SelectionFinger(Program.Textures.SaveSelect);
            finger.SetTo(SelectionRows[selectionRow]);
            finger.Depth = Constants.Depth.MiddleClose;
            Entities.Add(finger);
        }
    }
}
