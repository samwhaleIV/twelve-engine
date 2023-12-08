using Microsoft.Xna.Framework;
using TwelveEngine.Game3D.Entity.Types;
using TwelveEngine.Game3D;
using TwelveEngine;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TwelveEngine.Shell.UI;

namespace Elves.Scenes {
    public abstract class Scene3D:GameState3D {
        protected bool DebugOrtho { get; private set; } = Flags.Get(Constants.Flags.OrthoDebug);

        public Scene3D() : base(EntitySortMode.CameraFixed) {
            Name = "3D Scene";

            OnRender.Add(RenderEntities);
            OnPreRender.Add(PreRenderEntities);
            OnUpdate.Add(Update);
            OnLoad.Add(Load);

            SetCamera();

            OnWriteDebug.Add(WriteUIScale);

            MouseHandler.Router.OnScroll += MouseScroll;
        }

        private void WriteUIScale(DebugWriter writer) {
            writer.Write(UIScaleModifier,"UI Scale");
        }

        private void MouseScroll(Direction direction) {
            if(!ImpulseHandler.Keyboard.IsKeyDown(Keys.LeftControl)) {
                return;
            }
            UIScaleModifier += (direction == Direction.Down ? -1 : 1) * 0.1f;
        }

        private void SetCamera() => Camera = new AngleCamera() {
            NearPlane = 0.1f,
            FarPlane = 100f,
            FieldOfView = 75f,
            Orthographic = false,
            Angle = new Vector2(0f,180f),
            Position = new Vector3(0f,0f,Constants.Depth.Cam)
        };

        private float _uiScaleModifier = 1;

        public float UIScaleModifier {
            get => _uiScaleModifier;
            set {
                if(_uiScaleModifier == value) {
                    return;
                }
                value = MathF.Min(value,Constants.UI.MaxUIScale);
                value = MathF.Max(value,Constants.UI.MinUIScale);
                _uiScaleModifier = value;
            }
        }

        public float UIScale => Viewport.Height * Constants.UI.UIScaleBaseDivisor * UIScaleModifier;

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
