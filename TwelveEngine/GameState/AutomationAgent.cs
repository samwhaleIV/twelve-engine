using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Text;
using System.Diagnostics;
using System.IO.Compression;
using System.Threading.Tasks;

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
        internal async Task StopRecording(string path) {
            if(!recording) {
                throw new Exception("Cannot stop recording, we never started!");
            }

            InputFrame[] frames = outputBuffer.ToArray();
            outputBuffer.Clear();
            outputBuffer = null;
            recording = false;

            await writePlaybackFrames(path,frames);
        }

        private string playbackFile = null;
        internal string PlaybackFile => playbackFile;

        private static InputFrame[] readFileData(BinaryReader reader) {
            var frameCount = reader.ReadInt32();

            InputFrame[] frames = new InputFrame[frameCount];
            if(frameCount <= 0) {
                return frames;
            }
            long totalTime = reader.ReadInt64();

            SerialInputFrame lastFrame = new SerialInputFrame();
            for(var i = 0;i < frameCount;i++) {
                var serialFrame = new SerialInputFrame(ref totalTime,lastFrame,reader);
                frames[i] = new InputFrame(serialFrame);
                lastFrame = serialFrame;
            }
            return frames;
        }

        private static void writeFileData(BinaryWriter writer,InputFrame[] frames) {
            var frameCount = frames.Length;

            writer.Write(frameCount);

            if(frameCount <= 0) {
                return;
            }

            var firstFrame = frames[0];
            writer.Write((firstFrame.totalTime - firstFrame.elapsedTime).Ticks);

            SerialInputFrame lastFrame = new SerialInputFrame();

            for(var i = 0;i < frameCount;i++) {
                var newFrame = new SerialInputFrame(frames[i]);
                newFrame.Export(writer,lastFrame);
                lastFrame = newFrame;
            }
        }

        private static async Task<InputFrame[]> readPlaybackFrames(string path) {
            InputFrame[] frames = null;

            byte[] fileData;
            using(var stream = new MemoryStream()) {
                using(var fileStream = File.Open(path,FileMode.Open,FileAccess.Read,FileShare.Read)) {
                    await fileStream.CopyToAsync(stream);
                }
                fileData = stream.ToArray();
            }

            using(var stream = new MemoryStream()) {
                using(var compressStream = new MemoryStream(fileData)) {
                    using(var deflateStream = new DeflateStream(compressStream,CompressionMode.Decompress)) {
                        await deflateStream.CopyToAsync(stream);
                    }
                }
                using(var reader = new BinaryReader(stream,Encoding.Default,false)) {
                    stream.Seek(0,SeekOrigin.Begin);
                    frames = await Task.Run(() => readFileData(reader));
                }
            }

            if(frames == null) {
                frames = new InputFrame[0];
            }
            return frames;
        }

        private static async Task writePlaybackFrames(string path,InputFrame[] frames) {
            byte[] fileData;
            using(var stream = new MemoryStream()) {
                using(var writer = new BinaryWriter(stream,Encoding.Default,true)) {
                    await Task.Run(() => writeFileData(writer,frames));
                }
                fileData = stream.ToArray();
            }

            using(var stream = new MemoryStream()) {
                using(var compressor = new DeflateStream(stream,CompressionMode.Compress)) {
                    await compressor.WriteAsync(fileData,0,fileData.Length);
                }
                fileData = stream.ToArray();
            }

            using(var stream = File.Open(path,FileMode.Create,FileAccess.Write,FileShare.None)) {
                await stream.WriteAsync(fileData,0,fileData.Length);
            }
        }

        internal bool playbackLoading = false;
        public bool PlaybackLoading => playbackLoading;

        internal async Task StartPlayback(string path) {
            if(playbackLoading) {
                return;
            }
            if(playback) {
                throw new Exception("Cannot start playback, playback is already active!");
            }
            playbackLoading = true;
            playbackFrames = await readPlaybackFrames(path);
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
