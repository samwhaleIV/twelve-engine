using TwelveEngine.Game2D.Entity;

namespace Porthole.Collision {
    public interface IInteract {
        Hitbox GetHitbox();
        void Interact(Entity2D source);
    }
}
