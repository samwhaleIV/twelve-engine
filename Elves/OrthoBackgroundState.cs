using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Game3D.Entity.Types;

namespace Elves {
    public class OrthoBackgroundState:WorldBase {

        public OrthoBackgroundState(string backgroundImage,bool smoothBackground = true) {
            this.backgroundImage = backgroundImage;
            this.smoothBackground = smoothBackground;
            OnLoad += OrthoBackgroundState_OnLoad;
            OnUpdate += OrthoBackgroundState_OnUpdate;
            Camera.Orthographic = !Debug3D;
        }

        public OrthoBackgroundState(Texture2D backgroundImage,bool smoothBackground = true) {
            backgroundImageTexture = backgroundImage;
            this.smoothBackground = smoothBackground;
            OnLoad += OrthoBackgroundState_OnLoad;
            OnUpdate += OrthoBackgroundState_OnUpdate;
            Camera.Orthographic = !Debug3D;
        }

        private const string BACKGROUND_ENTITY_NAME = "Background";

        public TimeSpan ScrollingBackgroundPeriod { get; set; } = Constants.AnimationTiming.ScrollingBackgroundDefault;
        public bool ScrollingBackground { get; set; } = false;

        private readonly Color[] backgroundColors = new Color[4] { Color.White, Color.White, Color.White, Color.White };

        public void SetBackgroundColor(Color topLeft,Color topRight,Color bottomLeft,Color bottomRight) {
            backgroundColors[0] = topLeft;
            backgroundColors[1] = topRight;
            backgroundColors[2] = bottomLeft;
            backgroundColors[3] = bottomRight;
        }

        public void SetBackgroundColor(Color color) {
            backgroundColors[0] = color;
            backgroundColors[1] = color;
            backgroundColors[2] = color;
            backgroundColors[3] = color;
        }

        public Color[] GetBackgroundColor() {
            Color[] colors = new Color[backgroundColors.Length];
            backgroundColors.CopyTo(colors,0);
            return colors;
        }

        private TextureEntity background;
        protected TextureEntity Background => background;

        private readonly string backgroundImage;
        private readonly bool smoothBackground;
        private readonly Texture2D backgroundImageTexture;

        private void OrthoBackgroundState_OnLoad() {
            if(backgroundImageTexture != null) {
                background = new TextureEntity(backgroundImageTexture);
            } else {
                background = new TextureEntity(backgroundImage);
            }
            background.Name = BACKGROUND_ENTITY_NAME;
            background.PixelSmoothing = smoothBackground;
            background.Scale = new Vector3(1f);
            background.Depth = DepthConstants.Background;
            Entities.Add(background);
        }

        private void OrthoBackgroundState_OnUpdate() {
            background.SetColors(backgroundColors);
            if(ScrollingBackground) {
                double scrollT = Now / ScrollingBackgroundPeriod % 1d;
                background.UVOffset = new Vector2((float)scrollT,0f);
            } else {
                background.UVOffset = Vector2.Zero;
            }
        }
    }
}
