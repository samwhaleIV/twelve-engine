using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Text;
using System.Diagnostics;

namespace TwelveEngine {

    internal sealed class AutomationAgent {

        private bool recording = false;
        private bool playback = false;

        private int frameNumber = 0;
        internal int Frame => frameNumber;

        private Queue<InputFrame> outputBuffer = null;
        private InputFrame[] playbackFrames = null;

        private InputFrame recordingFrame = new InputFrame();
        private InputFrame playbackFrame = new InputFrame();

        internal bool RecordingActive => recording;
        internal bool PlaybackActive => playback;

        internal void StartRecording() {
            if(recording) {
                throw new Exception("Cannot start recording, recording is already happening!");
            }
            outputBuffer = new Queue<InputFrame>();
            recording = true;
        }
        internal void StopRecording(string path) {
            if(!recording) {
                throw new Exception("Cannot stop recording, we never started!");
            }

            var stringBuilder = new StringBuilder();

            InputFrame frame;
            while(outputBuffer.TryDequeue(out frame)) {
                new SerialInputFrame(frame).Export(stringBuilder);
            }

            outputBuffer = null;
            File.WriteAllText(path,stringBuilder.ToString());
            recording = false;
        }

        private string playbackFile = null;
        internal string PlaybackFile => playbackFile;

        internal void StartPlayback(string path) {
            if(playback) {
                throw new Exception("Cannot start playback, playback is already active!");
            }
            var lines = File.ReadAllLines(path);
            playbackFrames = new InputFrame[lines.Length];

            for(var i = 0;i < playbackFrames.Length;i++) {
                playbackFrames[i] = new InputFrame(new SerialInputFrame(lines[i]));
            }
            frameNumber = 0;
            proxyGameTime = new GameTime();
            playbackFile = path;
            playback = true;
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

        internal void StartFrame() {
            if(playback) {
                playbackFrame = playbackFrames[frameNumber];
                frameNumber += 1;
            } else {
                frameNumber += 1;
            }
        }

        internal void EndFrame() {
            if(recording) outputBuffer.Enqueue(recordingFrame);
            if(playback && frameNumber >= playbackFrames.Length) {
                StopPlayback();
                Debug.WriteLine("Playback stopped automatically.");
            }
        }

        private GameTime proxyGameTime = null;

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
