using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine.Serial;

namespace TwelveEngine.Game2D.Objects {
    public class GameObject {

        private Vector2 _position = Vector2.Zero, _size = Vector2.One;
        private float _rotation;

        protected virtual Vector2 GetPosition() {
            return _position;
        }
        protected virtual void SetPosition(Vector2 position) {
            _position = position;
        }

        protected virtual float GetRotation() {
            return _rotation;
        }

        protected virtual void SetRotation(float rotation) {
            _rotation = rotation;
        }

        protected virtual void SetSize(Vector2 size) {
            _size = size;
        }
        protected virtual Vector2 GetSize() {
            return _size;
        }

        public float Rotation {
            get => GetRotation();
            set => SetRotation(value);
        }

        public Vector2 Position {
            get => GetPosition();
            set => SetPosition(value);
        }

        public Vector2 Size {
            get => GetSize();
            set => SetSize(value);
        }

        internal GameObject(ObjectManager owner,int ID) {
            _id = ID;
            _owner = owner;
        }

        private readonly ObjectManager _owner;

        internal protected ObjectManager Owner => _owner;
        protected PhysicsGrid2D Grid => _owner.Grid;

        private readonly int _id;

        public int ID => _id;

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

        protected Action<SerialFrame> OnImport, OnExport;

        public Rectangle TextureSource { get; set; } = new Rectangle(16,0,16,16);
        public bool RenderShadow { get; set; } = false;
        public Vector2 ShadowOffset { get; set; } = Vector2.Zero;
        public Color Color { get; set; } = Color.White;
        public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
        public bool Invisible { get; set; } = false;

        internal void Export(SerialFrame frame) {
            frame.Set(Position);
            frame.Set(Rotation);
            frame.Set(Size);
            frame.Set(TextureSource);
            frame.Set(RenderShadow);
            frame.Set(ShadowOffset);
            frame.Set(Color);
            frame.Set((int)SpriteEffects);
            frame.Set(Invisible);
            OnExport?.Invoke(frame);
        }

        internal void Import(SerialFrame frame) {
            Position = frame.GetVector2();
            Rotation = frame.GetFloat();
            Size = frame.GetVector2();
            TextureSource = frame.GetRectangle();
            RenderShadow = frame.GetBool();
            ShadowOffset = frame.GetVector2();
            Color = frame.GetColor();
            SpriteEffects = (SpriteEffects)frame.GetInt();
            Invisible = frame.GetBool();
            OnImport?.Invoke(frame);
        }
    }
}
