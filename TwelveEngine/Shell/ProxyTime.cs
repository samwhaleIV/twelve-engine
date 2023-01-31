using System.Diagnostics;

namespace TwelveEngine.Shell {
    internal static class ProxyTime {

        private static readonly TimeSpan MAX_ELAPSED_TIME = TimeSpan.FromMilliseconds(Constants.MaxFrameDelta);

        private static bool _shouldAddPauseTime = false;

        private static readonly Stopwatch stopwatch = new();

        internal static void Start() {
            stopwatch.Start();
        }

        /// <summary>
        /// Obtain the realest time there is, directly from the stopwatch. Does not subtract internal paused time or MonoGame paused time.
        /// </summary>
        /// <returns>Total, elapsed duration since <see cref="Start"/> was first called.</returns>
        internal static TimeSpan GetElapsedTime() => stopwatch.Elapsed;

        private static void UpdateFrameDelta(TimeSpan frameDelta) {
            if(Config.GetBool(Config.Keys.LimitFrameDelta) && frameDelta > MAX_ELAPSED_TIME) {
                Console.WriteLine($"[WARNING] Exceeded frame delta limit: {frameDelta} > {MAX_ELAPSED_TIME}");
                frameDelta = MAX_ELAPSED_TIME;
            }
            FrameDelta = frameDelta;
        }

        /// <summary>
        /// Total duration that the engine has been paused. Does not a reflect window/MonoGame hang. See <see cref="Drift"/>.
        /// </summary>
        internal static TimeSpan PauseTime { get; private set; } = TimeSpan.Zero;

        /// <summary>
        /// Elapsed time delta provided by MonoGame. Filtered against <see cref="Constants.MaxFrameDelta"/> if engine config allows for it. See <see cref="Config.Keys.LimitFrameDelta"/>.
        /// </summary>
        internal static TimeSpan FrameDelta { get; private set; }

        /// <summary>
        /// Total duration. Realtime, minus paused game duration. Not affected by <see cref="Now"/> lag.
        /// </summary>
        internal static TimeSpan RealTime { get; private set; }

        /// <summary>
        /// Total duration. The game time provided by MonoGame minus paused game duration.
        /// </summary>
        internal static TimeSpan Now { get; private set; }

        /// <summary>
        /// The difference between <c>Now</c> and <c>GameTime</c>.<br/>
        /// E.g. MonoGame pauses the game loop while moving the window.<br/>
        /// This makes <see cref="Now"/> unsuitable for long, timelined events.
        /// </summary>
        internal static TimeSpan Drift => RealTime - Now;

        /// <summary>
        /// Indicates if time updating is paused. <see cref="FrameDelta"/> continues to update while paused.
        /// </summary>
        internal static bool IsPaused => _pauseCount > 0;

        internal static void Update(GameTime gameTime) {
            TimeSpan now = GetElapsedTime();

            if(_shouldAddPauseTime) {
                AddPauseTime(now - PauseTime - RealTime);
                _shouldAddPauseTime = false;
            }

            if(gameTime is not null) {
                UpdateFrameDelta(gameTime.ElapsedGameTime);
            }

            if(IsPaused) {
                return;
            }

            /* If gameTime is null, the automation agent is overriding the event loop. Time is being modified by 'AddSimulationTime'. */
            if(gameTime is null) {
                return;
            }
            RealTime = now - PauseTime;
            Now = gameTime.TotalGameTime - PauseTime;
        }

        internal static void AddPauseTime(TimeSpan pauseTime) {
            PauseTime += pauseTime;
        }

        private static int _pauseCount = 0;

        internal static void Pause() {
            _pauseCount += 1;
        }

        internal static void Resume() {
            if(_pauseCount <= 0) {
                _pauseCount = 0;
                return;
            }
            if(_pauseCount == 1) {
                _shouldAddPauseTime = true;
                _pauseCount = 0;
                return;
            }
            _pauseCount -= 1;
        }

        /// <summary>
        /// Add simulation time if the game is being updated outside of the primary update path. I.e. when using <see cref="Automation.AutomationAgent"/>.
        /// </summary>
        /// <param name="frameDelta">The delta of the frame that is being simulated.</param>
        internal static void AddSimulationTime(TimeSpan frameDelta) {
            FrameDelta = frameDelta;
            RealTime += frameDelta;
            Now += frameDelta;
        }
    }
}
