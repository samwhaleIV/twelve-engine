using TwelveEngine.Shell;

namespace TwelveEngine.Input {
    public sealed class VirtualMouseProvider {

        private const int HARDWARE_TRANSITION_LATENCY = 2;
        private const float PIXELS_PER_SECOND = 800;

        private readonly InputGameState _gameState;
        public VirtualMouseProvider(InputGameState gameState) {
            _gameState = gameState;
        }

        private bool _virtualMouseEnabled = false;

        public void Enable(Vector2 defaultPosition) {
            if(_virtualMouseEnabled) {
                return;
            }
            _virtualMouseEnabled = true;
            if(InputStateCache.LastInputEventWasFromMouse) {
                VirtualMousePosition = _gameState.Mouse.Position.ToVector2();
                RealMouseIsActive = true;
            } else {
                VirtualMousePosition = defaultPosition;
                RealMouseIsActive = false;
            }
        }

        public void Disable() {
            if(!_virtualMouseEnabled) {
                return;
            }
            _virtualMouseEnabled = false;
            _realMouseIsActive = false;
            VirtualMouseIsPressed = false;
        }

        private bool _realMouseIsActive = false;

        public bool RealMouseIsActive {
            get => _realMouseIsActive;
            private set {
                if(_realMouseIsActive == value) {
                    return;
                }
                _realMouseIsActive = value;
                if(value) {
                    /* Transition from software coordinate to real coordinate */
                    return;
                }
                /* Transition from real coordinate to software coordinate */
                VirtualMousePosition = _gameState.Mouse.Position.ToVector2();
            }
        }

        public Vector2 VirtualMousePosition { get; private set; }
        public Vector2 VirtualMousePositionNormal {
            get {
                var position = VirtualMousePosition;
                if(!Bounds.HasValue) {
                    return position;
                }
                var bounds = Bounds.Value;
                position -= bounds.Location.ToVector2();
                position /= bounds.Size.ToVector2();
                return position;
            }
        }
        public bool VirtualMouseIsPressed { get; private set; }
        public bool? HiddenState { get; private set; }

        private bool? GetHiddenState() {
            if(!_virtualMouseEnabled && _hideCursor <= 0 && _forceSoftwareCursor <= 0) {
                return null;
            }
            return _hiddenState;
        }

        private bool _hiddenState = false;

        private void UpdateFromRealMouse() {
            VirtualMousePosition = _gameState.Mouse.Position.ToVector2();
            VirtualMouseIsPressed = _gameState.Mouse.CapturingLeft;
        }

        private void UpdateFromAlternateHardware() {
            VirtualMousePosition = GetVirtualPosition();
            VirtualMouseIsPressed = _gameState.Impulse.IsImpulseDown(Impulse.Accept);
        }

        public void Update() {
            UpdateFrameCountEvents();
            if(InputStateCache.LastInputEventWasFromMouse) {
                RealMouseIsActive = true;
                if(_virtualMouseEnabled) {
                    UpdateFromRealMouse();
                }
            } else {
                RealMouseIsActive = false;
                if(_virtualMouseEnabled) {
                    UpdateFromAlternateHardware();
                }
            }
            bool hardwareCursorActive = !_hiddenState && _hideCursor <= 0;
            if(!_realMouseIsActive && hardwareCursorActive) {
                _hideCursor = HARDWARE_TRANSITION_LATENCY;
            }
            if(_realMouseIsActive && !hardwareCursorActive) {
                _hiddenState = false;
                if(_virtualMouseEnabled) {
                    _forceSoftwareCursor = HARDWARE_TRANSITION_LATENCY;
                }
            }
            HiddenState = GetHiddenState();
        }

        private void RenderVirtualMouse(SpriteBatch spriteBatch,CursorState cursorState) {
            if(!CustomCursor.Sources.TryGetValue(cursorState,out var source)) {
                return;
            }
            Texture2D cursorTexture = source.Texture;
            spriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);
            Vector2 mousePosition = VirtualMousePosition;
            Rectangle cursorArea = cursorTexture.Bounds;
            spriteBatch.Draw(cursorTexture,mousePosition,cursorArea,Color.White,0f,cursorArea.Size.ToVector2()*0.5f,Vector2.One,SpriteEffects.None,1f);
            spriteBatch.End();
        }

        /* Frame latencies for the cursor state modifications to take effect */
        private int _hideCursor = -1, _forceSoftwareCursor = -1;

        private void UpdateFrameCountEvents() {
            if(_hideCursor > 0) {
                _hideCursor--;
                if(_hideCursor == 0) {
                    _hiddenState = true;
                }
            }
            if(_forceSoftwareCursor > 0) {
                _forceSoftwareCursor--;
            }
        }

        internal void TryRenderVirtualMouse(SpriteBatch spriteBatch,CursorState cursorState) {
            if(!(_virtualMouseEnabled && !_realMouseIsActive || _forceSoftwareCursor > 0)) {
                return;
            }
            RenderVirtualMouse(spriteBatch,cursorState);
        }

        public Rectangle? Bounds { get; set; } = null;

        private Vector2 GetVirtualPositionDelta() {
            return _gameState.Impulse.GetDelta2D() * (float)_gameState.FrameDelta.TotalSeconds * PIXELS_PER_SECOND;
        }

        private Vector2 GetVirtualPosition() {
            var delta = GetVirtualPositionDelta();
            var position = VirtualMousePosition + delta;
            if(Bounds is null) {
                return position;
            }
            var bounds = Bounds.Value;
            if(position.X < bounds.Left) {
                position.X = bounds.Left;
            } else if(position.X > bounds.Right) {
                position.X = bounds.Right;
            }
            if(position.Y < bounds.Top) {
                position.Y = bounds.Top;
            } else if(position.Y > bounds.Bottom) {
                position.Y = bounds.Bottom;
            }
            return position;
        }
    }
}
