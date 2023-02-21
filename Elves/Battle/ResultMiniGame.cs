using System;
using System.Threading.Tasks;

namespace Elves.Battle {
    public abstract class ResultMiniGame<TResult>:MiniGame {

        private TaskCompletionSource<TResult> _taskCompletionSource = null;

        protected void SetResult(TResult value) {
            if(_taskCompletionSource is null) {
                return;
            }
            try {
                if(!_taskCompletionSource.TrySetResult(value)) {
                    throw new MiniGameException("Failure setting result for mini game, try set result failed.");
                }
            } catch(ObjectDisposedException exception) {
                throw new MiniGameException("Failure setting result for mini game, task completion source has already been disposed.",exception);
            }
            _taskCompletionSource = null;
        }

        public async Task<TResult> GetResult() {
            if(_taskCompletionSource is not null) {
                return await _taskCompletionSource.Task;
            }
            _taskCompletionSource = new();
            return await _taskCompletionSource.Task;
        }
    }
}
