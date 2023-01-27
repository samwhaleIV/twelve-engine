using Microsoft.Xna.Framework;
using TwelveEngine.Game3D.Entity.Types;
using TwelveEngine.Game3D;
using TwelveEngine;
using System;
using TwelveEngine.Shell;

namespace Elves.Scenes {
    public abstract class Scene3D:GameState3D, IScene<Scene3D> {

        public event Action<Scene3D,ExitValue> OnSceneEnd;
        public void EndScene(ExitValue data) => OnSceneEnd?.Invoke(this,data);

        protected bool Debug { get; private set; } = Flags.Get(Constants.Flags.Debug);

        public Scene3D() {
            Name = "3D Scene";

            OnRender += RenderEntities;
            OnPreRender += PreRenderEntities;
            OnUpdate += Scene_OnUpdate;
            OnLoad += Scene_OnLoad;

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
            return Game.Viewport.Height / VERTICAL_SCALE_DIVISOR;
        }

        private void Scene_OnUpdate() {
            if(!Debug || Camera is null) {
                return;
            }
            (Camera as AngleCamera).UpdateFreeCam(this,Constants.Debug3DLookSpeed,Constants.Debug3DMovementSpeed);
        }

        private void Scene_OnLoad() {
            if(Debug) {
                Entities.Add(new GridLinesEntity());
            }
        }
    }
}
