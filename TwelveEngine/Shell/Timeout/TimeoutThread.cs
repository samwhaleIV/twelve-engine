using System;
using TwelveEngine.Shell.States;

namespace TwelveEngine.Shell.Timeout {
    public abstract class TimeoutThread {
        public InputGameState GameState { get; set; }

        private int timeout = 0;
        private Action cancellationAction = null;

        protected void SetTimeout(Action action,TimeSpan delay,Action cancel = null) {
            ClearTimeout();
            timeout = GameState.SetTimeout(action,delay);
            cancellationAction = cancel;
        }

        protected void ClearTimeout(bool fireCancellation = true) {
            if(timeout < 1) {
                return;
            }
            GameState.ClearTimeout(timeout);
            timeout = 0;

            if(fireCancellation && cancellationAction != null) {
                cancellationAction.Invoke();
                cancellationAction = null;
            }
        }
    }
}
