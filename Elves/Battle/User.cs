using Microsoft.Xna.Framework;
using System.Text;

namespace Elves.Battle {
    public sealed class User {
        public readonly StringBuilder Name = new StringBuilder();
        public float Health { get; set; } = Constants.DefaultHealth;
        public float MaxHealth { get; set; } = Constants.DefaultHealth;
        public Color Color { get; set; } = Color.White;
    }
}
