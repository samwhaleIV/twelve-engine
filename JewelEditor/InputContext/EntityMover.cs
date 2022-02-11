using JewelEditor.Entity;
using JewelEditor.HistoryActions;
using Microsoft.Xna.Framework;
using TwelveEngine.EntitySystem;
using TwelveEngine.Game2D;

namespace JewelEditor.InputContext {
    internal sealed class EntityMover {

        private readonly EntityManager<Entity2D,Grid2D> entityManager;
        public EntityMover(EntityManager<Entity2D,Grid2D> entityManager) => this.entityManager = entityManager;

        private Entity2D targetEntity = null;

        public bool HasTarget => targetEntity != null;

        private Vector2 offset = Vector2.Zero;

        public float SnapResolution { get; set; } = 2f;

        private Vector2 oldPosition;

        private HistoryEventToken eventToken;

        public void SearchForTarget(StateEntity state,Vector2 location) {
            foreach(var marker in entityManager.GetByType(JewelEntities.EntityMarker)) {
                if(!marker.Contains(location)) {
                    continue;
                }
                targetEntity = marker;
                offset = marker.Position - location;
                oldPosition = marker.Position;
                (marker as EntityMarker).Highlighted = true;
                eventToken = state.StartHistoryEvent();
                return;
            }
        }
        
        private Vector2 GetSnappedPosition(Vector2 location,float resolution) {
            return Vector2.Round((offset + location) * resolution) / resolution;
        }
        private Vector2 GetPosition(Vector2 location) {
            return offset + location;
        }

        public void ReleaseTarget(StateEntity state,Vector2 location) {
            if(!HasTarget) {
                return;
            }
            if(targetEntity is EntityMarker marker) {
                marker.Highlighted = false;
            }
            Vector2 newPosition = GetSnappedPosition(location,SnapResolution);
            if(newPosition != oldPosition && targetEntity.HasName) {
                state.AddEventAction(eventToken,new MoveEntity(targetEntity.Name,newPosition,oldPosition));
            }
            state.EndHistoryEvent(eventToken);
            targetEntity.Position = newPosition;
            targetEntity = null;
        }

        public void MoveTarget(Vector2 location) {
            if(!HasTarget) {
                return;
            }
            targetEntity.Position = GetPosition(location);
        }
    }
}
