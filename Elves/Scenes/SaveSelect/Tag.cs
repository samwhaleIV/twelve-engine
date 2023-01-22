using Elves.UI.SpriteUI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Elves.Scenes.SaveSelect {

    public enum TagDisplay { None, Empty, Create, Custom, Delete }

    public sealed class Tag:SpriteElement {

        private static readonly Dictionary<TagDisplay,Rectangle> textureSources = new() {
            { TagDisplay.None, Rectangle.Empty },
            { TagDisplay.Empty, new(1,50,128,32) },
            { TagDisplay.Create, new(52,126,128,32) },
            { TagDisplay.Custom, new(1,90,128,32) },
            { TagDisplay.Delete, new(52,160,128,32) }
        };

        private TagDisplay _display = TagDisplay.None;

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

        public Tag() {
            Display = TagDisplay.Empty;
            Offset = new(-0.5f,-0.5f);

            OnUpdate += UpdateScaleForInteraction;

            PositionModeX = UI.CoordinateMode.Relative;
            PositionModeY = UI.CoordinateMode.Relative;
        }

        public int ID { get; set; } = -1;
    }
}
