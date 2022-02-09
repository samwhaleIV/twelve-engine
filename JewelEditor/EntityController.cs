using Microsoft.Xna.Framework;
using TwelveEngine.EntitySystem;
using TwelveEngine.Game2D;

namespace JewelEditor {
    internal sealed class EntityController {

        private readonly EntityManager<Entity2D,Grid2D> entityManager;
        public EntityController(EntityManager<Entity2D,Grid2D> entityManager) => this.entityManager = entityManager;

        private Entity2D targetEntity = null;

        public bool HasTarget => targetEntity != null;

        public void SearchForTarget(Vector2 location) {
            targetEntity = entityManager.Search(entity => {
                if(entity.Type != JewelEntities.EntityMarker) {
                    return false;
                }
                return entity.Contains(location);
            });
            if(targetEntity == null) {
                return;
            }
            var entity = (targetEntity as EntityMarker);
            entity.Highlighted = true;
        }
        
        private void UpdateEntityPosition(Vector2 mouseLocation) {
            targetEntity.Position = Vector2.Floor(mouseLocation);
        }

        public void ReleaseTarget(Vector2 location) {
            if(!HasTarget) {
                return;
            }
            var entity = (targetEntity as EntityMarker);
            entity.Highlighted = false;
            UpdateEntityPosition(location);
            targetEntity = null;
        }

        public void MoveTarget(Vector2 location) {
            if(!HasTarget) {
                return;
            }
            UpdateEntityPosition(location);
        }
    }
}
