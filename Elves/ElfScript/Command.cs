using Elves.BattleSequencer;
using System.Threading.Tasks;

namespace Elves.ElfScript {
    public abstract class Command {
        public abstract Task Execute(UVSequencer context,Script script);
    }
}
