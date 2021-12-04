namespace TwelveEngine {
    public sealed partial class SerialFrame {
        public void Set(EntityType entityType) => Set((int)entityType);
        public EntityType GetEntityType() => (EntityType)GetInt();
    }
}
