using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Game3D;
using TwelveEngine.Game3D.Entity.Types;

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

        public void SetBackgroundColor(Color color) {
            backgroundColors[0] = color;
            backgroundColors[1] = color;
            backgroundColors[2] = color;
            backgroundColors[3] = color;
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
            OnUpdate += OrthoBackgroundState_OnUpdate;
        }

        private void OrthoBackgroundState_OnUpdate(GameTime gameTime) {
            if(!debug3D || Camera == null) {
                return;
            }
            (Camera as AngleCamera).UpdateFreeCam(
                this,Constants.Debug3DLookSpeed,Constants.Debug3DMovementSpeed
            );
        }

        private readonly string backgroundImage;
        private readonly bool smoothBackground;
        private readonly Texture2D backgroundImageTexture;

        private readonly bool debug3D = false;

        protected bool Debug3D => debug3D;

        public OrthoBackgroundState(string backgroundImage,bool smoothBackground = true,bool debug3D = false) {
            this.backgroundImage = backgroundImage;
            this.smoothBackground = smoothBackground;
            this.debug3D = debug3D;
            OnLoad += OrthoBackgroundState_OnLoad;
            Initialize();
        }

        public OrthoBackgroundState(Texture2D backgroundImage,bool smoothBackground = true,bool debug3D = false) {
            backgroundImageTexture = backgroundImage;
            this.smoothBackground = smoothBackground;
            this.debug3D = debug3D;
            OnLoad += OrthoBackgroundState_OnLoad;
            Initialize();
        }

        private void OrthoBackgroundState_OnLoad() {
            if(debug3D) {
                Entities.Add(new GridLinesEntity());
            }
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

        private void SetOrthoCamera() {
            Camera = new AngleCamera() {
                NearPlane = 0.1f,
                FarPlane = 100f,
                FieldOfView = 75f,
                Orthographic = !debug3D,
                Angle = new Vector2(0f,180f),
                Position = new Vector3(0f,0f,DepthConstants.OrthoCam)
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
