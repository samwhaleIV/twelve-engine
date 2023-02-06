using FMOD.Studio;
using System.Collections.ObjectModel;

namespace TwelveEngine.Audio {
    public readonly struct EventWrapper {

        public ReadOnlyDictionary<string,ParameterDescription> Parameters { get; private init; }
        public string Name { get; private init; }
        public EventDescription Description { get; private init; }
        public int Length { get; private init; }

        private static Dictionary<string,ParameterDescription> GetParameterDictionary(PARAMETER_DESCRIPTION[] descriptions) {
            var dictionary = new Dictionary<string,ParameterDescription>();
            foreach(var description in descriptions) {
                dictionary.Add(description.name,new ParameterDescription(description));
            }
            return dictionary;
        }

        public EventWrapper(EventDescription eventDescription) {
            Description = eventDescription;

            Name = eventDescription.GetPath();
            eventDescription.getLength(out int length);
            Length = length;

            var parameterDictionary = GetParameterDictionary(eventDescription.GetParameterDescriptions());
            Parameters = new ReadOnlyDictionary<string,ParameterDescription>(parameterDictionary);
        }

        public ManagedEventInstance Create() => new(this);

        public void Play(float volume = 1) {
            Description.createInstance(out EventInstance instance);
            instance.setVolume(volume);
            instance.start();
            instance.release();
        }
    }
}
