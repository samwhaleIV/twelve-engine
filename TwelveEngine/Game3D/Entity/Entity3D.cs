using Microsoft.Xna.Framework;
using System;
using TwelveEngine.Serial;
using TwelveEngine.EntitySystem;

namespace TwelveEngine.Game3D.Entity {
    public abstract class Entity3D:Entity<World> {

        protected bool WorldMatrixValid { get; set; } = false;

        protected bool PositionValid { get; set; } = false;
        protected bool RotationValid { get; set; } = false;
        protected bool ScaleValid { get; set; } = false;

        private Vector3 _position = Vector3.Zero;
        private Vector3 _rotation = Vector3.Zero;
        private Vector3 _scale = Vector3.One;

        public Vector3 Position {
            get => _position;
            set {
                _position = value;
                PositionValid = false;
                WorldMatrixValid = false;
            }
        }

        public Vector3 Rotation {
            get => _rotation;
            set {
                _rotation = value;
                RotationValid = false;
                WorldMatrixValid = false;
            }
        }

        public Vector3 Scale {
            get => _scale;
            set {
                _scale = value;
                ScaleValid = false;
                WorldMatrixValid = false;
            }
        }

        private bool _billboard = false;
        public bool Billboard {
            get => _billboard;
            set {
                if(_billboard == value) {
                    return;
                }
                _billboard = value;
                PositionValid = false;
            }
        }

        public virtual bool IsVisible() => true;

        public event Action<GameTime> OnUpdate, OnRender, OnPreRender;

        public void Update(GameTime gameTime) => OnUpdate?.Invoke(gameTime);

        public void PreRender(GameTime gameTime) {
            if(!IsVisible()) {
                return;
            }
            OnPreRender?.Invoke(gameTime);
        }

        public void Render(GameTime gameTime) {
            if(!IsVisible()) {
                return;
            }
            OnRender?.Invoke(gameTime);
        }

        public static void Update(Entity3D entity,GameTime gameTime) => entity.Update(gameTime);
        public static void PreRender(Entity3D entity,GameTime gameTime) => entity.PreRender(gameTime);
        public static void Render(Entity3D entity,GameTime gameTime) => entity.Render(gameTime);
    }
}
