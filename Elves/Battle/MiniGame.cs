using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine;
using TwelveEngine.Shell;

namespace Elves.Battle {
    public abstract class MiniGame {

        public abstract CursorState Update(Vector2 mousePosition,bool isPressed);
        public abstract void Render(SpriteBatch spriteBatch,int width,int height);

        public InputGameState GameState { get; private set; }
        protected TimeSpan StartTime { get; private set; } = TimeSpan.Zero;

        public TimeSpan Now => (GameState?.Now ?? TimeSpan.Zero) - StartTime;

        protected bool MouseOnScreen() {
            var mousePosition = GameState.VirtualMouseProvider.VirtualMousePositionNormal;
            return mousePosition.X >= 0 && mousePosition.X < 1 && mousePosition.Y >= 0 && mousePosition.Y < 1;
        }

        public CursorState Update() {
            return Update(GameState.VirtualMouseProvider.VirtualMousePositionNormal,GameState.VirtualMouseProvider.VirtualMouseIsPressed);
        }

        public void UpdateState(InputGameState gameState) {
            GameState = gameState;
            StartTime = gameState?.Now ?? TimeSpan.Zero;
        }

        public virtual void UpdateBounds(Rectangle bounds) {
            GameState.VirtualMouseProvider.Bounds = bounds;
        }

        public bool IsActive { get; private set; }

        protected event Action OnActivated, OnDeactivated;

        public void Activate() {
            IsActive = true;
            GameState.VirtualMouseProvider.Enable(GameState.Viewport.Bounds.Center.ToVector2());
            OnActivated?.Invoke();
        }
        public void Deactivate() {
            IsActive = false;
            GameState.VirtualMouseProvider.Disable();
            OnDeactivated?.Invoke();
        }

        public Color ClearColor { get; set; } = Color.FromNonPremultiplied(242,242,242,byte.MaxValue);
    }
}
