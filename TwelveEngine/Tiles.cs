namespace TwelveEngine {
    public static class Tiles {
        public const int LaserLeftOn = 320;
        public const int LaserRightOn = 321;
        public const int LaserUpOn = 293;
        public const int LaserDownOn = 325;

        public const int LaserLeftOff = 352;
        public const int LaserRightOff = 353;
        public const int LaserUpOff = 293;
        public const int LaserDownOff = 324;

        public const int PulsePlusOn = 236;
        public const int PulsePlusOff = 235;

        public const int PulseMinusOn = 268;
        public const int PulseMinusOff = 267;

        public const int SwitchRightOn = 257;
        public const int SwitchLeftOn = 288;

        public const int SwitchRightOff = 256;
        public const int SwitchLeftOff = 289;

        public const int HorizontalLaser = 322;
        public const int VerticalLaser = 323;
        public const int CrossLaser = 290;

        public readonly struct CounterTiles {
            public readonly int[] Small;
            public readonly int[] Big;
            public CounterTiles(int[] small,int[] big) {
                Small = small; Big = big;
            }
        }

        public static class Counter {
            public static readonly CounterTiles Horizontal = new CounterTiles(
                new int[] { 172,173,204 },
                new int[] { 169,170,201,202,234 }
            );
            public static readonly CounterTiles Vertical = new CounterTiles(
                new int[] { 205,237,269 },
                new int[] { 300,301,332,333,364 }
            );
        }

        public static class Collision {
            public const int HorizontalLaser = 48;
            public const int VerticalLaser = 49;
        }
    }
}
