using TwelveEngine.Game2D.Entity;

namespace TwelveEngine.Game2D.Collision {
    public interface IInteract {
        Hitbox GetHitbox();
        void Interact(Entity2D source);
    }
}
