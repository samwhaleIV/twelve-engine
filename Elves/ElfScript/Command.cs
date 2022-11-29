using Elves.BattleSequencer;

namespace Elves.ElfScript {
    public abstract class Command {
        public abstract void Execute(UVSequencer context,Script script);
        public abstract Command[] Compile(string[][] lines);
    }
}
