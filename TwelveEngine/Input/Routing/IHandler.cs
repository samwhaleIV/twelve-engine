using System;

namespace TwelveEngine.Input.Routing {
    public interface IHandler<TEvent> {
        event Action<TEvent> OnEvent;
    }
}
