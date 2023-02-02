namespace TwelveEngine.Shell {
    public sealed class GameLoopSyncContext:SynchronizationContext {

        /* https://github.com/vchelaru/FlatRedBall/blob/NetStandard/LICENSE */

        private readonly struct CallbackAndState {
            public SendOrPostCallback Callback { get; init; }
            public object State { get; init; }
        }

        private readonly Queue<CallbackAndState> _messageQueue = new();
        private readonly object _messageQueueSyncLock = new();

        public GameLoopSyncContext() => SetSynchronizationContext(this);

        public override void Send(SendOrPostCallback message,object state) {
            throw new NotImplementedException();
        }

        public override void Post(SendOrPostCallback message,object state) {
            lock(_messageQueueSyncLock) {
                _messageQueue.Enqueue(new CallbackAndState() {
                    Callback = message,
                    State = state
                });
                Monitor.Pulse(_messageQueueSyncLock);
            }
        }

        private readonly Queue<CallbackAndState> frameQueue = new();
        public void Update() {
            var context = SynchronizationContext.Current;
            lock(_messageQueueSyncLock) {
                while(_messageQueue.TryDequeue(out CallbackAndState message)) {
                    frameQueue.Enqueue(message);
                }
            }
            while(frameQueue.TryDequeue(out CallbackAndState message)) {
                message.Callback.Invoke(message.State);
            }
        }

        public void Clear() {
            lock(_messageQueueSyncLock) {
                _messageQueue.Clear();
            }
        }
    }
}
