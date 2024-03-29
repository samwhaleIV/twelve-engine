﻿using Elves.Settings;
using Microsoft.Xna.Framework;
using System;
using TwelveEngine;
using TwelveEngine.UI;
using TwelveEngine.UI.Book;

namespace Elves.Scenes.Carousel.UI {
    public sealed class Button:SpriteElement, IEndpoint<ButtonAction>, ISettingsBackButton {

        public ButtonAction GetEndPointValue() => Action;

        public void FireActivationEvent(ButtonAction value) {
            OnActivated?.Invoke(value);
            if(value != ButtonAction.Back) {
                return;
            }
            OnBackAction?.Invoke();
        }

        public event Action<ButtonAction> OnActivated;
        public event Action OnBackAction;

        public ButtonAction Action { get; init; }

        public Button(Rectangle textureSource) {
            TextureSource = textureSource;
            Offset = new(-0.5f,-0.5f);
            OnUpdate += Update;
            Position = new(0.5f);
            PositionMode = CoordinateMode.Relative;
            Endpoint = new Endpoint<ButtonAction>(this);
        }

        private void Update(TimeSpan now) {
            float newScale = 1f;
            float newRotation = 0f;
            if(Selected) {
                newScale = 1.05f;
            }
            if(Pressed) {
                newScale *= 0.95f;
            }
            if(Scale == newScale && Rotation == newRotation) {
                return;
            }
            KeyFrame(now);
            Scale = newScale;
            Rotation = newRotation;
        }
    }
}
