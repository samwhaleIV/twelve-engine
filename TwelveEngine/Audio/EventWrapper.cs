using FMOD.Studio;
using System.Collections.ObjectModel;

namespace TwelveEngine.Audio {
    public readonly struct EventWrapper {

        public ReadOnlyDictionary<string,ParameterDescription> Parameters { get; private init; }
        public string Name { get; private init; }
        public EventDescription EventDescription { get; private init; }
        public int Length { get; private init; }

        private static Dictionary<string,ParameterDescription> GetParameterDictionary(PARAMETER_DESCRIPTION[] descriptions) {
            var dictionary = new Dictionary<string,ParameterDescription>();
            foreach(var description in descriptions) {
                dictionary.Add(description.name,new ParameterDescription(description));
            }
            return dictionary;
        }

        public EventWrapper(EventDescription eventDescription) {
            EventDescription = eventDescription;

            Name = eventDescription.GetPath();
            eventDescription.getLength(out int length);
            Length = length;

            var parameterDictionary = GetParameterDictionary(eventDescription.GetParameterDescriptions());
            Parameters = new ReadOnlyDictionary<string,ParameterDescription>(parameterDictionary);
        }

        public EventInstanceController Create() {
            EventDescription.createInstance(out EventInstance instance);
            return new EventInstanceController(instance);
        }
    }
}
