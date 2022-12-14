using System;
using System.Collections.Generic;
using System.Text;

namespace Elves.Battle {

	[Serializable]
	public class ElfBattleException:Exception {
		public ElfBattleException() { }
		public ElfBattleException(string message) : base(message) { }
		public ElfBattleException(string message,Exception inner) : base(message,inner) { }
		protected ElfBattleException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info,context) { }
	}
}
