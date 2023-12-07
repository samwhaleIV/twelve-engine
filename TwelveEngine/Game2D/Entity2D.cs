using TwelveEngine.EntitySystem;

namespace TwelveEngine.Game2D {
    public abstract class Entity2D:Entity<GameState2D> {
        private EntityUpdatePriority _priority = EntityUpdatePriority.Neutral;
        public EntityUpdatePriority UpdatePriority {
            get => _priority;
            set {
                if(value == _priority) {
                    return;
                }
                _priority = value;
                NotifySortedOrderChange();
            }
        }

        protected abstract Vector2 GetPosition();
        protected abstract void SetPosition(Vector2 position);

        public Vector2 Position { get => GetPosition(); set => SetPosition(value); }

        public float LayerDepth { get; set; } = 0.5f;

        public Vector2 Size { get; protected init; } = Vector2.One;
        public Vector2 Origin { get; protected init; } = Vector2.Zero;
    }
}
