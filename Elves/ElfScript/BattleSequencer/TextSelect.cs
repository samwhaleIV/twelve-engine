using Elves.BattleSequencer;
using System.Threading.Tasks;

namespace Elves.ElfScript.BattleSequencer {
    public abstract class TextSelect:Command {

        public bool NoRepeat { get; set; }
        public bool Random { get; set; }
        public bool RepeatLast { get; set; }
        public bool SkipFirstOnRepeat { get; set; }

        private const string INDEX_VALUE = "lastSpeechIndex";
        private const string INDEX_AT_END = "lastSpeechAtEnd";

        private int index = 0;
        private readonly string[] speeches;

        public TextSelect(string[] speeches) => this.speeches = speeches;

        public TextSelect(string speech) => speeches = new string[1] { speech };

        public abstract Task RouteText(UVSequencer context,string text);

        public override async Task Execute(UVSequencer context,Script script) {
            int speechIndex;
            if(speeches.Length == 0) {
                script.SetValue(INDEX_VALUE,0);
                script.SetValue(INDEX_AT_END,false);
                return;
            } else if(speeches.Length == 1) {
                speechIndex = 0;
                script.SetValue(INDEX_AT_END,true);
            } else if(Random) {
                speechIndex = script.GetRandom(0,speeches.Length);
                script.SetValue(INDEX_AT_END,false);
            } else {
                bool outOfRange = index >= speeches.Length;
                bool onFinal = index >= speeches.Length - 1;
                if(outOfRange) {
                    if(NoRepeat) {
                        script.SetValue(INDEX_VALUE,speeches.Length);
                        script.SetValue(INDEX_AT_END,true);
                        return;
                    } else if(RepeatLast) {
                        speechIndex = speeches.Length - 1;
                        script.SetValue(INDEX_AT_END,true);
                    } else {
                        index = SkipFirstOnRepeat ? 1 : 0;
                        speechIndex = index;
                        script.SetValue(INDEX_AT_END,false);
                    }
                } else {
                    script.SetValue(INDEX_AT_END,onFinal);
                    speechIndex = index;
                    index += 1;
                }
            }
            script.SetValue(INDEX_VALUE,speechIndex);
            await RouteText(context,speeches[speechIndex]);
        }
    }
    public sealed class SpeechSelect:TextSelect {
        public SpeechSelect(string[] speeches) : base(speeches) {}
        public SpeechSelect(string speech) : base(speech) {}
        public override Task RouteText(UVSequencer context,string text) {
            return context.Speech(text);
        }
    }
    public sealed class TagSelect:TextSelect {
        public TagSelect(string[] speeches) : base(speeches) {}
        public TagSelect(string speech) : base(speech) {}
        public override Task RouteText(UVSequencer context,string text) {
            return context.Tag(text);
        }
    }
}
