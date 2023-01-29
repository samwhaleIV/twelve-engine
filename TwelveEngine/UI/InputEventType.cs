namespace TwelveEngine.UI {
    public enum InputEventType {
        /// <summary>
        /// Send a mouse update every frame, even if the mouse position has not changed.
        /// </summary>
        MouseUpdate,
        /// <summary>
        /// The left mouse button has been pressed. Used for selected element pressed state.
        /// </summary>
        MousePressed,
        /// <summary>
        /// The left mouse button has been released. Used for selected element activation.
        /// </summary>
        MouseReleased,
        /// <summary>
        /// A direction key has been pressed.
        /// </summary>
        DirectionImpulse,
        /// <summary>
        /// The accept key has been pressed. Used for selected element pressed state.
        /// </summary>
        AcceptPressed,
        /// <summary>
        /// The accept key has been released. Used for selected element activation.
        /// </summary>
        AcceptReleased,
        /// <summary>
        /// A cancel button has been pressed. Used for context aware back button/pagination.
        /// </summary>
        BackButtonActivated,
        /// <summary>
        /// A focus button that advances the focus queue to the right. The tab button, in most traditional applications.
        /// </summary>
        FocusButtonActivated
    }
}
