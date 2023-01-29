using System;

namespace TwelveEngine.UI {
    public interface IEndPoint<TReturnValue> {
        public TReturnValue GetEndPointValue();
        public void FireActivationEvent(TReturnValue value);
        public event Action<TReturnValue> OnActivated;
    }
}
