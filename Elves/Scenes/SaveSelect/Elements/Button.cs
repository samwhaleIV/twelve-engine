using System;
using TwelveEngine.UI;
using TwelveEngine.UI.Book;

namespace Elves.Scenes.SaveSelect {
    public enum ButtonImpulse { None, Back, Play, Accept, Delete };

    public sealed class Button:SpriteElement, IEndpoint<ButtonImpulse> {

        public ButtonImpulse GetEndPointValue() => Impulse;
        public void FireActivationEvent(ButtonImpulse value) => OnActivated?.Invoke(value);
        public event Action<ButtonImpulse> OnActivated;

        public ButtonImpulse Impulse { get; set; } = ButtonImpulse.None;

        private const float ROTATION_AMOUNT = 1f;

        public Button(int textureX,int textureY) {
            TextureSource = new(textureX,textureY,16,16);
            Offset = new(-0.5f,-0.5f);
            OnUpdate += Update;
            Position = new(0.5f);
            PositionMode = CoordinateMode.Relative;

            Endpoint = new Endpoint<ButtonImpulse>(this);
        }

        public bool IsEven { get; set; } = false;

        private void Update(TimeSpan now) {
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
            KeyFrame(now);
            Scale = newScale;
            Rotation = newRotation;
        }
    }
}
