using System;
using System.Runtime.Serialization;

namespace Elves.Battle {
	[Serializable]
	public class MiniGameException:Exception {
		public MiniGameException() { }
		public MiniGameException(string message) : base(message) { }
		public MiniGameException(string message,Exception inner) : base(message,inner) { }
		protected MiniGameException(
		  SerializationInfo info,
		  StreamingContext context) : base(info,context) { }
	}
}
