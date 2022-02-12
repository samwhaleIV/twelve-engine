using System;
using System.IO;
using System.Text;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Linq;

namespace TwelveEngine.Shell.Automation {
    internal static class IO {
        private static InputFrame[] readFileData(BinaryReader reader) {
            var frameCount = reader.ReadInt32();

            InputFrame[] frames = new InputFrame[frameCount];
            if(frameCount <= 0) {
                return frames;
            }

            SerialInputFrame lastFrame = new SerialInputFrame();
            for(var i = 0;i < frameCount;i++) {
                var serialFrame = new SerialInputFrame(lastFrame,reader);
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

            SerialInputFrame lastFrame = new SerialInputFrame();
            for(var i = 0;i < frameCount;i++) {
                var newFrame = new SerialInputFrame(frames[i]);
                newFrame.Export(writer,lastFrame);
                lastFrame = newFrame;
            }
        }

        internal static async Task<InputFrame[]> ReadPlaybackFrames(string path) {
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
                    frames = readFileData(reader);
                }
            }

            if(frames == null) {
                frames = new InputFrame[0];
            }
            return frames;
        }

        internal static async Task WritePlaybackFrames(string path,InputFrame[] frames) {
            byte[] fileData;
            using(var stream = new MemoryStream()) {
                using(var writer = new BinaryWriter(stream,Encoding.Default,true)) {
                    writeFileData(writer,frames);
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

        internal static string GetPlaybackFile() {
            var defaultFile = Constants.Config.DefaultPlaybackFile;
            if(File.Exists(defaultFile)) {
                return defaultFile;
            }

            var file = Directory.EnumerateFiles(
                Directory.GetCurrentDirectory(),$"{Constants.Config.PlaybackFolder}\\*.{Constants.PlaybackFileExt}"
            ).OrderByDescending(name => name).FirstOrDefault();

            return file;
        }
        internal static string PrepareOutputPath() {
            var folder = Constants.Config.PlaybackFolder;
            var path = $"{folder}\\{DateTime.Now.ToFileTimeUtc()}.{Constants.PlaybackFileExt}";
            if(!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
            return path;
        }
    }
}
