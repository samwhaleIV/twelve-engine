using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace TwelveEngine {

    public static class Logger {

        private static StreamWriter streamWriter = null;

        public static void CleanUp() {
            AutoFlush = false;
            WriteLine();
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
                WriteLine(exception.ToString());
            }
            if(logFileExists) {
                streamWriter.WriteLine();
            }
            WriteLine($"Logger started, current time (UTC): {DateTime.UtcNow}");
        }
#if DEBUG
        [DllImport("kernel32.dll",SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();
#endif

        public static bool AutoFlush { get; set; } = false;

        public static void WriteLine(string line) {
            Console.WriteLine(line);
            if(streamWriter == null) {
                return;
            }
            streamWriter.WriteLine(line);
            if(AutoFlush) {
                streamWriter.Flush();
            }
        }
        public static void WriteLine() => WriteLine(Environment.NewLine);

        public static void Write(string line) {
            Console.Write(line);
            if(streamWriter == null) {
                return;
            }
            streamWriter.Write(line);
            if(AutoFlush) {
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
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(label);
            stringBuilder.Append(": ");
            for(int i = 0;i<names.Length;i++) {
                stringBuilder.Append(names[i]);
                stringBuilder.Append(" = ");
                stringBuilder.Append(values[i] ? "Yes" : "No");
                stringBuilder.Append(" | ");
            }
            stringBuilder.Remove(stringBuilder.Length - 3, 3);
            WriteLine(stringBuilder.ToString());
        }

        public static void Flush() {
            if(streamWriter == null) {
                return;
            }
            streamWriter.Flush();
        }
    }
}
