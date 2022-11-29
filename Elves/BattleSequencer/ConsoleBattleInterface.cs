using System;
using System.Collections.Generic;
using System.Text;

namespace Elves.BattleSequencer {
    public sealed class ConsoleBattleInterface:UVSequencer {

        public void TestBattle() {
            var player = AddUser(new UVUser() {
                Name = "Sam"
            });
            var elf = AddUser(new UVUser() {
                Name = "Elf"
            });
            while(true) {
                ProcessEvent(elf.Kill());
                ProcessUserTurns();
                Console.ReadKey();
            }

        }

        internal override void EmitTagline(string message) {
            Console.WriteLine(message);
        }

        internal override void EmitUserDamaged(UVUser user,int amount) {
            Console.WriteLine($">{user.Name} has lost {amount} health.");
            Console.Beep(200,100);
        }

        internal override void EmitUserDeath(UVUser user) {
            Console.WriteLine($">{user.Name} has died.");
        }

        internal override void EmitUserEnterBattle(UVUser user) {
            Console.WriteLine($">{user.Name} enters the battle.");
        }

        internal override void EmitUserExitBattle(UVUser user) {
            Console.WriteLine($">{user.Name} exits the battle.");
        }

        internal override void EmitUserHealed(UVUser user,int amount) {
            Console.WriteLine($">{user.Name} has gained {amount} health.");
        }

        internal override void EmitUserSpeech(UVUser user,string message) {
            Console.WriteLine($"{user.Name}: {message}");
        }
    }
}
