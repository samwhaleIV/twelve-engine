using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace John {
    public static class Constants {
        public const int INPUT_TILE_SIZE = 16;
        public const float CAMERA_SCALE = 10f;

        public const float GRAVITY = 200f;

        public const string TILEMAP_TEXTURE = "spritesheet";
        public const string MAP_FILE = "john-collection.json";

        public const float PHYSICS_SIM_SCALE = 8f;

        public const float CAM_OFFSET_MIN_X = 0.25F, CAM_OFFSET_MAX_X = -0.25F, CAM_OFFSET_MIN_Y = 0.0f, CAM_OFFSET_MAX_Y = -0.75F;

        public const float CLAW_OFFSET_MIN_X = 22 / 16f, CLAW_OFFSET_MAX_X = -22 / 16f, CLAW_OFFSET_MIN_Y = 0.5f, CLAW_OFFSET_MAX_Y = 20 / 16f;

        public static readonly HashSet<short> NON_COLLIDING_TILES = new() { 81,97,113 };

        public static readonly Vector2 GRABBY_CLAW_START_POS = new Vector2(13,18.5f);

        public static readonly TimeSpan WALKING_ANIMATION_FRAME_LENGTH = TimeSpan.FromMilliseconds(100);
        public static readonly int WALKING_ANIMATION_FRAME_COUNT = 4;

        public static readonly Rectangle JOHN_ANIMATION_SOURCE = new Rectangle(16,16,27,16);
    }
}
