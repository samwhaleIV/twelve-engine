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

        public WalkingJohn(JohnCollectionGame johnCollectionGame) : base(new Vector2(9/16f,1f),new Vector2(0.5f)) {
            _game = johnCollectionGame;
            OnRender += WalkingJohn_OnRender;
            OnUpdate += WalkingJohn_OnUpdate;
            OnLoad += WalkingJohn_OnLoad;

            Restitution = 0f;
            LinearDamping = JOHN_LINEAR_DAMPING;
            Friction = JOHN_FRICTION;
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

        private void LandedOnSurface() {
            if(_game.GetRandomBool()) {
                return;
            }
            ChangeDirection();
        }

        private bool _hasCollisionWithWorld = false;

        private bool Body_OnCollision(Fixture sender,Fixture other,Contact contact) {
            _collisionList.Add(other);

            bool hadCollisionWithWorld = _hasCollisionWithWorld;
            _hasCollisionWithWorld = false;
            foreach(var fixture in _collisionList) {
                if(fixture.CollisionCategories == Category.Cat1) {
                    _hasCollisionWithWorld = true;
                }
            }

            if(!hadCollisionWithWorld && _hasCollisionWithWorld) {
                LandedOnSurface();
            }
            return true;
        }

        private TimeSpan _lastDirectionChangeTime = TimeSpan.Zero;
        private static readonly TimeSpan _directionChangeTimeout = TimeSpan.FromSeconds(0.3f);

        private void ChangeDirection() {
            if(Now - _lastDirectionChangeTime >  _directionChangeTimeout) {
                _movementForceModifier = -_movementForceModifier;
                _lastDirectionChangeTime = Now;
            }
        }

        private float _movementForceModifier = 1f;

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

        private TimeSpan _notBeingPushedStart = TimeSpan.Zero;
        private static readonly TimeSpan _pushingHoldDuration = TimeSpan.FromSeconds(0.125f);

        private void WalkingJohn_OnUpdate() {
            if(!Body.Enabled) {
                return;
            }
            SetDirectionForFinalDestination();

            if(!(BeingPushed || ShouldApplyForce && !IsMovingReasonably)) {
                _notBeingPushedStart = Now;
            } else if(Now - _notBeingPushedStart > _pushingHoldDuration) {
                ChangeDirection();
                _notBeingPushedStart = Now;
            }

            if(ShouldApplyForce) {
                Body.ApplyLinearImpulse(new Vector2(JOHN_MOVEMENT_FORCE * _movementForceModifier,0));
            }

            LimitPosition();
        }

        private void LimitPosition() {
            if(Position.X < 1) {
                if(Position.Y > SCORING_Y_LIMIT) {
                    _game.ReturnJohn(this,JohnReturnType.JohnBin);
                } else {
                    _game.ReturnJohn(this,JohnReturnType.Default);
                }
            } else if(Position.X > _game.TileMap.Width - 1) {
                if(Position.Y > SCORING_Y_LIMIT) {
                    _game.ReturnJohn(this,JohnReturnType.NotJohnBin);
                } else {
                    _game.ReturnJohn(this,JohnReturnType.Default);
                }
            } else if(Position.Y > _game.TileMap.Height - 1) {
                _game.ReturnJohn(this,JohnReturnType.Default);
            }
        }

        private bool ShouldApplyForce {
            get {
                if(_collisionList.Count > 0) {
                    /* If I am colliding with only Cat2 fixtures who are below me, do not apply force. */
                    foreach(var fixture in _collisionList) {
                        if(!(fixture.CollisionCategories.HasFlag(Category.Cat2) && fixture.Body.Position.Y > Position.Y)) {
                            return true;
                        }
                    }
                    return false;
                } else {
                    /* If I am midair, do not apply force. */
                    return false;
                }
            }
        }

        public int PoolID { get; private set; } = -1;

        public void Enable(int poolID,JohnStartPosition startPosition) {
            PoolID = poolID;
            Position = startPosition.Value;
            Body.Enabled = true;
            Body.LinearVelocity = Vector2.Zero;
            float movementPolarity = startPosition.Direction switch {
                JohnStartPositionDirection.FacingLeft => -1,
                JohnStartPositionDirection.FacingRight => 1,
                _ => _game.GetRandomBool() ? 1 : -1,
            };
            _movementForceModifier = MathF.Abs(_movementForceModifier) * movementPolarity;
            _lastDirectionChangeTime = Now + _directionChangeTimeout;
        }

        public void Disable() {
            Body.Enabled = false;
            Body.Position = new Vector2(-1);
            PoolID = -1;
        }

        private  bool IsMovingReasonably {
            get {
                return Math.Abs(Body.LinearVelocity.X) > WALKING_VELOCITY_DETECTION_THRESHOLD;
            }
        }

        private bool OnGround {
            get {
                foreach(var fixture in _collisionList) {
                    if(fixture.CollisionCategories == Category.Cat1) {
                        return true;
                    }
                }
                return false;
            }
        }

        private bool BeingPushed {
            get {
                return IsMovingReasonably && OnGround && MathF.Sign(Body.LinearVelocity.X) != MathF.Sign(_movementForceModifier);
            }
        }

        private bool ShowWalkingAnimation {
            get {
                return ShouldApplyForce && IsMovingReasonably && _collisionList.Count > 0 && !BeingPushed;
            }
        }

        private bool FacingRight => MathF.Sign(_movementForceModifier) >= 0;

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

            if(ShowWalkingAnimation) {
                location.X += GetAnimationFrameOffset(Owner.Now - _walkingStart) * size.X;
            }

            return new Rectangle(location,size);
        }

        public SpriteEffects GetSpriteEffects() {
            return FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }
        
        public bool IsGrabbed { get; set; }

        private void WalkingJohn_OnRender() {
            //TODO: Offscreen filtering
            if(!Body.Enabled && !IsGrabbed) {
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
