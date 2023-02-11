﻿using System.Runtime.InteropServices;
using System.Text;
using TwelveEngine.Shell;

namespace TwelveEngine {

    public static class Logger {

        private static StreamWriter streamWriter = null;

        public static string Path { get; private set; } = null;

        public const string EMPTY_TEXT = "<Empty>";
        public const string NONE_TEXT = "<None>";
        public const string UNKNOWN_TEXT = "<Unknown>";
        public const string NO_NAME_TEXT = "<No Name>";

        public static bool HasStreamWriter => streamWriter != null;

        private static readonly Dictionary<LoggerLabel,string> labelNames = new() {
            { LoggerLabel.None, "None" },
            { LoggerLabel.KeyBinds, "Key Binds" },
            { LoggerLabel.Config, "Config" },
            { LoggerLabel.Save, "Save" },
            { LoggerLabel.Benchmark, "Benchmark" },
            { LoggerLabel.Logger, "Logger" },
            { LoggerLabel.GameManager, "Game Manager" },
            { LoggerLabel.Flags, "Flags" },
            { LoggerLabel.UI, "UI" },
            { LoggerLabel.Script, "Script" },
            { LoggerLabel.Audio, "Audio" }
        };

        private static string GetLoggerLabel(LoggerLabel label) {
            if(label == LoggerLabel.None) {
                return null;
            }
            if(!labelNames.TryGetValue(label,out string labelValue)) {
                return $"[{label}] ";
            }
            return $"[{labelValue}] ";
        }

        public static void CleanUp() {
            WriteLine($"[{DateTime.UtcNow} UTC] Game is exiting.");
            if(!HasStreamWriter) {
                return;
            }
            try {
                streamWriter.Flush();
            } catch(Exception exception) {
                Console.Write(GetLoggerLabel(LoggerLabel.Logger));
                Console.WriteLine($"Failure flushing log stream. This will not be reflected in file. Exception: {exception}");
            }
            streamWriter?.Dispose();
            streamWriter = null;
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
                Console.Write(GetLoggerLabel(LoggerLabel.Logger));
                Console.WriteLine($"Failure creating log stream: {exception}");
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
            if(Flags.Get(Constants.Flags.Console)) {
                AllocConsole();
            }
#endif
            if(TryCreateStreamWriter(path)) {
                WriteLine($"[{DateTime.UtcNow} UTC] Game started.");
            } else {
                WriteLine($"[{DateTime.UtcNow} UTC] Game started with no log file.");
            }
        }

        public static void WriteLine(string line,LoggerLabel label = LoggerLabel.None) {
            string loggerLabel = GetLoggerLabel(label);
            if(loggerLabel != null) {
                Console.Write(loggerLabel);
            }
            Console.WriteLine(line);
            if(!HasStreamWriter) {
                return;
            }
            if(loggerLabel != null) {
                streamWriter.Write(loggerLabel);
            }
            streamWriter.WriteLine(line);
        }

        public static void WriteLine(StringBuilder line,LoggerLabel label = LoggerLabel.None) {
            string loggerLabel = GetLoggerLabel(label);
            if(loggerLabel != null) {
                Console.Write(loggerLabel);
            }
            Console.WriteLine(line);
            if(!HasStreamWriter) {
                return;
            }
            if(loggerLabel != null) {
                streamWriter.Write(loggerLabel);
            }
            streamWriter.WriteLine(line);
        }

        public static void Write(string text,LoggerLabel label = LoggerLabel.None) {
            string loggerLabel = GetLoggerLabel(label);
            if(loggerLabel != null) {
                Console.Write(loggerLabel);
            }
            Console.Write(text);
            if(!HasStreamWriter) {
                return;
            }
            if(loggerLabel != null) {
                streamWriter.Write(loggerLabel);
            }
            streamWriter.Write(text);
        }

        public static void Write(StringBuilder text,LoggerLabel label = LoggerLabel.None) {
            string loggerLabel = GetLoggerLabel(label);
            if(loggerLabel != null) {
                Console.Write(loggerLabel);
            }
            Console.Write(text);
            if(!HasStreamWriter) {
                return;
            }
            if(loggerLabel != null) {
                streamWriter.Write(loggerLabel);
            }
            streamWriter.Write(text);
        }

        public static void WriteBooleanSet(string text,string[] names,bool[] values,LoggerLabel label = LoggerLabel.None) {
            if(names.Length != values.Length) {
                throw new ArgumentException("Bad boolean set, names and values must be equal in length.");
            }
            if(names.Length == 0) {
                return;
            }
            string loggerLabel = GetLoggerLabel(label);
            var lease = Pools.StringBuilder.Lease(out var sb);
            if(loggerLabel != null) {
                sb.Append(loggerLabel);
            }
            sb.Append(text);
            sb.Append(": ");
            for(int i = 0;i<names.Length;i++) {
                sb.Append(names[i]);
                sb.Append(" = ");
                sb.Append(values[i] ? "Yes" : "No");
                sb.Append(" | ");
            }
            sb.Remove(sb.Length - 3, 3);
            WriteLine(sb);
            Pools.StringBuilder.Return(lease);
        }

        public static void LogStateChange(GameState state) {
            var lease = Pools.StringBuilder.Lease(out var sb);
            sb.Append('[');
            sb.AppendFormat(Constants.TimeSpanFormat,ProxyTime.GetElapsedTime());
            sb.Append("] Set state: ");
            string stateName = state.Name;
            sb.Append('"');
            sb.Append(string.IsNullOrEmpty(stateName) ? Logger.NO_NAME_TEXT : stateName);
            sb.Append("\" { Args = ");
            StateData data = state.Data;
            if(data.Args is not null && data.Args.Length >= 1) {
                foreach(var arg in data.Args) {
                    if(string.IsNullOrWhiteSpace(arg)) {
                        continue;
                    }
                    sb.Append($"{arg}, ");
                }
                sb.Remove(sb.Length-2,2);
            } else {
                sb.Append("None");
            }
            sb.AppendLine($", Flags = {data.Flags.ToString()} }}");
            Write(sb);
            Pools.StringBuilder.Return(lease);
        }
    }
}
