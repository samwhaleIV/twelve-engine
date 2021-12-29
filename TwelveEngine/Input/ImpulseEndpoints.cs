using System;
using System.Collections.Generic;

namespace TwelveEngine.Input {
    public sealed partial class ImpulseHandler {

        private Dictionary<Impulse,(Action,Action)> GetEndpoints() => new Dictionary<Impulse,(Action, Action)>() {

            {Impulse.Accept,(
                () => OnAcceptDown?.Invoke(),
                () => OnAcceptUp?.Invoke()
            )},

            {Impulse.Exit,(
                () => OnExitDown?.Invoke(),
                () => OnExitUp?.Invoke()
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
        };

        public event Action OnAcceptDown;
        public event Action OnAcceptUp;

        public event Action OnExitDown;
        public event Action OnExitUp;

        public event Action<Direction> OnDirectionUp;
        public event Action<Direction> OnDirectionDown;
    }
}
