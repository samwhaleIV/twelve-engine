using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace John {
    public static class Constants {
        public const int INPUT_TILE_SIZE = 16;
        public const float CAMERA_SCALE = 10f;

        public const float GRAVITY = 600f;

        public const string TILEMAP_TEXTURE = "spritesheet";
        public const string MAP_FILE = "clawmachinemap.json";

        public const float PHYSICS_SIM_SCALE = 8f;

        public const bool USE_CAM_BOUNDS = true;
        public const float CLAW_SPEED = 0.028f;

        public const float CAM_OFFSET_MIN_X = 2.5f, CAM_OFFSET_MAX_X = -2.5F, CAM_OFFSET_MIN_Y = 2.5f, CAM_OFFSET_MAX_Y = -(2.5f + 3f / 16f);
        public const float CLAW_OFFSET_MIN_X = 3.25f, CLAW_OFFSET_MAX_X = -3.25f, CLAW_OFFSET_MIN_Y = 3.5f, CLAW_OFFSET_MAX_Y = -4.5f;

        public static readonly HashSet<short> NON_COLLIDING_TILES = new() { 81,97,113,16 };

        public static readonly Vector2 GRABBY_CLAW_START_POS = new Vector2(15,26);

        public static readonly TimeSpan WALKING_ANIMATION_FRAME_LENGTH = TimeSpan.FromMilliseconds(100);
        public static readonly int WALKING_ANIMATION_FRAME_COUNT = 4;

        public static readonly Rectangle JOHN_ANIMATION_SOURCE = new Rectangle(16,16,27,16);

        public const int JOHN_SPRITESHEET_SIZE = 512, JOHN_BLOCK_SIZE = 32, JOHN_HEIGHT = 16, JOHN_WIDTH = 9;

        public const int JOHN_CONFIG_COUNT = (JOHN_SPRITESHEET_SIZE / JOHN_BLOCK_SIZE) * (JOHN_SPRITESHEET_SIZE / JOHN_BLOCK_SIZE) * 2;

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

        public static readonly TimeSpan JOHN_SUMMON_COOLDOWN = TimeSpan.FromSeconds(0.5f);
        public static readonly TimeSpan JOHN_SUMMON_VARIABILITY = TimeSpan.FromSeconds(0);

        public const float SURFACE_FRICTION = 0.2f;
        public const float JOHN_FRICTION = 0f;

        public const float JOHN_LINEAR_DAMPING = 8f;
        public const float JOHN_MOVEMENT_FORCE = 1f;

        public const float SCORING_Y_LIMIT = 23f;

        public const float WALKING_VELOCITY_DETECTION_THRESHOLD = 0.5f;

        public const float GOOD_BIN_X = 8, BAD_BIN_X = 22, FREEDOM_HOLE_X = 15;

        public const int MASK_TYPE_COUNT = 32;

        public const float FAKE_JOHN_PROBABILITY = 0.75f, REAL_JOHN_PROBABILITY = 0.1f;

        public const int ROUND_COMPLETION_COUNT = 5;

        public const float SCORING_MARGIN_OFFSET_X = 3f, SCORING_MARGIN_OFFSET_Y = 1.5f, RESET_MARGIN_OFFSET = 1;

        public const string NO_RANDOM_SEED_FLAG = "startseeded";

        public const float DPI_SCALE_CONSTANT = 0.008f;

        public const string PRESS_E_TEXT = "Press Enter to Continue";

        public static readonly string[] START_SCREEN_TEXT = new[] {
            "John has splintered into an endless stream of facsimiles, each but a sliver of his true form. Now, unsure of his identity, John needs your help to regain his individuality.",
            "Changing appearance in pants, shirt, and hair color, you must be able to differentiate the real Johns from the fakes. Good luck."
        };
    }
}
