using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace TwelveEngine {

    public static class Logger {

        private static StreamWriter streamWriter = null;
        private static readonly StringBuilder stringBuilder = new();

        public static string Path { get; private set; } = null;

        public static void CleanUp() {
            WriteLine($"[{DateTime.UtcNow} UTC] Game is exiting.");
            _autoFlush = false;
            if(!HasStreamWriter) {
                return;
            }
            if(!TryFlush()) {
                return;
            }
            ClearStreamWriter();
        }

        private static bool TryCreateStreamWriter(string path) {
            Path = path;
            bool fileExists = File.Exists(path);
            bool addNewLine = fileExists;
            try {
                if(fileExists) {
                    var info = new FileInfo(path);
                    if(info.Length > Constants.LogResetLimit) {
                        streamWriter = File.CreateText(path);
                        addNewLine = false;
                    } else {
                        streamWriter = File.AppendText(path);
                    }
                } else {
                    streamWriter = File.CreateText(path);
                    addNewLine = false;
                }
            } catch(Exception exception) {
                streamWriter = null;
                Console.WriteLine($"[Logger] Failure creating log stream: {exception}");
                return false;
            }
            if(addNewLine && HasStreamWriter) {
                /* Only writes a new line to the file already existed before we append to it. Makes logging sessions blank line seperated */
                streamWriter.WriteLine();
            }
            return HasStreamWriter;
        }

#if DEBUG
        [DllImport("kernel32.dll",SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();
#endif
        public static void Initialize(string path) {
#if DEBUG
            AllocConsole();
#endif
            if(TryCreateStreamWriter(path)) {
                WriteLine($"[{DateTime.UtcNow} UTC] Game started.");
            } else {
                WriteLine($"[{DateTime.UtcNow} UTC] Game started with no log file.");
            }
        }

        private static bool _autoFlush = false;

        public static bool HasStreamWriter => streamWriter != null;

        public static bool AutoFlush {
            get => _autoFlush;
            set {
                if(value == _autoFlush) {
                    return;
                }
                if(HasStreamWriter && _autoFlush) {
                    TryFlush();
                }
                _autoFlush = value;
            }
        }

        public static void WriteLine(string line) {
            Console.WriteLine(line);
            if(!HasStreamWriter) {
                return;
            }
            streamWriter.WriteLine(line);
            if(_autoFlush) {
                TryFlush();
            }
        }

        public static void WriteLine(StringBuilder line) {
            Console.WriteLine(line);
            if(!HasStreamWriter) {
                return;
            }
            streamWriter.WriteLine(line);
            if(_autoFlush) {
                TryFlush();
            }
        }

        public static void Write(string text,bool checkAutoFlush = false) {
            Console.Write(text);
            if(!HasStreamWriter) {
                return;
            }
            streamWriter.Write(text);
            if(checkAutoFlush && _autoFlush) {
                TryFlush();
            }
        }

        public static void Write(StringBuilder text,bool checkAutoFlush = false) {
            Console.Write(text);
            if(!HasStreamWriter) {
                return;
            }
            streamWriter.Write(text);
            if(checkAutoFlush && _autoFlush) {
                TryFlush();
            }
        }

        public static void WriteBooleanSet(string label,string[] names,bool[] values) {
            if(names.Length != values.Length) {
                throw new ArgumentException("Bad boolean set, names and values must be equal in length.");
            }
            if(names.Length == 0) {
                return;
            }
            var sb = stringBuilder;
            sb.Clear();
            sb.Append(label);
            sb.Append(": ");
            for(int i = 0;i<names.Length;i++) {
                sb.Append(names[i]);
                sb.Append(" = ");
                sb.Append(values[i] ? "Yes" : "No");
                sb.Append(" | ");
            }
            sb.Remove(stringBuilder.Length - 3, 3);
            WriteLine(sb);
        }

        private static void ClearStreamWriter() {
            if(!HasStreamWriter) {
                return;
            }
            Console.WriteLine("[Logger] Logging stream terminated. Logging is no longer reflected in file.");
            streamWriter.Dispose();
            streamWriter = null;
        }

        private static bool TryFlush() {
            try {
                streamWriter.Flush();
                return true;
            } catch(Exception exception) {
                Console.WriteLine($"[Logger] Failure flushing log stream. This will not be reflected in file. Exception: {exception}");
                ClearStreamWriter();
                return false;
            }
        }

        public static void Flush() {
            if(!HasStreamWriter) {
                return;
            }
            TryFlush();
        }
    }
}
