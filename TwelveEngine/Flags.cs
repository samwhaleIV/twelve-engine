using System.Collections.Generic;
using System.Text;

namespace TwelveEngine {
    public static class Flags {

        private static HashSet<string> _flags;

        internal static void SetFlags(HashSet<string> flags) {
            _flags = flags;
        }

        public static bool Get(string flag) {
            return _flags.Contains(flag);
        }

        private static void WriteToLog(HashSet<string> flags) {
            var sb = new StringBuilder();
            sb.Append($"[Flags] {{ ");
            if(flags.Count <= 0) {
                sb.Append(Constants.Logging.None);
                sb.Append(" }");
                Logger.WriteLine(sb);
                return;
            }
            foreach(var flag in flags) {
                sb.Append(string.IsNullOrWhiteSpace(flag) ? Constants.Logging.Empty : flag);
                sb.Append(", ");
            }
            sb.Remove(sb.Length-2,2);
            sb.Append(" }");
            Logger.WriteLine(sb);
        }

        internal static void Load(string[] args) {
            IEnumerable<string> flagList;
            var configFlags = Config.GetStringArray(Config.Keys.Flags);
            if(configFlags is null) {
                flagList = args;
            } else {
                flagList = configFlags;
            }
            var flagSet = new HashSet<string>();
            if(flagList is null) {
                SetFlags(flagSet);
                WriteToLog(flagSet);
                return;
            }
            foreach(var flag in flagList) {
                if(string.IsNullOrWhiteSpace(flag)) {
                    continue;
                }
                flagSet.Add(flag);
            }
            SetFlags(flagSet);
            WriteToLog(flagSet);
        }
    }
}
