﻿using System;
using TwelveEngine.UI;

namespace Elves.Scenes.SaveSelect {
    public enum ButtonImpulse { None, Back, Play, Accept, Delete };

    public sealed class Button:SpriteElement {

        public ButtonImpulse Impulse { get; set; } = ButtonImpulse.None;

        private const float ROTATION_AMOUNT = 1f;

        public Button(int textureX,int textureY) {
            TextureSource = new(textureX,textureY,16,16);
            Offset = new(-0.5f,-0.5f);
            OnUpdate += Button_OnUpdate;
            Position = new(0.5f);
            PositionMode = CoordinateMode.Relative;
        }

        public bool IsEven { get; set; } = false;

        private void Button_OnUpdate(TimeSpan now) {
            float newScale = 1f;
            float newRotation = 0f;
            
            if(Selected) {
                newScale = 1.05f;
                newRotation = IsEven ? -ROTATION_AMOUNT : ROTATION_AMOUNT;
            }
            if(Pressed) {
                newScale *= 0.95f;
            }
            if(Scale == newScale && Rotation == newRotation) {
                return;
            }
            KeyAnimation(now);
            Scale = newScale;
            Rotation = newRotation;
        }
    }
}
