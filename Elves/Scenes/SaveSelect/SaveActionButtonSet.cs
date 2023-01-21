using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TwelveEngine;
using TwelveEngine.Shell;

namespace Elves.Scenes.SaveSelect {

    public sealed class SaveActionButtonSet {
        private readonly Texture2D texture;

        public readonly List<SaveActionButton> Buttons = new();

        public SaveActionButtonSet(Texture2D texture) {
            this.texture = texture;
        }

        public TimeSpan Now { get; set; } = TimeSpan.Zero;

        public void ClearButtons() {
            Buttons.Clear();
            SelectedButton = null;
            CanExit = true;
        }

        public int AddButton(SaveButtonType type) {
            int index = Buttons.Count;
            SaveActionButton button = new() {
                Texture = texture,
                Type = type,
                IsEvenNumbered = index % 2 == 0
            };
            Buttons.Add(button);
            return index;
        }

        public void Update(float height,Rectangle targetArea) {
            CursorState = CursorState.Default;
            if(Buttons.Count < 1) {
                return;
            }
            VectorRectangle bounds = new(targetArea);

            float centerX = bounds.Width * 0.5f + height * 0.5f;
            float buttonY = bounds.Height * (5 / 7f);
            float centerOffset = height * 0.05f;
            float totalWidth = height * Buttons.Count + centerOffset * (Buttons.Count - 1);
            float xStart = centerX - totalWidth * 0.5f;

            for(int i = 0;i<Buttons.Count;i++) {
                var button = Buttons[i];
                button.Scale = Scale;
                button.Update(Now,new Vector2(xStart + (centerOffset + height) * i,buttonY),height);
            }
        }

        public float Scale { get; set; } = 1f;

        public void Render(SpriteBatch spriteBatch) {
            if(Buttons.Count < 1) {
                return;
            }
            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            foreach(var button in Buttons) {
                button.Render(spriteBatch);
            }
            spriteBatch.End();
        }

        private SaveActionButton _selectedButton;
        public SaveActionButton SelectedButton {
            get => _selectedButton;
            set {
                if(_selectedButton == value) {
                    return;
                }
                if(_selectedButton is not null) {
                    _selectedButton.SetUnHover(Now);
                }
                if(value is not null) {
                    value.SetHover(Now);
                }
                _selectedButton = value;
            }
        }

        private SaveActionButton _capturedButton;
        public SaveActionButton CapturedButton {
            get => _capturedButton;
            set {
                if(_capturedButton == value) {
                    return;
                }
                if(_capturedButton is not null) {
                    _capturedButton.SetUnPress(Now);
                }
                if(value is not null) {
                    value.SetPress(Now);
                }
                _capturedButton = value;
            }
        }

        //public event Action ButtonPressed;

        public CursorState CursorState { get; set; }

        private SaveActionButton GetButtonAtMouse(Point location) {
            foreach(var button in Buttons) {
                if(!button.HitTestArea.Contains(location)) {
                    continue;
                }
                return button;
            }
            return null;
        }

        public void MouseMove(Point location) {
            UpdateMouse(location,setSelection: true);
        }

        public void MouseDown(Point location) {
            UpdateMouse(location,setSelection: true);
            CapturedButton = SelectedButton;
        }

        public void MouseUp(Point location) {
            UpdateMouse(location,setSelection: true);
            CapturedButton = null;
        }

        public bool CanExit { get; set; } = false;

        public void UpdateMouse(Point location,bool setSelection) {
            var button = GetButtonAtMouse(location);
            if(button == null) {
                if(CapturedButton is null && setSelection) {
                    SelectedButton = null;
                }
                return;
            }
            if(CapturedButton is not null) {
                if(CapturedButton == button) {
                    CursorState = CursorState.Pressed;
                } else {
                    CursorState = CursorState.Default;
                }
            } else {
                if(setSelection) {
                    SelectedButton = button;
                }
                CursorState = CursorState.Interact;
            }
        }

        public void Accept() {

        }

        public void DirectionImpulse(int direction) {

        }
    }
}
