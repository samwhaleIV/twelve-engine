using System;
using Microsoft.Xna.Framework;
using TwelveEngine.Game2D.Entities;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using nkast.Aether.Physics2D.Dynamics;
using System.Collections.Generic;

namespace John {

    using static Constants;

    public sealed class WalkingJohn:PhysicsEntity2D {

        private readonly JohnCollectionGame _game;

        private readonly float _johnMinX = JOHN_EDGE_LIMIT, _johnMaxX, _johnMaxY, _leftResetX = JOHN_EDGE_LIMIT + JOHN_EDGE_SWAP_MARGIN, _rightResetX;

        public WalkingJohn(JohnCollectionGame johnCollectionGame) : base(new Vector2(9/16f,1f),new Vector2(0.5f)) {
            _game = johnCollectionGame;
            OnRender += WalkingJohn_OnRender;
            OnUpdate += WalkingJohn_OnUpdate;
            OnLoad += WalkingJohn_OnLoad;

            LinearDamping = JOHN_LINEAR_DAMPING;
            Restitution = 0f;
            Friction = JOHN_FRICTION;

            _johnMaxX =  _game.TileMap.Width - JOHN_EDGE_LIMIT;
            _johnMaxY =  _game.TileMap.Height - JOHN_EDGE_LIMIT;
            _rightResetX = _johnMaxX - JOHN_EDGE_SWAP_MARGIN;
        }

        private void WalkingJohn_OnLoad() {
            Body.OnCollision += Body_OnCollision;
            Body.OnSeparation += Body_OnSeparation;
            Fixture.CollisionCategories = Category.Cat1 | Category.Cat2;
        }

        private readonly List<Fixture> _collisionList = new();

        private void Body_OnSeparation(Fixture sender,Fixture other,Contact contact) {
            _collisionList.Remove(other);
        }

        private bool Body_OnCollision(Fixture sender,Fixture other,Contact contact) {
            _collisionList.Add(other);
            return true;
        }

        private TimeSpan _lastDirectionChangeTime = TimeSpan.Zero;
        private static readonly TimeSpan _directionChangeTimeout = TimeSpan.FromSeconds(0.2f);

        private void ChangeDirection() {
            if(Now - _lastDirectionChangeTime >  _directionChangeTimeout) {
                _movementForceModifier = -_movementForceModifier;
                _lastDirectionChangeTime = Now;
            }
        }

        private float _movementForceModifier = 1f;

        private TimeSpan _lastWalkingTime = TimeSpan.Zero, _lastWalkingResetTime = TimeSpan.Zero;

        private TimeSpan _stuckTimeout = _stuckTimeoutBase;

        private static readonly TimeSpan _stuckTimeoutBase = TimeSpan.FromSeconds(1f), _stuckTimeoutVariability = TimeSpan.FromSeconds(0.8f);

        private TimeSpan GetRandomDirectionSwapTimeout() {
            return _stuckTimeoutBase + _stuckTimeoutVariability * _game.Random.NextDouble() * 2 - _stuckTimeoutVariability;
        }

        private void NotMovingDetection() {
            if(!ShouldApplyForce) {
                return;
            }
            if(IsMoving) {
                _lastWalkingTime = Now;
                _stuckTimeout = GetRandomDirectionSwapTimeout();
            } else if(Now - _lastWalkingTime > _stuckTimeout && Now - _lastWalkingResetTime > _stuckTimeout) {
                _lastWalkingResetTime = Now;
                ChangeDirection();
            }
        }

        private void SetDirectionForFinalDestination() {
            if(Position.Y < SCORING_Y_LIMIT) {
                return;
            }
            if(Position.X < GOOD_BIN_X) {
                _movementForceModifier = -MathF.Abs(_movementForceModifier);
            } else if(Position.X > BAD_BIN_X) {
                _movementForceModifier = MathF.Abs(_movementForceModifier);
            } else if(Position.X < FREEDOM_HOLE_X) {
                _movementForceModifier = MathF.Abs(_movementForceModifier);
            } else {
                _movementForceModifier = -MathF.Abs(_movementForceModifier);
            }
        }

        private void WalkingJohn_OnUpdate() {
            NotMovingDetection();

            SetDirectionForFinalDestination();

            if(Position.Y < SCORING_Y_LIMIT && BeingPushed) {
                ChangeDirection();
            }

            if(ShouldApplyForce) {
                Body.ApplyLinearImpulse(new Vector2(JOHN_MOVEMENT_FORCE * _movementForceModifier,0));
            }

            LimitPosition();
        }

        private void LimitPosition() {
            if(Position.X < _johnMinX) {
                if(Position.Y > SCORING_Y_LIMIT) {
                    _game.ReturnJohn(this,JohnReturnType.JohnBin);
                    return;
                }
                Vector2 position = Position;
                position.X = _rightResetX;
                Position = position;
                _movementForceModifier = -MathF.Abs(_movementForceModifier);
            } else if(Position.X > _johnMaxX) {
                if(Position.Y > SCORING_Y_LIMIT) {
                    _game.ReturnJohn(this,JohnReturnType.NotJohnBin);
                    return;
                }
                Vector2 position = Position;
                position.X = _leftResetX;
                Position = position;
                _movementForceModifier = MathF.Abs(_movementForceModifier);
            }
            if(Position.Y > _johnMaxY) {
                _game.ReturnJohn(this,JohnReturnType.PitOfDoom);
            }
        }

        private bool ShouldApplyForce {
            get {
                if(_collisionList.Count > 0) {
                    /* If I am only colliding with category 2 fixtures, don't apply force. */
                    foreach(var fixture in _collisionList) {
                        if(!fixture.CollisionCategories.HasFlag(Category.Cat2)) {
                            return true;
                        }
                    }
                    return false;
                } else {
                    /* If I am midair, do not apply movement force. */
                    return false;
                }
            }
        }

        public int PoolID { get; private set; } = -1;

        public void Enable(int poolID,Vector2 position,bool movementPolarity) {
            PoolID = poolID;
            Position = position;
            Body.Enabled = true;
            Body.LinearVelocity = Vector2.Zero;
            _movementForceModifier = movementPolarity ? 1 : -1;
        }

        public void Disable() {
            Body.Enabled = false;
            Body.Position = new Vector2(-1);
            PoolID = -1;
        }

        public bool IsMoving {
            get {
                float velocityX = Math.Abs(Body.LinearVelocity.X);
                return velocityX > WALKING_ANIMATION_VELOCITY_THRESHIOLD;
            }
        }

        public bool BeingPushed {
            get {
                return ShouldApplyForce && MathF.Sign(Body.LinearVelocity.X) != MathF.Sign(_movementForceModifier);
            }
        }

        public bool AnimateWalking {
            get {
                return ShouldApplyForce && IsMoving && _collisionList.Count > 0 && !BeingPushed;
            }
        }

        public bool FacingRight => MathF.Sign(_movementForceModifier) >= 0;

        public int ConfigID { get; set; } = -1;

        private static int GetAnimationFrameOffset(TimeSpan now) {
            int frameNumber = (int)Math.Floor(now / WALKING_ANIMATION_FRAME_LENGTH);

            /* Staggered animation pattern: { 0, 1, 0, 2 } */
            return (frameNumber % WALKING_ANIMATION_FRAME_COUNT) switch { 0 => 0, 1 => 1, 2 => 0, 3 => 2, _ => 0 };
        }

        private TimeSpan _walkingStart = TimeSpan.Zero; //TODO: Set to time when walking starts

        public Rectangle GetTextureSource() {
            Point location = _game.Decorator.GetTextureOrigin(ConfigID);
            Point size = (Size * Owner.Camera.TileInputSize).ToPoint();

            if(AnimateWalking) {
                location.X += GetAnimationFrameOffset(Owner.Now - _walkingStart) * size.X;
            }

            return new Rectangle(location,size);
        }

        public SpriteEffects GetSpriteEffects() {
            return FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        private void WalkingJohn_OnRender() {
            //TODO: Offscreen filtering
            if(!Body.Enabled) {
                return;
            }
            Vector2 position = Owner.Camera.GetRenderLocation(this);

            Vector2 scale = new Vector2(Owner.Camera.Scale);

            Rectangle textureSource = GetTextureSource();
            SpriteEffects spriteEffects = GetSpriteEffects();

            Vector2 origin = textureSource.Size.ToVector2() * Origin;

            Owner.SpriteBatch.Draw(_game.Decorator.Texture,position,textureSource,Color.White,0f,origin,scale,spriteEffects,LayerDepth);
        }
    }
}
