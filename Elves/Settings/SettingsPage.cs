using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using TwelveEngine;
using TwelveEngine.UI.Book;

namespace Elves.Settings {

    public sealed class SettingsPage:BookPage<SpriteElement> {

        public event Action OnUpdate, OnBack;

        private readonly SpriteElement backButton;

        public SettingsPage(SpriteElement backButton) {
            this.backButton = backButton;

            settingsPhone = new() {
                Scale = 1,
                SmoothStepAnimation = true,
                PositionMode = CoordinateMode.Absolute,
                SizeMode = CoordinateMode.Absolute,
                Offset = Vector2.Zero,
                TextureSource = TotalArea.ToRectangle(),
                Texture = Program.Textures.Settings,
                Depth = Constants.Depth.MiddleClose
            };
        }

        private readonly FloatRectangle TotalArea = new(0,0,131,256), PhoneArea = new(5,0,72,120);

        private readonly SpriteElement settingsPhone;

        public override bool Back() {
            if(OnBack is null) {
                return false;
            }
            OnBack.Invoke();
            return true;
        }

        public IEnumerable<SpriteElement> GetElements() {
            yield return settingsPhone;
        }

        public override void Close() {
            settingsPhone.KeyFrame(Now,TransitionDuration);

            settingsPhone.Scale = 1;
            var newPos = settingsPhone.Position;
            newPos.Y = Viewport.Bottom;
            settingsPhone.Position = newPos;
        }

        public override BookElement Open() {
            backButton.Scale = 1;
            backButton.PositionModeX = CoordinateMode.Absolute;
            backButton.PositionModeY = CoordinateMode.Relative;
            backButton.SizeMode = CoordinateMode.Absolute;
            backButton.Offset = new(-0.5f,-0.5f);
            backButton.Flags = ElementFlags.UpdateAndInteract;
            Update();

            settingsPhone.Scale = 1;
            var newPos = settingsPhone.Position;
            newPos.Y = Viewport.Bottom;
            settingsPhone.Position = newPos;

            settingsPhone.KeyFrame(Now,TransitionDuration);

            return backButton; //todo return first focus element.
        }

        public float Scale { get; set; } = 0.98f;

        public override void Update() {
            float height = Viewport.Height * Scale;

            Vector2 innerSize = new(height * PhoneArea.AspectRatio,height);
            Vector2 outerSize = innerSize * (TotalArea.Size / PhoneArea.Size);

            float pixelSize = innerSize.Y / PhoneArea.Height;

            float settingsX = Viewport.Center.X - innerSize.X * 0.5f;
            settingsX -= PhoneArea.X * pixelSize;
            float settingsY = Viewport.Center.Y - innerSize.Y * 0.5f;

            Vector2 settingsPosition = new(settingsX,settingsY);

            settingsPhone.PositionMode = CoordinateMode.Absolute;
            settingsPhone.Size = outerSize;
            settingsPhone.Position = settingsPosition;

            backButton.SizeByPixels(pixelSize);
            float backButtonX = Viewport.Center.X - innerSize.X * 0.5f - backButton.Size.X * 0.5f - pixelSize * 2;
            backButton.Position = new Vector2(backButtonX,0.5f);
            OnUpdate?.Invoke();
        }
    }
}
