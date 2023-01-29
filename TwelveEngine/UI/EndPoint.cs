using System;

namespace TwelveEngine.UI {
    /// <summary>
    /// Variable event parameter support. Only <c>T</c> to an arity of <c>1</c> is supported; you can fit anything you need inside of one <c>T</c>.
    /// </summary>
    public sealed class EndPoint<TReturnValue>:EndPoint {

        private readonly IEndPoint<TReturnValue> container;

        public EndPoint(IEndPoint<TReturnValue> container) => this.container = container;

        public override void Activate() => container.FireActivationEvent(container.GetEndPointValue());
    }

    /// <summary>
    /// Generic end point with no parameters.
    /// </summary>
    public class EndPoint {

        private readonly Action SendValue;

        public EndPoint() => SendValue = null;
        public EndPoint(Action sendValue) => SendValue = sendValue;

        public virtual void Activate() => SendValue.Invoke();
    }
}
