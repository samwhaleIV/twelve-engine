using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Input;

namespace Elves.Scenes.SaveSelect {
    public sealed class SaveSelectScene:Scene {

        private ScrollingBackground background;

        public const float Row1 = 1 / 5f;
        public const float Row2 = 2 / 4f;
        public const float Row3 = 4 / 5f;

        public static readonly float[] SelectionRows = new float[] { Row1, Row2, Row3 };

        private static readonly Vector3 DefaultSaveTagRotation = new(0,0,-2.25f);

        public SaveSelectScene() {
            OnLoad += SaveSelectScene_OnLoad;
            OnPreRender += SaveSelectScene_OnPreRender;
            OnUnload += SaveSelectScene_OnUnload;
            Camera.Orthographic = !Debug;
            Input.OnDirectionDown += Input_OnDirectionDown;
        }

        private readonly List<SaveTag> tags = new();

        private SaveTag SaveTag1 => tags[0];
        private SaveTag SaveTag2 => tags[1];
        private SaveTag SaveTag3 => tags[2];

        private SelectionFinger finger;

        private void UpdateSaveTagPositions() {
            var bounds = Game.Viewport.Bounds.Size.ToVector2();
            float twoThirdsX = bounds.X * (2 / 3f);
            float scale = (bounds.Y / SaveTag.BaseSize.Y) * 0.26f;

            float horizontalStep = scale * SaveTag.BaseSize.Y * 0.11f;

            SaveTag1.Origin = new(twoThirdsX+horizontalStep,bounds.Y * Row1);
            SaveTag2.Origin = new(twoThirdsX,bounds.Y * Row2);
            SaveTag3.Origin = new(twoThirdsX-horizontalStep,bounds.Y * Row3);

            SaveTag1.TagScale = scale;
            SaveTag2.TagScale = scale;
            SaveTag3.TagScale = scale;
        }

        protected override void UpdateGame() {
            UpdateSaveTagPositions();
            UpdateInputs();
            UpdateCameraScreenSize();
            Entities.Update();
            UpdateCamera();
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
                var saveTag = new SaveTag(i+1,Program.Textures.SaveSelect);
                tags.Add(saveTag);
                Entities.Add(saveTag);
                saveTag.Rotation = DefaultSaveTagRotation;
            }
            finger = new SelectionFinger(Program.Textures.SaveSelect);
            finger.SetTo(SelectionRows[selectionRow]);
            Entities.Add(finger);
        }
    }
}
