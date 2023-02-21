using System;

namespace Elves.Battle {
	[Serializable]
	public sealed class MiniGameException:Exception {
		public MiniGameException() { }
		public MiniGameException(string message) : base(message) { }
		public MiniGameException(string message,Exception inner) : base(message,inner) { }
		protected MiniGameException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info,context) { }
	}
}
