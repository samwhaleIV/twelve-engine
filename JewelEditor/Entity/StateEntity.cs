using TwelveEngine.Serial;

namespace JewelEditor.Entity {
    internal sealed class StateEntity:JewelEntity {

        protected override int GetEntityType() => JewelEntities.StateEntity;

        public InputMode InputMode { get; set; } = InputMode.Pointer;
        public TileType TileType { get; set; } = TileType.None;

        public StateEntity() {
            OnImport += StateEntity_OnImport;
            OnExport += StateEntity_OnExport;
        }

        private void StateEntity_OnExport(SerialFrame frame) {
            frame.Set((int)InputMode);
            frame.Set((int)TileType);
        }

        private void StateEntity_OnImport(SerialFrame frame) {
            InputMode = (InputMode)frame.GetInt();
            TileType = (TileType)frame.GetInt();
        }
    }
}
