using System.Runtime.Serialization;

namespace TwelveEngine.EntitySystem {
    [Serializable]
    public class EntityManagerException:Exception {
        public EntityManagerException() { }
        public EntityManagerException(string message) : base(message) { }
        public EntityManagerException(string message,Exception inner) : base(message,inner) { }
        protected EntityManagerException(SerializationInfo info,StreamingContext context) : base(info,context) { }
    }
}
