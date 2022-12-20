using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TwelveEngine.Shell.Automation {

    public sealed class AutomationAgent {
        private int frameNumber = 0;
        public int FrameNumber => frameNumber;

        private bool recording = false;
        public bool RecordingActive => recording;

        private bool playbackActive = false;
        public bool PlaybackActive => playbackActive;

        private string playbackFile = null;
        public string PlaybackFile => playbackFile;

        private bool playbackLoading = false;
        public bool PlaybackLoading => playbackLoading;

        private Queue<InputFrame> outputBuffer = null;
        private InputFrame[] playbackFrames = null;

        private InputFrame recordingFrame;
        private InputFrame playbackFrame;

        internal void StartRecording() {
            if(recording) {
                throw new InvalidOperationException("Cannot start recording, recording is already happening!");
            }
            outputBuffer = new Queue<InputFrame>();
            recording = true;
        }
        internal async Task StopRecording() {
            if(!recording) {
                throw new InvalidOperationException("Cannot stop recording, we never started!");
            }
            var path = IO.PrepareOutputPath();

            InputFrame[] frames = outputBuffer.ToArray();
            outputBuffer.Clear();
            outputBuffer = null;
            recording = false;

            await IO.WritePlaybackFrames(path,frames);
            Debug.WriteLine($"Recording saved to '{path}'.");
        }

        internal async Task StartPlayback() {
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
            Debug.WriteLine($"Playing input file '{path}'");
        }

        internal event Action PlaybackStopped, PlaybackStarted;

        internal void StopPlayback() {
            if(!playbackActive) {
                throw new InvalidOperationException("Cannot stop playback, playback is not active!");
            }
            playbackFrames = null;
            playbackActive = false;
            playbackFile = null;
            PlaybackStopped?.Invoke();
        }

        internal async void TogglePlayback() {
            if(!recording) {
                if(playbackLoading) {
                    return;
                }
                if(playbackActive) {
                    StopPlayback();
                } else {
                    await StartPlayback();
                }
            }
        }
        internal async void ToggleRecording() {
            if(!playbackActive) {
                if(recording) {
                    await StopRecording();
                } else {
                    StartRecording();
                }
            }
        }

        internal int? PlaybackFrameCount {
            get {
                if(playbackFrames == null) {
                    return null;
                } else {
                    return playbackFrames.Length;
                }
            }
        }

        internal KeyboardState FilterKeyboardState(KeyboardState state) {
            if(playbackActive) {
                state = playbackFrame.KeyboardState;
            }
            if(recording) {
                recordingFrame.KeyboardState = state;
            }
            return state;
        }
        internal MouseState FilterMouseState(MouseState state) {
            if(playbackActive) {
                state = playbackFrame.MouseState;
            }
            if(recording) {
                recordingFrame.MouseState = state;
            }
            return state;
        }

        internal void StartUpdate() {
            if(playbackActive) {
                playbackFrame = playbackFrames[frameNumber];
                frameNumber += 1;
            } else {
                frameNumber += 1;
            }
        }

        internal void EndUpdate() {
            if(recording) outputBuffer.Enqueue(recordingFrame);
            if(playbackActive && frameNumber >= playbackFrames.Length) {
                StopPlayback();
                Debug.WriteLine("Playback stopped automatically.");
            }
        }

        internal TimeSpan GetAveragePlaybackFrameTime() {
            long ticks = 0;
            int count = playbackFrames.Length;
            for(int i = 0;i < count;i++) {
                ticks += playbackFrames[i].ElapsedTime.Ticks;
            }
            return TimeSpan.FromTicks((long)Math.Floor((double)ticks / count));
        }

        internal TimeSpan GetFrameTime() => playbackFrame.ElapsedTime;
        internal void UpdateRecordingFrame(GameTime gameTime) {
            recordingFrame.ElapsedTime = gameTime.ElapsedGameTime;
        }

    }
}
