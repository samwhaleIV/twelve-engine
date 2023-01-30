using System;

namespace TwelveEngine.UI {
    /// <summary>
    /// Variable event parameter support. Only <c>TReturnValue</c> to an arity of <c>1</c> is supported; you can fit anything you need inside of one value.
    /// </summary>
    public sealed class Endpoint<TReturnValue>:Endpoint {

        private readonly IEndpoint<TReturnValue> container;

        public Endpoint(IEndpoint<TReturnValue> container) => this.container = container;

        public override void Activate() => container.FireActivationEvent(container.GetEndPointValue());
    }

    /// <summary>
    /// Generic end point with no parameters.
    /// </summary>
    public class Endpoint {

        private readonly Action SendValue;

        public Endpoint() => SendValue = null;
        public Endpoint(Action sendValue) => SendValue = sendValue;

        public virtual void Activate() => SendValue.Invoke();
    }
}
