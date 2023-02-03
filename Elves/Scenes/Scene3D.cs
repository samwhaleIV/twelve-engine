using Microsoft.Xna.Framework;
using TwelveEngine.Game3D.Entity.Types;
using TwelveEngine.Game3D;
using TwelveEngine;
using System;

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
        }

        private void SetCamera() => Camera = new AngleCamera() {
            NearPlane = 0.1f,
            FarPlane = 100f,
            FieldOfView = 75f,
            Orthographic = false,
            Angle = new Vector2(0f,180f),
            Position = new Vector3(0f,0f,Constants.Depth.Cam)
        };   

        private const float VERTICAL_SCALE_DIVISOR = 70f;


        public float GetUIScale() {
            return Viewport.Height / VERTICAL_SCALE_DIVISOR;
        }

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
