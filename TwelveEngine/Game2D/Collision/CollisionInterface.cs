﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TwelveEngine.Game2D.Collision {
    public abstract class CollisionInterface {
        public abstract CollisionResult Collides(Hitbox source);
    }
}
