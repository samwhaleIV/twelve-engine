using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwelveEngine.Serial;

namespace Elves.BattleSequencer {
    public class UVUser:ISerializable {

        public string Name { get; set; } = Strings.MissingName;
        private int _health = Constants.DefaultHealth;
        private int _maxHealth = Constants.DefaultHealth;
        public UserModifiers Modifiers { get; set; } = UserModifiers.None;

        internal int _ID { get; set; } = Constants.InvalidUserID;
        internal bool _InRemovalQueue { get; set; } = false;

        public int Priority { get; set; } = Constants.DefaultUserPriority;

        protected internal virtual UVEvent GetTurnEvent() => Events.UserDidNothing(this);
        protected internal virtual UVEvent GetExitEvent() => Events.UserGoodbye(this);
        protected internal virtual UVEvent GetEnterEvent() => Events.UserHello(this);

        private UVEvent ProcessDamage(int value) {
            if(Modifiers.HasFlag(UserModifiers.NoDamage)) {
                return Events.UserTookNoDamage(this);
            }
            int difference = _health - value;
            _health = value;
            Queue<UVEvent> eventQueue = new Queue<UVEvent>();
            eventQueue.Enqueue(Events.UserDamaged(this,difference));
            if(IsDead) {
                eventQueue.Enqueue(Events.UserGoodbye(this));
                eventQueue.Enqueue(Events.UserHasDied(this));
            }
            return new EventSet(eventQueue);
        }

        private UVEvent ProcessHeal(int value) {
            if(Modifiers.HasFlag(UserModifiers.NoHeal)) {
                return Events.UserCannotHeal(this);
            }
            int difference = value - _health;
            _health = value;
            return Events.UserHealed(this,difference);
        }

        public UVEvent SetHealth(int value) {
            value = Math.Min(Math.Max(value,Constants.MinHealth),_maxHealth);
            if(value == _health) {
                return null;
            } else if(value < _health) {
                return ProcessDamage(value);
            } else {
                return ProcessHeal(value);
            }
        }

        public UVEvent SetMaxHealth(int value) {
            _maxHealth = Math.Max(value,Constants.MinHealth);
            return SetHealth(_health);
        }

        public UVEvent Heal(int amount) => SetHealth(_health + amount);
        public UVEvent Hurt(int amount) => SetHealth(_health - amount);

        public int Health { get => _health; }
        public int MaxHealth { get => _maxHealth; }

        public UVEvent Kill() => SetHealth(Constants.MinHealth);

        public bool IsAlive => _health > Constants.MinHealth;
        public bool IsDead => _health == Constants.MinHealth;

        public virtual void Export(SerialFrame frame) {
            frame.Set(Name);
            frame.Set(_health);
            frame.Set(_maxHealth);
            frame.Set((int)Modifiers);  
            frame.Set(_ID);
            frame.Set(_InRemovalQueue);
            frame.Set(Priority);
        }

        public virtual void Import(SerialFrame frame) {
            Name = frame.GetString();
            _health = frame.GetInt();
            _maxHealth = frame.GetInt();
            Modifiers = (UserModifiers)frame.GetInt();
            _ID = frame.GetInt();
            _InRemovalQueue = frame.GetBool();
            Priority = frame.GetInt();
        }
    }
}
