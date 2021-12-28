﻿namespace TwelveEngine.Game2D {
    public interface IInteract {
        Hitbox GetHitbox();
        void Interact(Entity2D source);
    }
}
