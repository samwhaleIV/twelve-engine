namespace JewelEditor.HistoryActions {
    internal abstract class EditEntity:HistoryAction {

        private readonly string entityName;
        public string EntityName => entityName;

        public EditEntity(string entityName) => this.entityName = entityName;
    }
}
