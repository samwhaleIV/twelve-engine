using System.Runtime.Serialization;

namespace ElfScript.Errors {
    [Serializable]
    public class ElfScriptCompilerException:Exception {
        public ElfScriptCompilerException() {}
        public ElfScriptCompilerException(string message) : base(message) {}
        public ElfScriptCompilerException(string message,Exception inner) : base(message,inner) {}
        protected ElfScriptCompilerException(SerializationInfo info,StreamingContext context) : base(info,context) {}
    }
}
