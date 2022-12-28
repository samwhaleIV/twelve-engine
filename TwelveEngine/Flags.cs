using System;
using System.Collections.Generic;

namespace TwelveEngine {
    public static class Flags {

        private static HashSet<string> _flags;

        internal static void SetFlags(HashSet<string> flags) {
            _flags = flags;
        }

        public static bool Get(string flag) {
            return _flags.Contains(flag);
        }
    }
}
