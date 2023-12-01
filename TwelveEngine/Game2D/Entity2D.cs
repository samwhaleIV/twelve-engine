using TwelveEngine.EntitySystem;

namespace TwelveEngine.Game2D {
    public abstract class Entity2D:Entity<GameState2D> {
        private EntityPriority _priority = EntityPriority.Neutral;
        public EntityPriority Priority {
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
    }
}
