using Elves.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using TwelveEngine;
using TwelveEngine.Game3D.Entity.Types;

namespace Elves {

    public sealed class FallingSnowParticleSystem:ParticleSystem {

        private const int SNOW_COUNT = 128;
        private const float TIME_BASE = 20f;

        private const float SWAY_SCALE = 1 / 5f;
        private const float PARTICLE_SIZE = 0.025f;

        private const float MIN_X = 0.1f;
        private const float X_RANGE = 0.9f;

        private const float MIN_Y = 1 / 5f;
        private const float Y_RANGE = 2 / 3f;

        private const float DEPTH_RANGE = 0.5f;

        private readonly Random random = new Random();

        private readonly Vector2[] p_velocity = new Vector2[SNOW_COUNT];
        private readonly float[] p_centerX = new float[SNOW_COUNT];
        private readonly TimeSpan[] p_startTime = new TimeSpan[SNOW_COUNT];
        private readonly bool[] p_polarity = new bool[SNOW_COUNT];

        public FallingSnowParticleSystem() : base(SNOW_COUNT) {

            ParticleSize = PARTICLE_SIZE;

            Texture = UITextures.Menu;
            SamplerState = SamplerState.PointClamp;
            UVArea = new VectorRectangle(38,78,4,4,Texture);

            TimeSpan now = Now;
            for(int i = 0;i<SNOW_COUNT;i++) {
                ResetParticle(i);
                p_startTime[i] = now - TimeSpan.FromSeconds(p_velocity[i].Y * random.NextSingle());
                p_position[i].Z = (((float)i / SNOW_COUNT) * DEPTH_RANGE) - DEPTH_RANGE * 0.5f;
            }
        }

        private bool RandomBool() {
            return random.NextSingle() > 0.5f;
        }

        private Vector2 GetVelocity() {
            Vector2 vector;
            float x = MIN_X + random.NextSingle() * X_RANGE;
            vector.X = (RandomBool() ? x : -x) * MathHelper.TwoPi;
            vector.Y = (MIN_Y + random.NextSingle() * Y_RANGE) * TIME_BASE;
            return vector;
        }

        private void ResetParticle(int index) {
            p_startTime[index] = Now;
            p_velocity[index] = GetVelocity();
            p_centerX[index] = random.NextSingle();
            p_polarity[index] = RandomBool();
        }

        private float SinOrCos(float value,bool polarity) {
            return polarity ? MathF.Sin(value) : MathF.Cos(value);
        }

        protected override void UpdatePositions() {

            Vector3 position;
            Vector2 velocity;
            float t;

            for(int i = 0;i<SNOW_COUNT;i++) {
                position = p_position[i];
                velocity = p_velocity[i];

                t = (float)(Time.TotalGameTime-p_startTime[i]).TotalSeconds / velocity.Y;

                position.X = p_centerX[i] + SinOrCos(Math.Abs(velocity.X) * t,p_polarity[i]) * Math.Sign(velocity.X) * SWAY_SCALE;

                position.Y = MathHelper.Lerp(1,0,t);
                p_position[i] = position;

                if(t >= 1f) {
                    ResetParticle(i);
                }
            }
        }
    }

    public sealed class SnowTest:OrthoBackgroundState {

        public SnowTest():base(UITextures.Nothing,false) {
            OnLoad += SnowTest_OnLoad;
            SetBackgroundColor(Color.Black);
        }

        private void SnowTest_OnLoad() {
            var particleSystem = new FallingSnowParticleSystem() {
                Position = new Vector3(0,0,DepthConstants.Middle)
            };
            Entities.Add(particleSystem);
            Background.IsVisible = false;
        }
    }
}
