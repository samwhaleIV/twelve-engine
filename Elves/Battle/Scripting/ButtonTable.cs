using TwelveEngine;

namespace Elves.Battle.Scripting {
    public readonly struct ButtonTable<T> {

        public readonly LowMemoryList<string> Options { get; private init; }
        public readonly LowMemoryList<T> Values { get; private init; }

        public ButtonTable(LowMemoryList<string> options,LowMemoryList<T> actions) {
            Options = options;
            Values = actions;
        }

        public ButtonTable((string Option,T Value) button) {
            Options = new(button.Option);
            Values = new(button.Value);
        }

        public ButtonTable((string Option,T Value) button1,(string Option,T Value) button2) {
            Options = new(button1.Option,button2.Option);
            Values = new(button1.Value,button2.Value);
        }

        public ButtonTable((string Option,T Value) button1,(string Option,T Value) button2,(string Option,T Value) button3) {
            Options = new(button1.Option,button2.Option,button3.Option);
            Values = new(button1.Value,button2.Value,button3.Value);
        }

        public ButtonTable((string Option,T Value) button1,(string Option,T Value) button2,(string Option,T Value) button3,(string Option,T Value) button4) {
            Options = new(button1.Option,button2.Option,button3.Option,button4.Option);
            Values = new(button1.Value,button2.Value,button3.Value,button4.Value);
        }
    }
}
