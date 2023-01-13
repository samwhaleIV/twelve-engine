using Microsoft.Xna.Framework;
using TwelveEngine.Game3D.Entity.Types;
using TwelveEngine.Game3D;
using TwelveEngine;

namespace Elves {
    public abstract class WorldBase:World {

        private readonly bool debug3D = Flags.Get(Constants.Flags.Debug3D);
        protected bool Debug3D => debug3D;

        public WorldBase() {
            Name = "World Base";

            OnRender += RenderEntities;
            OnPreRender += PreRenderEntities;
            OnUpdate += WorldBase_OnUpdate;
            OnLoad += WorldBase_OnLoad;

            Initialize();
        }

        private void Initialize() {
            WriteDebugEnabled = debug3D;
            Camera = new AngleCamera() {
                NearPlane = 0.1f,
                FarPlane = 100f,
                FieldOfView = 75f,
                Orthographic = false,
                Angle = new Vector2(0f,180f),
                Position = new Vector3(0f,0f,DepthConstants.Cam)
            };
        }

        private const float VERTICAL_SCALE_DIVISOR = 70f;

        public float GetUIScale() {
            return Game.Viewport.Height / VERTICAL_SCALE_DIVISOR;
        }

        private void WorldBase_OnUpdate() {
            if(!debug3D || Camera is null) {
                return;
            }
            (Camera as AngleCamera).UpdateFreeCam(this,Constants.Debug3DLookSpeed,Constants.Debug3DMovementSpeed);
        }

        private void WorldBase_OnLoad() {
            if(debug3D) {
                Entities.Add(new GridLinesEntity());
            }
        }
    }
}
