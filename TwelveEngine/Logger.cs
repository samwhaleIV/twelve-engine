using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace TwelveEngine {

    public static class Logger {

        private static StreamWriter streamWriter = null;
        private static readonly StringBuilder stringBuilder = new();

        public static void CleanUp() {
            _autoFlush = false;
            Flush();
            streamWriter?.Dispose();
            streamWriter = null;
        }

        static Logger() {
#if DEBUG
            AllocConsole();
#endif
            bool logFileExists = File.Exists(Constants.LogFile);
            try {
                streamWriter = File.AppendText(Constants.LogFile);
            } catch(Exception exception) {
                streamWriter = null;
                WriteLine(exception.ToString());
            }
            if(logFileExists && streamWriter != null) {
                streamWriter.WriteLine();
            }
            WriteLine($"Logger started, current time (UTC): {DateTime.UtcNow}");
        }
#if DEBUG
        [DllImport("kernel32.dll",SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();
#endif

        private static bool _autoFlush = false;

        public static bool AutoFlush {
            get => _autoFlush;
            set {
                if(value == _autoFlush) {
                    return;
                }
                if(_autoFlush) {
                    Flush();
                }
                _autoFlush = value;
            }
        }

        public static void WriteLine(string line) {
            Console.WriteLine(line);
            if(streamWriter == null) {
                return;
            }
            streamWriter.WriteLine(line);
            if(_autoFlush) {
                streamWriter.Flush();
            }
        }

        public static void WriteLine(StringBuilder line) {
            Console.WriteLine(line);
            if(streamWriter == null) {
                return;
            }
            streamWriter.WriteLine(line);
            if(_autoFlush) {
                streamWriter.Flush();
            }
        }

        public static void Write(string text,bool checkAutoFlush = false) {
            Console.Write(text);
            if(streamWriter == null) {
                return;
            }
            streamWriter.Write(text);
            if(checkAutoFlush && _autoFlush) {
                streamWriter.Flush();
            }
        }

        public static void Write(StringBuilder text,bool checkAutoFlush = false) {
            Console.Write(text);
            if(streamWriter == null) {
                return;
            }
            streamWriter.Write(text);
            if(checkAutoFlush && _autoFlush) {
                streamWriter.Flush();
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

        public static void Flush() {
            if(streamWriter == null) {
                return;
            }
            streamWriter.Flush();
        }
    }
}
