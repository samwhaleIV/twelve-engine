using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TwelveEngine.Automation {

    internal sealed class AutomationAgent {

        private bool recording = false;
        private bool playback = false;

        private int frameNumber = 0;
        internal int Frame => frameNumber;

        private Queue<InputFrame> outputBuffer = null;
        private InputFrame[] playbackFrames = null;

        private InputFrame recordingFrame;
        private InputFrame playbackFrame;

        private string playbackFile = null;
        private bool playbackLoading = false;

        internal bool RecordingActive => recording;
        internal string PlaybackFile => playbackFile;
        internal bool PlaybackActive => playback;
        internal bool PlaybackLoading => playbackLoading;

        private GameTime proxyGameTime = null;

        internal void StartRecording() {
            if(recording) {
                throw new Exception("Cannot start recording, recording is already happening!");
            }
            outputBuffer = new Queue<InputFrame>();
            recording = true;
        }
        internal async Task StopRecording(string path) {
            if(!recording) {
                throw new Exception("Cannot stop recording, we never started!");
            }

            InputFrame[] frames = outputBuffer.ToArray();
            outputBuffer.Clear();
            outputBuffer = null;
            recording = false;

            await IO.WritePlaybackFrames(path,frames);
        }

        internal async Task StartPlayback(string path) {
            if(playbackLoading) {
                return;
            }
            if(playback) {
                throw new Exception("Cannot start playback, playback is already active!");
            }
            playbackLoading = true;
            playbackFrames = await IO.ReadPlaybackFrames(path);
            frameNumber = 0;
            proxyGameTime = new GameTime();
            playbackFile = path;
            playback = true;
            playbackLoading = false;
        }

        internal void StopPlayback() {
            if(!playback) {
                throw new Exception("Cannot stop playback, playback is not active!");
            }
            playbackFrames = null;
            playback = false;
            proxyGameTime = null;
            playbackFile = null;
        }

        public int? PlaybackFrameCount {
            get {
                if(playbackFrames == null) {
                    return null;
                } else {
                    return playbackFrames.Length;
                }
            }
        }

        internal KeyboardState GetKeyboardState() {
            KeyboardState state;
            if(playback) {
                state = playbackFrame.keyboardState;
            } else {
                state = Keyboard.GetState();
            }
            if(recording) {
                recordingFrame.keyboardState = state;
            }
            return state;
        }
        internal MouseState GetMouseState() {
            MouseState state;
            if(playback) {
                state = playbackFrame.mouseState;
            } else {
                state = Mouse.GetState();
            }
            if(recording) {
                recordingFrame.mouseState = state;
            }
            return state;
        }

        internal void StartUpdate() {
            if(playback) {
                playbackFrame = playbackFrames[frameNumber];
                frameNumber += 1;
            } else {
                frameNumber += 1;
            }
        }

        internal void EndUpdate() {
            if(recording) outputBuffer.Enqueue(recordingFrame);
            if(playback && frameNumber >= playbackFrames.Length) {
                StopPlayback();
                Debug.WriteLine("Playback stopped automatically.");
            }
        }

        internal GameTime GetGameTime(GameTime gameTime) {
            if(playback) {
                gameTime = proxyGameTime;
                var frame = playbackFrame;
                gameTime.ElapsedGameTime = frame.elapsedTime;
                gameTime.TotalGameTime = frame.totalTime;
            }
            if(recording) {
                recordingFrame.elapsedTime = gameTime.ElapsedGameTime;
                recordingFrame.totalTime = gameTime.TotalGameTime;
            }

            if(proxyGameTime != null) {
                return proxyGameTime;
            }
            return gameTime;
        }

    }
}
