using TwelveEngine.UI.Book;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine;
using System;
using TwelveEngine.UI;
using TwelveEngine.Effects;

namespace Elves.Scenes.SaveSelect {

    public enum TagDisplay { Empty, Create, Custom, Delete }

    public sealed class Tag:SpriteElement, IEndpoint<Tag> {

        public Tag GetEndPointValue() => this;
        public void FireActivationEvent(Tag value) => OnActivated?.Invoke(value);

        private static readonly Dictionary<TagDisplay,Rectangle> textureSources = new() {
            { TagDisplay.Empty, new(1,50,128,32) },
            { TagDisplay.Create, new(52,126,128,32) },
            { TagDisplay.Custom, new(1,90,128,32) },
            { TagDisplay.Delete, new(52,160,128,32) }
        };

        private TagDisplay _display = TagDisplay.Empty;

        public TagDisplay Display {
            get => _display;
            set {
                if(_display == value) {
                    return;
                }
                _display = value;
                TextureSource = textureSources[value];
            }
        }

        public DrawingFrame DrawingFrame { get; set; } = null;

        public event Action<Tag> OnActivated;

        public Tag(bool tagHasSaveFile) {
            Display = tagHasSaveFile ? TagDisplay.Custom : TagDisplay.Empty;
            TextureSource = textureSources[Display];

            Offset = new(-0.5f,-0.5f);

            OnUpdate += Tag_OnUpdate;

            PositionMode = CoordinateMode.Relative;

            OnRender += Tag_OnRender;

            Endpoint = new Endpoint<Tag>(this);
        }

        private readonly Interpolator blipAnimator = new(TimeSpan.FromMilliseconds(175));

        private void Tag_OnUpdate(TimeSpan now) {
            blipAnimator.Update(now);
            float newScale = 1f;
            newScale += MathF.Sin(blipAnimator.Value * MathF.PI) * 0.05f;
            if(Selected) {
                newScale = 1.05f;
            }
            if(Pressed) {
                newScale *= 0.95f;
            }
            if(Scale == newScale) {
                return;
            }
            if(blipAnimator.IsFinished) {
                KeyAnimation(now);
            }
            Scale = newScale;
        }

        public void Blip(TimeSpan now) {
            blipAnimator.Reset(now);
        }

        public void Tag_OnRender(SpriteBatch spriteBatch) {
            if(Display != TagDisplay.Custom || DrawingFrame is null) {
                return;
            }
            var renderTarget = DrawingFrame.RenderTarget;
            Draw(spriteBatch,renderTarget,renderTarget.Bounds);
        }

        public int ID { get; set; } = -1;
    }
}
