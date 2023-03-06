using System.Runtime.Serialization;

namespace ElfScript.Errors {
	[Serializable]
	public sealed class ElfScriptException:Exception {
		public ElfScriptException() {}
		public ElfScriptException(string message) : base(message) {}
		public ElfScriptException(string message,Exception inner) : base(message,inner) {}
		private ElfScriptException(SerializationInfo info,StreamingContext context) : base(info,context) {}
	}
}
