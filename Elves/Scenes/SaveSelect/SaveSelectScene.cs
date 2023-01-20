using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine;
using Microsoft.Xna.Framework;
using TwelveEngine.Input;
using TwelveEngine.Shell;
using Elves.Scenes.Test;
using Elves.Scenes.Carousel;

namespace Elves.Scenes.SaveSelect {
    public sealed class SaveSelectScene:Scene {

        public const float ROW_1 = 1 / 5f, ROW_2 = 2 / 4f, ROW_3 = 4 / 5f;
        public const float FOCUS_BACKGROUND_SCALE = 0.15f, TAG_FOCUS_SCALE = 0.35f;
        public static readonly float[] SelectionRows = new float[] { ROW_1, ROW_2, ROW_3 };

        private readonly List<SaveTag> tags = new();
        private readonly AnimationInterpolator saveTagFocusAnimator = new(Constants.AnimationTiming.SaveTagFocus);
        private readonly TagContextPage tagContextPage = new(Program.Textures.SaveSelect);

        private ScrollingBackground background;
        private SaveTag capturedTag = null;

        private SelectionFinger selectionFinger;

        private CursorState cursorState = CursorState.Default;
        private SaveTag focusedTag = null;
        private bool focusingIn = false;

        private bool focusedFromMouse = false;

        public SaveSelectScene() {
            OnLoad += SaveSelectScene_OnLoad;
            OnPreRender += SaveSelectScene_OnPreRender;
            OnRender += SaveSelectScene_OnRender;

            Mouse.OnMove += Mouse_OnMove;
            Mouse.OnPress += Mouse_OnPress;
            Mouse.OnRelease += Mouse_OnRelease;

            Input.OnDirectionDown += Input_OnDirectionDown;
            Input.OnAcceptDown += Input_OnAcceptDown;
            Input.OnAcceptUp += Input_OnAcceptUp;
            Input.OnCancelDown += Input_OnCancelDown;

            Camera.Orthographic = true;
            tagContextPage.OnButtonPressed +=TagContextPage_OnButtonPressed;
        }

        private float GetFocusInDelta() {
            if(focusingIn) {
                return saveTagFocusAnimator.Value;
            } else {
                return 1 - saveTagFocusAnimator.Value;
            }
        }

        private void UpdateTagButtons() {
            tagContextPage.ClearButtons();
            switch(focusedTag.State) {
                case TagState.NoSave:
                    throw new InvalidOperationException();
                case TagState.Delete:
                    tagContextPage.AddButton(SaveButtonType.Back);
                    tagContextPage.AddButton(SaveButtonType.Yes);
                    break;
                case TagState.CreateNew:
                    tagContextPage.AddButton(SaveButtonType.Back);
                    tagContextPage.AddButton(SaveButtonType.Yes);
                    break;
                case TagState.Customized:
                    tagContextPage.AddButton(SaveButtonType.Back);
                    tagContextPage.AddButton(SaveButtonType.Play);
                    tagContextPage.AddButton(SaveButtonType.Delete);
                    break;
            }
        }

        private void FocusSelectionTag() {
            if(focusingIn) {
                return;
            }
            capturedTag = null;
            focusedTag = tags[selectedRow];
            if(focusedTag.State == TagState.NoSave) {
                focusedTag.State = TagState.CreateNew;
            }
            focusingIn = true;
            UpdateTagButtons();
            saveTagFocusAnimator.ResetCarryOver(Now);
        }

        private void TagContextPage_OnButtonPressed(int index) {
            switch(focusedTag.State) {
                case TagState.NoSave:
                    throw new InvalidOperationException();
                case TagState.Delete:
                    tagContextPage.CanExit = true;
                    if(index != 1) {
                        focusedTag.State = TagState.Customized;
                    } else {
                        focusedTag.State = TagState.NoSave;
                        //todo: delete actual save data
                        ExitSelectionTag();
                    }
                    break;
                case TagState.CreateNew:
                    if(index == 0) { //back
                        ExitSelectionTag();
                    } else if(index == 1) { //create
                        focusedTag.State = TagState.Customized;
                        UpdateTagButtons();
                    }
                    break;
                case TagState.Customized:
                    if(index == 0) { //back
                        ExitSelectionTag();
                    } else if(index == 1) { //play
                        //todo: transition properly
                        TransitionOut(new TransitionData() {
                            Generator = () => new CarouselMenu(),
                            Duration = Constants.AnimationTiming.TransitionDuration,
                            Data = new StateData() { Flags = StateFlags.FadeIn }
                        });
                    } else if(index == 2) { //delete
                        tagContextPage.CanExit = false;
                        focusedTag.State = TagState.Delete;
                        UpdateTagButtons();
                    }
                    break;
            }
            Console.WriteLine($"Tag button pressed: {index}, save index {selectedRow}");
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
            focusedFromMouse = false;
        }

        private int selectedRow = 0;
        public int SelectionRow {
            get => selectedRow;
            private set {
                if(selectedRow == value || value < 0 || value >= SelectionRows.Length) {
                    return;
                }
                tags[selectedRow].ShiftRight();
                tags[value].ShiftLeft();
                selectedRow = value;
                selectionFinger.AnimateTo(SelectionRows[value]);
            }
        }

        #region UI LAYOUT
        private void UpdateSaveActionButtons() {
            float height = 0f;

            if(focusedTag is not null) {
                height = focusedTag.RenderScale * SaveTag.BaseSize.Y;
            }

            tagContextPage.Scale = GetFocusInDelta();
            tagContextPage.Update(height,Game.Viewport.Bounds);
        }
        private void UpdateUIPositions() {
            var bounds = Game.Viewport.Bounds.Size.ToVector2();
            float twoThirdsX = bounds.X * (2 / 3f);
            float scale = bounds.Y / SaveTag.BaseSize.Y * 0.26f;

            float horizontalStep = scale * SaveTag.BaseSize.Y * 0.11f;

            tags[0].Origin = new(twoThirdsX+horizontalStep,bounds.Y * ROW_1);
            tags[1].Origin = new(twoThirdsX,bounds.Y * ROW_2);
            tags[2].Origin = new(twoThirdsX-horizontalStep,bounds.Y * ROW_3);

            saveTagFocusAnimator.Update(Now);

            var focusT = GetFocusInDelta();

            background.Scale = 1 + focusT * FOCUS_BACKGROUND_SCALE;
            selectionFinger.OffscreenDelta = focusT;

            var tagScale = scale * (1 - focusT);

            for(int i = 0;i<tags.Count;i++) {
                var tag = tags[i];
                tag.LocalAnimationStrength = 1 - focusT;

                if(tag == focusedTag) {
                    tag.Origin = Vector2.Lerp(tag.Origin,new(bounds.X*0.5f,bounds.Y*(2/7f)),focusT);
                    tag.RenderScale = MathHelper.Lerp(scale,(bounds.Y / SaveTag.BaseSize.Y) * TAG_FOCUS_SCALE,focusT);
                    tag.Depth = Constants.Depth.Middle;
                } else {
                    tag.RenderScale = tagScale;
                    tag.Depth = Constants.Depth.MiddleFar;
                }

                tag.IsVisible = tag.RenderScale > 0;
            }
        }
        #endregion

        #region UPDATE LOOP
        protected override void UpdateGame() {
            cursorState = focusingIn ? tagContextPage.CursorState : CursorState.Default;
            UpdateUIPositions();
            tagContextPage.Now = Now;
            UpdateSaveActionButtons();
            if(focusingIn && GetFocusInDelta() >= 1 && focusedFromMouse) {
                tagContextPage.UpdateMouse(Mouse.Position,setSelection: true);
                focusedFromMouse = false;
            }
            UpdateInputs();
            UpdateMouse(Mouse.Position);
            UpdateCameraScreenSize();
            Entities.Update();
            UpdateCamera();
            Game.CursorState = cursorState;
        }

        private void SaveSelectScene_OnPreRender() {
            background.Update(Now);
            background.Render(Game.SpriteBatch,Game.Viewport);
        }

        private void SaveSelectScene_OnRender() {
            tagContextPage.Render(Game.SpriteBatch);
        }
        #endregion

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
            selectionFinger = new SelectionFinger(Program.Textures.SaveSelect);
            selectionFinger.SetTo(SelectionRows[selectedRow]);
            selectionFinger.Depth = Constants.Depth.MiddleClose;
            Entities.Add(selectionFinger);
        }

        #region KEYBOARD INPUT
        private void Input_OnCancelDown() {
            if(capturedTag is not null) {
                return;
            }
            if(tagContextPage.CanExit) {
                ExitSelectionTag();
            }
        }

        private void Input_OnAcceptDown() {
            if(!focusingIn) {
                if(capturedTag is not null) {
                    return;
                }
                FocusSelectionTag();
                tagContextPage.DirectionImpulse(0);
            } else {
                tagContextPage.AcceptDown();
            }
        }

        private void Input_OnAcceptUp() {
            if(focusingIn) {
                tagContextPage.AcceptUp();
            }
        }

        private void Input_OnDirectionDown(Direction direction) {
            if(!focusingIn) {
                if(capturedTag is not null || !saveTagFocusAnimator.IsFinished) {
                    return;
                }
                SelectionRow += GetUIDirection(direction);
            } else {
                if(!saveTagFocusAnimator.IsFinished) {
                    return;
                }
                tagContextPage.DirectionImpulse(GetUIDirection(direction));
            }
        }
        #endregion

        #region MOUSE

        private SaveTag GetTagAtMouse(Point location) {
            foreach(var tag in tags) {
                if(!tag.Contains(location)) {
                    continue;
                }
                return tag;
            }
            return null;
        }

        private void UpdateMouseUnfocused(Point location,bool setSelection) {
            var tag = GetTagAtMouse(location);
            if(tag == null) {
                return;
            }
            if(capturedTag is not null) {
                cursorState = capturedTag == tag ? CursorState.Pressed : CursorState.Default;
            } else {
                if(setSelection) {
                    SelectionRow = tag.ID;
                }
                cursorState = CursorState.Interact;
            }
        }

        private void UpdateMouse(Point location) {
            if(focusingIn) {
                tagContextPage.UpdateMouse(location,setSelection: false);
            } else {
                UpdateMouseUnfocused(location,setSelection: false);
            }
        }

        private void Mouse_OnMove(Point location) {
            if(focusingIn) {
                tagContextPage.MouseMove(location);
            } else {
                UpdateMouseUnfocused(location,setSelection: true);
            }
        }

        private void Mouse_OnPress(Point location) {
            if(focusingIn) {
                tagContextPage.MouseDown(location);
            } else {
                UpdateMouseUnfocused(location,setSelection: true);
                if(cursorState == CursorState.Interact) {
                    capturedTag = tags[selectedRow];
                }
            }
        }

        private void Mouse_OnRelease(Point location) {
            if(focusingIn) {
                tagContextPage.UpdateMouse(location,setSelection: true);
                tagContextPage.MouseUp(location);
            } else {
                if(capturedTag is not null && GetTagAtMouse(location) == capturedTag) {
                    FocusSelectionTag();
                    focusedFromMouse = true;
                }
                capturedTag = null;
                UpdateMouseUnfocused(location,setSelection: true);
            }
        }
        #endregion
    }
}
