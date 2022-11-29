using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Elves.ElfScript {
    [Serializable]
    public class ElfScriptException:Exception {
        public ElfScriptException() { }
        public ElfScriptException(string message) : base(message) { }
        public ElfScriptException(string message, Exception inner) : base(message,inner) { }
        protected ElfScriptException(SerializationInfo info,StreamingContext context) : base(info,context) { }
    }
}
