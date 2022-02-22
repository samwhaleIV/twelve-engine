﻿namespace TwelveEngine.Physics {
    public sealed class VelocityData:IVelocityData {
        public float Acceleration { get; set; }
        public float Decay { get; set; }
        public float MaxVelocity { get; set; }
        public float ReboundStrength { get; set; }
    }
}
