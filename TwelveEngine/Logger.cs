using System;
using System.IO;
using System.Runtime.InteropServices;

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
            WriteLine("======================================== Twelve Engine Log ========================================");
            WriteLine($"Game started, current time (UTC): {DateTime.UtcNow}");
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

        public static void Flush() {
            if(streamWriter == null) {
                return;
            }
            streamWriter.Flush();
        }
    }
}
