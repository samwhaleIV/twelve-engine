using FMOD.Studio;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text;

namespace TwelveEngine.Audio {
    public readonly struct BankWrapper {

        public Bank Bank { get; private init; }
        public int ID { get; private init; }
        public string File { get; private init; }

        public ImmutableArray<StringDescription> Strings { get; private init; }
        public ReadOnlyDictionary<string,EventWrapper> Events { get; private init; }

        public readonly bool HasEvents => Events.Count > 0;
        public readonly bool IsLoaded => AudioSystem.HasBank(ID);

        private const string EVENT_PREFIX = "event:/";

        private static string EventStringFilter(string eventString) {
            if(eventString.StartsWith(EVENT_PREFIX)) {
                return eventString[EVENT_PREFIX.Length..];
            }
            return eventString;
        }

        public BankWrapper(Bank bank,int ID,string file) {
            Bank = bank;
            this.ID = ID;
            File = file;

            Strings = ImmutableArray.Create(bank.GetStrings());

            EventWrapper[] events = bank.GetEvents();
            var eventDictionary = new Dictionary<string,EventWrapper>();
            foreach(var eventWrapper in events) {
                eventDictionary.Add(EventStringFilter(eventWrapper.Name),eventWrapper);
            }
            Events = new ReadOnlyDictionary<string,EventWrapper>(eventDictionary);
        }

        private void ValidateLoadState() {
            if(IsLoaded) {
                return;
            }
            throw new InvalidOperationException("The underlying audio bank has already been unloaded.");
        }

        public void LogEvents() {
            ValidateLoadState();
            StringBuilder sb = new();

            sb.Append($"Loaded bank '{File}'.");

            if(Strings.Length > 0) {
                sb.Append(" Strings: {");
                foreach(var value in Strings) {
                    sb.Append($" {value.Path},");
                }
                sb.Remove(sb.Length-1,1);
                sb.Append(" }");
            }

            if(Events.Count > 0) {
                if(Strings.Length > 0) {
                    sb.Append(',');
                }
                sb.Append(" Events: {");
                foreach(var value in Events.Values) {
                    sb.Append($" {value.Name},");
                }
                sb.Remove(sb.Length-1,1);
                sb.Append(" }");
            }

            Logger.WriteLine(sb.ToString(),LoggerLabel.Audio);
        }
    }
}
