using System;
using System.Collections.Generic;

namespace TwelveEngine.Shell.Input {
    public sealed partial class InputHandler {

        private Dictionary<Impulse,(Action,Action)> GetEndpoints() => new Dictionary<Impulse,(Action, Action)>() {

            {Impulse.Accept,(
                () => OnAcceptDown?.Invoke(),
                () => OnAcceptUp?.Invoke()
            )},

            {Impulse.Cancel,(
                () => OnCancelDown?.Invoke(),
                () => OnCancelUp?.Invoke()
            )},

            {Impulse.Up,(
                () => OnDirectionDown?.Invoke(Direction.Up),
                () => OnDirectionUp?.Invoke(Direction.Up)
            )},

            {Impulse.Down,(
                () => OnDirectionDown?.Invoke(Direction.Down),
                () => OnDirectionUp?.Invoke(Direction.Down)
            )},

            {Impulse.Left,(
                () => OnDirectionDown?.Invoke(Direction.Left),
                () => OnDirectionUp?.Invoke(Direction.Left)
            )},

            {Impulse.Right,(
                () => OnDirectionDown?.Invoke(Direction.Right),
                () => OnDirectionUp?.Invoke(Direction.Right)
            )},

            {Impulse.Toggle,(
                () => OnToggleDown?.Invoke(),
                () => OnToggleUp?.Invoke()
            )},
        };

        public event Action OnToggleDown;
        public event Action OnToggleUp;

        public event Action OnAcceptDown;
        public event Action OnAcceptUp;

        public event Action OnCancelDown;
        public event Action OnCancelUp;

        public event Action<Direction> OnDirectionUp;
        public event Action<Direction> OnDirectionDown;
    }
}
