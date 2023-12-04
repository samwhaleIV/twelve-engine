using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Shell.Automation {

    public static class AutomationAgent {
        private static int frameNumber = 0;
        public static int FrameNumber => frameNumber;

        private static bool recording = false;
        public static bool RecordingActive => recording;

        private static bool playbackActive = false;
        public static bool PlaybackActive => playbackActive;

        private static string playbackFile = null;
        public static string PlaybackFile => playbackFile;

        private static bool playbackLoading = false;
        public static bool PlaybackLoading => playbackLoading;

        private static Queue<InputFrame> outputBuffer = null;
        private static InputFrame[] playbackFrames = null;

        private static InputFrame recordingFrame;
        private static InputFrame playbackFrame;

        internal static void StartRecording() {
            if(recording) {
                throw new InvalidOperationException("Cannot start recording, recording is already happening!");
            }
            outputBuffer = new Queue<InputFrame>();
            recording = true;
        }

        internal static async Task StopRecording() {
            if(!recording) {
                throw new InvalidOperationException("Cannot stop recording, we never started!");
            }
            var path = IO.PrepareOutputPath();
            if(string.IsNullOrWhiteSpace(path)) {
                return;
            }
            InputFrame[] frames = outputBuffer.ToArray();
            outputBuffer.Clear();
            outputBuffer = null;
            recording = false;

            await IO.WritePlaybackFrames(path,frames);
            Console.WriteLine($"[Automation Agent] Recording saved to '{path}'.");
        }

        internal static async Task StartPlayback() {
            if(playbackLoading) {
                return;
            }
            if(playbackActive) {
                throw new InvalidOperationException("Cannot start playback, playback is already active!");
            }
            var path = IO.GetPlaybackFile();
            if(string.IsNullOrWhiteSpace(path)) {
                return;
            }
            playbackLoading = true;
            playbackFrames = await IO.ReadPlaybackFrames(path);
            frameNumber = 0;
            playbackFile = path;
            playbackActive = true;
            playbackLoading = false;
            PlaybackStarted?.Invoke();
            Console.WriteLine($"[Automation Agent] Playing input file '{path}'");
        }

        internal static event Action PlaybackStopped, PlaybackStarted;

        internal static void StopPlayback() {
            if(!playbackActive) {
                throw new InvalidOperationException("Cannot stop playback, playback is not active!");
            }
            playbackFrames = null;
            playbackActive = false;
            playbackFile = null;
            PlaybackStopped?.Invoke();
        }

        internal static void TogglePlayback() {
            if(!recording) {
                if(playbackLoading) {
                    return;
                }
                if(playbackActive) {
                    StopPlayback();
                } else {
                    GameLoopSyncContext.RunTask(StartPlayback);
                }
            }
        }

        internal static void ToggleRecording() {
            if(!playbackActive) {
                if(recording) {
                    GameLoopSyncContext.RunTask(StopRecording);
                } else {
                    StartRecording();
                }
            }
        }

        internal static int? PlaybackFrameCount {
            get {
                if(playbackFrames == null) {
                    return null;
                } else {
                    return playbackFrames.Length;
                }
            }
        }

        internal static KeyboardState FilterKeyboardState(KeyboardState state) {
            if(playbackActive) {
                state = playbackFrame.KeyboardState;
            }
            if(recording) {
                recordingFrame.KeyboardState = state;
            }
            return state;
        }
        internal static MouseState FilterMouseState(MouseState state) {
            if(playbackActive) {
                state = playbackFrame.MouseState;
            }
            if(recording) {
                recordingFrame.MouseState = state;
            }
            return state;
        }

        internal static void StartUpdate() {
            if(playbackActive) {
                playbackFrame = playbackFrames[frameNumber];
                frameNumber += 1;
            } else {
                frameNumber += 1;
            }
        }

        internal static void EndUpdate() {
            if(recording) outputBuffer.Enqueue(recordingFrame);
            if(playbackActive && frameNumber >= playbackFrames.Length) {
                StopPlayback();
                Console.WriteLine("[Automation Agent] Playback stopped automatically.");
            }
        }

        internal static TimeSpan GetAveragePlaybackFrameTime() {
            long ticks = 0;
            int count = playbackFrames.Length;
            for(int i = 0;i < count;i++) {
                ticks += playbackFrames[i].FrameDelta.Ticks;
            }
            return TimeSpan.FromTicks((long)Math.Floor((double)ticks / count));
        }

        internal static TimeSpan GetFrameTime() => playbackFrame.FrameDelta;
        internal static void UpdateRecordingFrame(TimeSpan frameDelta) {
            recordingFrame.FrameDelta = frameDelta;
        }

    }
}
