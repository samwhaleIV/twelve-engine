using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TwelveEngine.Game2D.Objects {
    public class GameObject {

        private Vector2 _position = Vector2.Zero, _size = Vector2.One;
        private float _rotation;

        protected virtual Vector2 GetPosition() => _position;
        protected virtual void SetPosition(Vector2 position) => _position = position;
        protected virtual float GetRotation() => _rotation;
        protected virtual void SetRotation(float rotation) => _rotation = rotation;
        protected virtual void SetSize(Vector2 size) => _size = size;
        protected virtual Vector2 GetSize() => _size;

        public float Rotation { get => GetRotation(); set => SetRotation(value); }
        public Vector2 Position { get => GetPosition(); set => SetPosition(value); }
        public Vector2 Size { get => GetSize(); set => SetSize(value); }

        private readonly int _id;
        internal int ID => _id;

        private readonly ObjectManager _owner;
        internal GameObject(ObjectManager owner,int ID) { _id = ID; _owner = owner; }

        internal protected ObjectManager Owner => _owner;
        protected PhysicsGrid2D Grid => _owner.Grid;

        public void Delete() => Owner.Delete(this);

        public bool IsLoaded { get; private set; } = false;
        private bool isUnloaded = false;
        protected Action OnLoad, OnUnload;

        internal void FireLoadEvent() {
            if(IsLoaded) {
                throw new InvalidOperationException("Cannot fire load event, GameObject has already been loaded!");
            }
            IsLoaded = true;
            OnLoad?.Invoke();
        }

        internal void FireUnloadEvent() {
            if(isUnloaded) {
                throw new InvalidOperationException("Cannot unload object, it has already been unloaded!");
            }
            if(!IsLoaded) {
                throw new InvalidOperationException("Cannot fire unload event, GameObject has never been loaded!");
            }
            OnUnload?.Invoke();
            isUnloaded = true;
        }

        public Rectangle TextureSource { get; set; } = new Rectangle(16,0,16,16);
        public Color Color { get; set; } = Color.White;
        public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
        public bool Invisible { get; set; } = false;
    }
}
