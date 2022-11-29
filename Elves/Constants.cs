using System;
using System.Collections.Generic;
using System.Text;

namespace Elves {
    public static class Constants {
        public const int MinHealth = 0;
        public const int DefaultHealth = 100;
        public const int DefaultUserPriority = 0;
        public const int PlayerUserPriority = DefaultUserPriority - 1;
        public const int LowUserPriority = DefaultUserPriority + 1;
        public const int HighUserPriority = PlayerUserPriority - 1;
        public const int InvalidUserID = -1;
    }
}
