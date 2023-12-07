using System;
using Microsoft.Xna.Framework;
using TwelveEngine.Game2D.Entities;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using nkast.Aether.Physics2D.Dynamics;
using System.Collections.Generic;
using TwelveEngine;

namespace John {

    using static Constants;

    public sealed class WalkingJohn:PhysicsEntity2D {

        private readonly CollectionGame _game;

        public WalkingJohn(CollectionGame johnCollectionGame) : base(new Vector2((float)JOHN_WIDTH/johnCollectionGame.Camera.TileInputSize,1f),new Vector2(0.5f)) {
            _game = johnCollectionGame;
            Restitution = 0f;
            LinearDamping = JOHN_LINEAR_DAMPING;
            Friction = JOHN_FRICTION;
            OnUpdate += Update;
            OnRender += Render;
            OnLoad += Load;
        }

        private void Load() {
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

        private void Update() {
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
            if(Position.Y > SCORING_Y_LIMIT) {
                if(Position.X < SCORING_MARGIN_OFFSET_X) {
                    _game.ReturnJohn(this,JohnReturnType.JohnBin);
                } else if(Position.X > _game.TileMap.Width - SCORING_MARGIN_OFFSET_X) {
                    _game.ReturnJohn(this,JohnReturnType.NotJohnBin);
                } else if(Position.Y >  _game.TileMap.Height - SCORING_MARGIN_OFFSET_Y) {
                    _game.ReturnJohn(this,JohnReturnType.Default);
                }
            } else if(Position.X < RESET_MARGIN_OFFSET || Position.X > _game.TileMap.Width - RESET_MARGIN_OFFSET) {
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

        private Rectangle GetTextureSource() {
            Point location = _game.Decorator.GetTextureOrigin(ConfigID);
            Point size = (Size * Owner.Camera.TileInputSize).ToPoint();

            if(ShowWalkingAnimation) {
                location.X += GetAnimationFrameOffset(Owner.Now) * size.X;
            }

            return new Rectangle(location,size);
        }

        private SpriteEffects GetSpriteEffects() {
            return FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }
        
        public bool IsGrabbed { get; set; }

        private void Render() {
            if(!Body.Enabled && !IsGrabbed) {
                return;
            }

            if(!Owner.Camera.TryGetRenderLocation(this,out var position)) {
                return;
            }
            Rectangle textureSource = GetTextureSource();
            SpriteEffects spriteEffects = GetSpriteEffects();
            //Vector2 origin = textureSource.Size.ToVector2() * Origin;
            float layerDepth = IsGrabbed ? LayerDepth + 0.1f : LayerDepth;
            Owner.SpriteBatch.Draw(_game.Decorator.Texture,position,textureSource,Color.White,0f,Vector2.Zero,Owner.Camera.Scale,spriteEffects,layerDepth);
        }
    }
}
