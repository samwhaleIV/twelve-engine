using System;

namespace TwelveEngine.Input.Routing {
    public abstract class InputEventHandler<TEvent,TRouter>:IHandler<TEvent> where TRouter:IIRouter<TEvent>, new() {
        public event Action<TEvent> OnEvent;
        public void SendEvent(TEvent @event) => OnEvent?.Invoke(@event);

        private TRouter CreateRouter() {
            var router = new TRouter();
            OnEvent += router.RouteEvent;
            return router;
        }

        private TRouter _router = default;
        public TRouter Router {
            get {
                if(_router is null) {
                    _router = CreateRouter();
                }
                return _router;
            }
        }
    }
}
