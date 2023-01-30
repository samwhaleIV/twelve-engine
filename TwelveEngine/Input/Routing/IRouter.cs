namespace TwelveEngine.Input.Routing {
    public interface IIRouter<TEvent> {
        void RouteEvent(TEvent inputEvent);
    }
}
