using Microsoft.Xna.Framework;
using TwelveEngine.Game3D.Entity.Types;
using TwelveEngine.Game3D;
using TwelveEngine;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TwelveEngine.Shell.UI;

namespace Elves.Scenes {
    public abstract class Scene3D:GameState3D, IScene<Scene3D> {

        public event Action<Scene3D,ExitValue> OnSceneEnd;
        public void EndScene(ExitValue data) => OnSceneEnd?.Invoke(this,data);

        protected bool DebugOrtho { get; private set; } = Flags.Get(Constants.Flags.OrthoDebug);

        public Scene3D() : base(EntitySortMode.CameraFixed) {
            Name = "3D Scene";

            OnRender.Add(RenderEntities);
            OnPreRender.Add(PreRenderEntities);
            OnUpdate.Add(Update);
            OnLoad.Add(Load);

            SetCamera();

            OnWriteDebug.Add(WritePixelScaleModifier);

            Mouse.Router.OnScroll += MouseScroll;
        }

        private void WritePixelScaleModifier(DebugWriter writer) {
            writer.Write(PixelScaleModifier,"Pixel Scale");
        }

        private void MouseScroll(Direction direction) {
            if(!Impulse.Keyboard.IsKeyDown(Keys.LeftControl)) {
                return;
            }
            PixelScaleModifier += (direction == Direction.Down ? -1 : 1) * 0.1f;
        }

        private void SetCamera() => Camera = new AngleCamera() {
            NearPlane = 0.1f,
            FarPlane = 100f,
            FieldOfView = 75f,
            Orthographic = false,
            Angle = new Vector2(0f,180f),
            Position = new Vector3(0f,0f,Constants.Depth.Cam)
        };

        private float _pixelScaleModifier = 1;

        public float PixelScaleModifier {
            get => _pixelScaleModifier;
            set {
                if(_pixelScaleModifier == value) {
                    return;
                }
                value = MathF.Min(value,Constants.UI.MaxScaleModifier);
                value = MathF.Max(value,Constants.UI.MinScaleModifier);
                _pixelScaleModifier = value;
            }
        }

        public float PixelScale => Viewport.Height * Constants.UI.PixelScaleDivisor * PixelScaleModifier;

        private void Update() {
            if(!DebugOrtho || Camera is null) {
                return;
            }
            (Camera as AngleCamera).UpdateFreeCam(this,Constants.Debug3DLookSpeed,Constants.Debug3DMovementSpeed);
        }

        private void Load() {
            if(!DebugOrtho) {
                return;
            }
            Entities.Add(new GridLinesEntity());
            Camera.Orthographic = false;
        }
    }
}
