using FMOD.Studio;

namespace TwelveEngine.Audio {
    public readonly struct ParameterDescription {

        public string Name { get; init; }
        public float DefaultValue { get; init; }
        public float MinValue { get; init; }
        public float MaxValue { get; init; }

        public PARAMETER_ID ID { get; init; }
        public PARAMETER_TYPE Type { get; init; }
        public PARAMETER_FLAGS Flags { get; init; }

        public ParameterDescription(PARAMETER_DESCRIPTION description) {
            Name = description.name;
            ID = description.id;
            DefaultValue = description.defaultvalue;
            MinValue = description.minimum;
            MaxValue = description.maximum;
            Flags= description.flags;
            Type = description.type;
        }
    }
}
