using FMOD;
using FMOD.Studio;

namespace TwelveEngine.Audio {
    public static class Extensions {

        public static string GetPath(this EventDescription eventDescription) {
            eventDescription.getPath(out string path);
            return path;
        }

        public static PARAMETER_DESCRIPTION[] GetParameterDescriptions(this EventDescription eventDescription) {
            eventDescription.getParameterDescriptionCount(out int parameterCount);
            if(parameterCount < 1) {
                return Array.Empty<PARAMETER_DESCRIPTION>();
            }
            PARAMETER_DESCRIPTION[] parameterDescriptions = new PARAMETER_DESCRIPTION[parameterCount];
            for(int i = 0;i<parameterDescriptions.Length;i++) {
                eventDescription.getParameterDescriptionByIndex(i,out PARAMETER_DESCRIPTION parameter);
                parameterDescriptions[i] = parameter;
            }
            return parameterDescriptions;
        }

        public static StringDescription GetString(this Bank bank,int index) {
            bank.getStringInfo(index,out GUID id,out string path);
            return new StringDescription() { GUID = id, Path = path };
        }

        public static StringDescription[] GetStrings(this Bank bank) {
            bank.getStringCount(out int stringCount);
            if(stringCount < 1) {
                return Array.Empty<StringDescription>();
            }
            StringDescription[] stringDescriptions = new StringDescription[stringCount];
            for(int i = 0;i<stringDescriptions.Length;i++) {
                stringDescriptions[i] = bank.GetString(i);
            }
            return stringDescriptions;
        }

        public static EventWrapper[] GetEvents(this Bank bank) {
            bank.getEventList(out EventDescription[] events);
            EventWrapper[] eventWrappers = new EventWrapper[events.Length];
            for(int i = 0;i<eventWrappers.Length;i++) {
                eventWrappers[i] = new EventWrapper(events[i]);
            }
            return eventWrappers;
        }

    }
}
