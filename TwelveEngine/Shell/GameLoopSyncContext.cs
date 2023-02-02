namespace TwelveEngine.Shell {
    public sealed class GameLoopSyncContext:SynchronizationContext {

        /* https://github.com/vchelaru/FlatRedBall/blob/NetStandard/LICENSE */

        private readonly struct CallbackAndState {
            public SendOrPostCallback Callback { get; init; }
            public object State { get; init; }
        }

        private readonly Queue<CallbackAndState> _messageQueue = new();
        private readonly object _messageQueueSyncLock = new();

        internal static TaskScheduler Scheduler { get; private set; }
        internal static GameLoopSyncContext Context { get; private set; }

        public event Action<Exception> OnTaskException;

        public static void RunTask(Func<Task> action,TaskCreationOptions options = TaskCreationOptions.None) {
            Task.Factory.StartNew(async () => {
                try {
                    await action.Invoke();
                } catch(Exception exception) {
                    Context.OnTaskException?.Invoke(exception);
                }
            },CancellationToken.None,options,Scheduler);
        }

        public GameLoopSyncContext() {
            if(Context is not null) {
                throw new InvalidOperationException("Only one game sync context can be instantiated.");
            }
            Context = this;
            SetSynchronizationContext(this);
            Scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        public override void Send(SendOrPostCallback message,object state) => throw new NotImplementedException();

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
