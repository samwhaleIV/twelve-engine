using JewelEditor.Entity;
using Microsoft.Xna.Framework;
using TwelveEngine.EntitySystem;
using TwelveEngine.Game2D;

namespace JewelEditor.InputContext {
    internal sealed class EntityMover {

        private readonly EntityManager<Entity2D,Grid2D> entityManager;
        public EntityMover(EntityManager<Entity2D,Grid2D> entityManager) => this.entityManager = entityManager;

        private Entity2D targetEntity = null;

        public bool HasTarget => targetEntity != null;

        public void SearchForTarget(Vector2 location) {
            foreach(var marker in entityManager.GetByType(JewelEntities.EntityMarker)) {
                if(!marker.Contains(location)) {
                    continue;
                }
                targetEntity = marker;
                (marker as EntityMarker).Highlighted = true;
                return;
            }
        }
        
        private void UpdateEntityPosition(Vector2 mouseLocation) {
            targetEntity.Position = Vector2.Floor(mouseLocation);
        }

        public void ReleaseTarget(Vector2 location) {
            if(!HasTarget) {
                return;
            }
            if(targetEntity is EntityMarker marker) {
                marker.Highlighted = false;
            }
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
