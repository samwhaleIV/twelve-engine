using System.Text;

namespace TwelveEngine {
    public static class Flags {

        private static HashSet<string> _flags;

        private static void SetFlags(HashSet<string> flags) {
            _flags = flags;
        }

        public static bool Get(string flag) {
            if(_flags is null) {
                throw new InvalidOperationException("Cannot obtain a flag value before the flags set has been loaded. Call 'Flags.Load(string[])' first.");
            }
            return _flags.Contains(flag);
        }

        private static void WriteToLog(HashSet<string> flags) {
            StringBuilder sb = new();
            sb.Append($"{{ ");
            if(flags.Count <= 0) {
                sb.Append(Logger.NONE_TEXT);
                sb.Append(" }");
                Logger.WriteLine(sb);
                return;
            }
            foreach(var flag in flags) {
                sb.Append(string.IsNullOrWhiteSpace(flag) ? Logger.EMPTY_TEXT : flag);
                sb.Append(", ");
            }
            sb.Remove(sb.Length-2,2);
            sb.Append(" }");
            Logger.WriteLine(sb,LoggerLabel.Flags);
        }

        internal static void Load(string[] args) {
            string[] flagList = null;
            string[] configFlags = Config.GetStringArray(Config.Keys.Flags);
            if(args is not null && args.Length >= 1) {
                flagList = args;
            } else if(configFlags is not null) {
                flagList = configFlags;
            }
            HashSet<string> flagSet;
            if(flagList is null) {
                flagSet = new HashSet<string>(0);
                SetFlags(flagSet);
                WriteToLog(flagSet);
                return;
            }
            flagSet = new HashSet<string>(flagList.Length);
            foreach(string flag in flagList) {
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
