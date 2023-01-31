using Elves.ElfData;
using Microsoft.Xna.Framework;

namespace Elves.Battle {
    public sealed class UserData {
 
        public string Name { get; private init; }
        public Color Color { get; private init; }

        public UserData(Elf elf) {
            Name = elf.Name;
            Color = elf.Color;
        }

        public UserData(string name,Color color) {
            Name = name;
            Color = color;
        }

        public UserData(string name) {
            Name = name;
            Color = Color.White;
        }

        private float _maxHealth = Constants.Battle.DefaultHealth;
        private float _health = Constants.Battle.DefaultHealth;

        public bool IsAlive => Health > 0;
        public bool IsDead => Health <= 0;

        public float HealthFraction => _maxHealth <= 0 ? 0 : _health / _maxHealth;

        private float ValidateHealth(float value) {
            if(value > _maxHealth) {
                value = _maxHealth;
            }
            if(value < 0) {
                value = 0;
            }
            return value;
        }

        public float Health {
            get => _health;
            set => _health = ValidateHealth(value);
        }

        public float MaxHealth {
            get => _maxHealth;
            set {
                _maxHealth = value;
                _health = ValidateHealth(Health);
            }
        }

        public void Heal(float amount) {
            Health = ValidateHealth(Health + amount);
        }

        public void Hurt(float amount) {
            Health = ValidateHealth(Health - amount);
        }
    }
}
