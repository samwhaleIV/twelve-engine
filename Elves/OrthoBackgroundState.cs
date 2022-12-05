using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Game3D;
using TwelveEngine.Game3D.Entity.Types;
using Elves.Battle.Sprite.Elves;
using Elves.UI;
using System.Text;
using System;
using Elves.UI.Font;
using System.Reflection.Metadata;

namespace Elves {
    public class OrthoBackgroundState:World {

        private const int MAX_UI_SCALE = 8;
        private const string BACKGROUND_ENTITY_NAME = "Background";

        public double ScrollingBackgroundPeriod { get; set; } = 60d;
        public bool ScrollingBackground { get; set; } = false;

        private readonly Color[] backgroundColors = new Color[4] { Color.White, Color.White, Color.White, Color.White };

        public void SetBackgroundColors(Color topLeft,Color topRight,Color bottomLeft,Color bottomRight) {
            backgroundColors[0] = topLeft;
            backgroundColors[1] = topRight;
            backgroundColors[2] = bottomLeft;
            backgroundColors[3] = bottomRight;
        }

        public Color[] GetBackgroundColors() {
            Color[] colors = new Color[backgroundColors.Length];
            backgroundColors.CopyTo(colors,0);
            return colors;
        }

        public int GetUIScale() {
            int screenHeight = Game.Viewport.Height;
            /* 2160 (4K) / 8 = 270 */
            return Math.Min(Math.Max(screenHeight / 270 - 2,1),MAX_UI_SCALE);
        }

        private TextureEntity background;
        protected TextureEntity Background => background;

        private void Initialize() {
            ClearColor = Color.Black;
            SetOrthoCamera();
            OnUpdate += UpdateBackground;
            OnRender += RenderEntities;
            OnPreRender += PreRenderEntities;
        }

        private readonly string backgroundImage;
        private readonly bool smoothBackground;
        private readonly Texture2D backgroundImageTexture;

        public OrthoBackgroundState(string backgroundImage,bool smoothBackground = true) {
            this.backgroundImage = backgroundImage;
            this.smoothBackground = smoothBackground;
            OnLoad += OrthoBackgroundState_OnLoad;
            Initialize();
        }

        public OrthoBackgroundState(Texture2D backgroundImage,bool smoothBackground = true) {
            backgroundImageTexture = backgroundImage;
            this.smoothBackground = smoothBackground;
            OnLoad += OrthoBackgroundState_OnLoad;
            Initialize();
        }

        private void OrthoBackgroundState_OnLoad() {
            if(backgroundImageTexture != null) {
                background = new TextureEntity(backgroundImageTexture);
            } else {
                background = new TextureEntity(backgroundImage);
            }
            background.Name = BACKGROUND_ENTITY_NAME;
            background.PixelSmoothing = smoothBackground;
            background.Billboard = true;
            background.Scale = new Vector3(1f);
            Entities.Add(background);
        }

        private void SetOrthoCamera() {
            Camera = new AngleCamera() {
                NearPlane = 0.1f,
                FarPlane = 20f,
                FieldOfView = 75f,
                Orthographic = true,
                Angle = new Vector2(0f,180f),
                Position = new Vector3(0f,0f,10f)
            };
        }

        private void UpdateBackground(GameTime gameTime) {
            background.SetColors(backgroundColors);
            if(ScrollingBackground) {
                double scrollT = gameTime.TotalGameTime.TotalSeconds / ScrollingBackgroundPeriod % 1d;
                background.UVOffset = new Vector2((float)scrollT,0f);
            } else {
                background.UVOffset = Vector2.Zero;
            }
        }
    }
}
