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

        public const float GRAVITY = 600f;

        public const string TILEMAP_TEXTURE = "spritesheet";
        public const string MAP_FILE = "john-collection.json";

        public const float PHYSICS_SIM_SCALE = 8f;

        public const bool USE_CAM_BOUNDS = true;
        public const float CLAW_SPEED = 0.02f;

        public const float CAM_OFFSET_MIN_X = 2.5f, CAM_OFFSET_MAX_X = -2.5F, CAM_OFFSET_MIN_Y = 2.5f, CAM_OFFSET_MAX_Y = -2.5F;
        public const float CLAW_OFFSET_MIN_X = 3f, CLAW_OFFSET_MAX_X = -3f, CLAW_OFFSET_MIN_Y = 3.25f, CLAW_OFFSET_MAX_Y = -3.25f;

        public static readonly HashSet<short> NON_COLLIDING_TILES = new() { 81,97,113,16 };

        public static readonly Vector2 GRABBY_CLAW_START_POS = new Vector2(15,26); //TODO: Set real value

        public static readonly TimeSpan WALKING_ANIMATION_FRAME_LENGTH = TimeSpan.FromMilliseconds(100);
        public static readonly int WALKING_ANIMATION_FRAME_COUNT = 4;

        public static readonly Rectangle JOHN_ANIMATION_SOURCE = new Rectangle(16,16,27,16);

        public static readonly TimeSpan JOHN_GAME_DURATION = TimeSpan.FromMinutes(2);

        public const int JOHN_CONFIG_COUNT = 32, JOHN_BLOCK_SIZE = 32, JOHN_HEIGHT = 16;

        public static readonly JohnStartPosition[] JOHN_SPAWN_LOCATIONS = new JohnStartPosition[] {
            new JohnStartPosition() { Value = new Vector2(15,1.5f), Direction = JohnStartPositionDirection.Random },

            new JohnStartPosition() { Value = new Vector2(1.5f,4.5f), Direction = JohnStartPositionDirection.FacingRight },
            new JohnStartPosition() { Value = new Vector2(1.5f,12.5f), Direction = JohnStartPositionDirection.FacingRight },
            new JohnStartPosition() { Value = new Vector2(1.5f,20.5f), Direction = JohnStartPositionDirection.FacingRight },

            new JohnStartPosition() { Value = new Vector2(28.5f,4.5f), Direction = JohnStartPositionDirection.FacingLeft },
            new JohnStartPosition() { Value = new Vector2(28.5f,12.5f), Direction = JohnStartPositionDirection.FacingLeft },
            new JohnStartPosition() { Value = new Vector2(28.5f,20.5f), Direction = JohnStartPositionDirection.FacingLeft },
        };

        public const int MAX_ARENA_JOHNS = 64;
        public const float REAL_JOHN_PROBABILITY = 0.1f;

        public static readonly TimeSpan JOHN_SUMMON_COOLDOWN = TimeSpan.FromSeconds(0.5f);
        public static readonly TimeSpan JOHN_SUMMON_VARIABILITY = TimeSpan.FromSeconds(0);

        public const float SURFACE_FRICTION = 0.2f;
        public const float JOHN_FRICTION = 0f;

        public const float JOHN_LINEAR_DAMPING = 8f;
        public const float JOHN_MOVEMENT_FORCE = 1f;

        public const float SCORING_Y_LIMIT = 23f;

        public const float WALKING_VELOCITY_DETECTION_THRESHOLD = 0.5f;

        public const float GOOD_BIN_X = 8, BAD_BIN_X = 22, FREEDOM_HOLE_X = 15;
    }
}
