using Elves.ElfData;
using Microsoft.Xna.Framework;
using System;

namespace Elves.Battle {
    public sealed class UserData {
 
        public string Name { get; private init; }
        public Color Color { get; private init; }

        public string PossessiveName => $"{Name}'{(Name.EndsWith('c') ? "" : "s")}";

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

        private void FireHealthChangeEvents(float health,float oldHealth) {
            if(health == oldHealth) {
                return;
            }
            if(health < oldHealth) {
                OnHurt?.Invoke();
            } else {
                OnHeal?.Invoke();
            }
            if(health <= 0) {
                OnDied?.Invoke();
            }
        }

        private float UpdateHealth(float value) {
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
            set {
                float oldValue = _health;
                _health = UpdateHealth(value);
                FireHealthChangeEvents(_health,oldValue);
            }
        }

        public float MaxHealth {
            get => _maxHealth;
            set {
                float oldValue = _health;
                _maxHealth = value;
                _health = UpdateHealth(value);
                FireHealthChangeEvents(_health,oldValue);
            }
        }

        public void Heal(float amount) => Health += amount;
        public void Hurt(float amount) => Health -= amount;

        public void HealPercent(float percent) => Health += percent * MaxHealth;
        public void HurtPercent(float percent) => Health -= percent * MaxHealth;

        public void Kill() => Health = 0;

        public event Action OnHeal, OnHurt, OnDied;
    }
}
