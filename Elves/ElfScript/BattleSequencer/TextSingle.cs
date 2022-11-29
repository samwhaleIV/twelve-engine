using Elves.BattleSequencer;
using System.Threading.Tasks;

namespace Elves.ElfScript.BattleSequencer {
    public abstract class TextSingle:Command {
        private readonly string[] lines;
        public TextSingle(string[] lines) {
            this.lines = lines;
        }
        public TextSingle(string line) {
            lines = new string[1] { line };
        }
        public abstract Task RouteText(UVSequencer context,string text);
        public override async Task Execute(UVSequencer context,Script script) {
            foreach(var line in lines) {
                await RouteText(context,line);
            }
        }
    }
    public sealed class SpeechMulti:TextSingle {
        public SpeechMulti(string line) : base(line) {}
        public SpeechMulti(string[] lines) : base(lines) {}
        public override Task RouteText(UVSequencer context,string text) {
            return context.Speech(text);
        }
    }
    public sealed class TagMulti:TextSingle {
        public TagMulti(string line) : base(line) {}
        public TagMulti(string[] lines) : base(lines) {}
        public override Task RouteText(UVSequencer context,string text) {
            return context.Tag(text);
        }
    }
}
