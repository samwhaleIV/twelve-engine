namespace TwelveEngine.Input.Routing {
    public abstract class InputEventRouter<TEvent,THandler>:IIRouter<TEvent> where THandler:IHandler<TEvent> {
        public abstract void RouteEvent(TEvent routedEvent);
    }
}
