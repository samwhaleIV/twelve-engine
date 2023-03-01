using System.Threading.Tasks;
using System;
using TwelveEngine;

namespace Elves.Battle.Scripting {
    public readonly struct ButtonTable<T> {

        public readonly LowMemoryList<string> Options { get; private init; }
        public readonly LowMemoryList<T> Values { get; private init; }

        public ButtonTable(string option1,T value1) {
            Options = new(option1);
            Values = new(value1);
        }

        public ButtonTable(string option1,T value1,string option2,T value2) {
            Options = new(option1,option2);
            Values = new(value1,value2);
        }

        public ButtonTable(string option1,T value1,string option2,T value2,string option3,T value3) {
            Options = new(option1,option2,option3);
            Values = new(value1,value2,value3);
        }

        public ButtonTable(string option1,T value1,string option2,T value2,string option3,T value3,string option4,T value4) {
            Options = new(option1,option2,option3,option4);
            Values = new(value1,value2,value3,value4);
        }
    }
}
