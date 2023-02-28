namespace TwelveEngine.Shell {
    public sealed class GameLoopSyncContext:SynchronizationContext {

        /* https://github.com/vchelaru/FlatRedBall/blob/NetStandard/LICENSE */

        private readonly struct CallbackAndState {
            public readonly SendOrPostCallback Callback { get; init; }
            public readonly object State { get; init; }
        }

        private readonly Queue<CallbackAndState> _frameQueue = new(), _messageQueue = new();
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

        internal GameLoopSyncContext() {
            if(Context is not null) {
                throw new InvalidOperationException("Only one game sync context can be instantiated.");
            }
            Context = this;
            SetSynchronizationContext(this);
            Scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

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

        internal void Update() {
            lock(_messageQueueSyncLock) {
                while(_messageQueue.TryDequeue(out CallbackAndState message)) {
                    _frameQueue.Enqueue(message);
                }
            }
            while(_frameQueue.TryDequeue(out CallbackAndState message)) {
                message.Callback.Invoke(message.State);
            }
        }

        internal void Clear() {
            lock(_messageQueueSyncLock) {
                _messageQueue.Clear();
            }
        }
    }
}
