using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elves.Scenes.Credits {
    public readonly struct CreditsItem {

        public readonly string TextValue { get; private init; }
        public readonly string Label { get; private init; }

        public readonly float Scale { get; private init; }

        public readonly Texture2D Texture { get; private init; }
        public readonly CreditsItemType Type { get; private init; }

        public bool IsHeading => Type == CreditsItemType.TextHeading;
        public bool IsLabel => Type == CreditsItemType.TextLabel;
        public bool IsLink => Type == CreditsItemType.Link;

        public static CreditsItem Image(Texture2D image,float scale = 1) => new() {
            Type = CreditsItemType.Image,Texture = image,Scale = scale
        };

        public static CreditsItem Text(string text) => new() {
            Type = CreditsItemType.Text,TextValue = text
        };

        public static CreditsItem Link(string link) => new() {
            Type = CreditsItemType.Link,TextValue = link
        };

        public static CreditsItem TextLabel(string label,string text) => new() {
            Type = CreditsItemType.TextLabel,TextValue = text,Label = label
        };

        public static CreditsItem Heading(string text) => new() {
            Type = CreditsItemType.TextHeading,TextValue = text
        };

        public static readonly CreditsItem LineBreak = new() {
            Type = CreditsItemType.LineBreak
        };
    }
}
