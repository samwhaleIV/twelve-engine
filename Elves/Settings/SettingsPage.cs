using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using TwelveEngine;
using TwelveEngine.UI.Book;

namespace Elves.Settings {

    public sealed class SettingsPage:BookPage<SpriteElement> {

        public SettingsPage() {
            settingsPhone = new() {
                Scale = 1,
                PositionMode = CoordinateMode.Absolute,
                SizeMode = CoordinateMode.Absolute,
                Offset = Vector2.Zero,
                TextureSource = TotalArea.ToRectangle(),
                Texture = Program.Textures.Settings,
                Depth = Constants.Depth.MiddleClose
            };
        }

        private readonly FloatRectangle TotalArea = new(0,0,131,256);
        private readonly FloatRectangle PhoneArea = new(5,0,72,120);

        private readonly SpriteElement settingsPhone;

        public event Action OnBack;

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
            Update();

            settingsPhone.Scale = 1;
            var newPos = settingsPhone.Position;
            newPos.Y = Viewport.Bottom;
            settingsPhone.Position = newPos;

            settingsPhone.KeyFrame(Now,TransitionDuration);

            return null; //todo return first focus element.
        }

        public float Scale { get; set; } = 0.98f;

        public override void Update() {
            float height = Viewport.Height * Scale;

            Vector2 innerSize = new(height * PhoneArea.AspectRatio,height);
            Vector2 outerSize = innerSize * (TotalArea.Size / PhoneArea.Size);

            Vector2 center = Viewport.Center - innerSize * 0.5f;

            settingsPhone.PositionMode = CoordinateMode.Absolute;
            settingsPhone.Size = outerSize;
            settingsPhone.Position = center;
        }
    }
}
