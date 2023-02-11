using Elves.Scenes.Carousel.UI;
using System;
using TwelveEngine.UI;
using TwelveEngine.UI.Book;

namespace Elves.Settings {
    public sealed class VolumeAdjustmentButton:SpriteElement,IEndpoint<VolumeAdjustment> {
        public VolumeAdjustment Type { get; private init; }

        public VolumeAdjustmentButton(VolumeAdjustment type) {
            Texture = Program.Textures.SettingsPhone;
            Type = type;
            OnUpdate += Update;
            Offset = new(-0.5f,-0.5f);
            TextureSource = Type == VolumeAdjustment.Up ? new(241,1,6,9) : new(248,1,6,9);
            Endpoint = new Endpoint<VolumeAdjustment>(this);
        }

        public event Action<VolumeAdjustment> OnActivated;
        public void FireActivationEvent(VolumeAdjustment value) => OnActivated?.Invoke(value);
        public VolumeAdjustment GetEndPointValue() => Type;

        private void Update(TimeSpan now) {
            float newScale = 1f;
            if(Selected) {
                newScale = 1.05f;
            }
            if(Pressed) {
                newScale *= 0.95f;
            }
            if(Scale == newScale) {
                return;
            }
            KeyFrame(now);
            Scale = newScale;
        }
    }
}
