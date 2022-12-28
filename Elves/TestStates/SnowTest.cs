using Elves.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine;
using TwelveEngine.Game3D.Entity.Types;

namespace Elves.TestStates {

    public sealed class FallingSnowParticleSystem:ParticleSystem {

        private const int SNOW_COUNT = 10000;
        private const float TIME_BASE = 10f;

        private const float PARTICLE_SIZE = 0.025f;

        private const float MIN_X = 0.1f;
        private const float X_RANGE = 0.9f;

        private const float MIN_Y = 1 / 5f;
        private const float Y_RANGE = 2 / 3f;

        private const float DEPTH_RANGE = 1f;

        private const float COSINE_RADIUS = 0.0625f;

        private readonly Random random = new();

        private readonly Vector2[] p_velocity = new Vector2[SNOW_COUNT];
        private readonly float[] p_centerX = new float[SNOW_COUNT];

        public FallingSnowParticleSystem() : base(SNOW_COUNT) {

            ParticleSize = PARTICLE_SIZE;

            Texture = UITextures.Menu;
            SamplerState = SamplerState.PointClamp;
            UVArea = new VectorRectangle(38,78,4,4,Texture);

            for(int i = 0;i<SNOW_COUNT;i++) {
                ResetParticle(i);
                var position = p_position[i];
                position.Y = random.NextSingle();
                position.Z = (float)i / SNOW_COUNT * DEPTH_RANGE - DEPTH_RANGE * 0.5f;
                p_position[i] = position;

            }
        }

        private bool RandomBool() {
            return random.NextSingle() > 0.5f;
        }

        private Vector2 GetVelocity() {
            Vector2 vector;
            float x = MIN_X + random.NextSingle() * X_RANGE;
            vector.X = (RandomBool() ? x : -x) * MathHelper.TwoPi;
            vector.Y = -(MIN_Y + random.NextSingle() * Y_RANGE);
            return vector;
        }

        private void ResetParticle(int index) {
            p_velocity[index] = GetVelocity();
            p_centerX[index] = random.NextSingle();
        }

        protected override void UpdatePositions() {
            Vector3 position;
            Vector2 velocity;

            TimeSpan elapsedTime = Time.ElapsedGameTime;
            float delta = (float)elapsedTime.TotalSeconds / TIME_BASE;

            for(int i = 0;i<SNOW_COUNT;i++) {
                position = p_position[i];
                velocity = p_velocity[i];

                position.Y += velocity.Y * delta;
                position.X = p_centerX[i] + MathF.Sin(position.Y * velocity.X) * COSINE_RADIUS;

                if(position.Y < 0) {
                    position.Y = 1;
                    ResetParticle(i);
                }
                p_position[i] = position;
            }
        }
    }

    public sealed class SnowTest:OrthoBackgroundState {

        public SnowTest(bool fadeIn = false):base(UITextures.Nothing,false) {
            OnLoad += SnowTest_OnLoad;
            SetBackgroundColor(Color.Black);
            if(fadeIn) {
                TransitionIn(TimeSpan.FromSeconds(0.125f));
            }
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
