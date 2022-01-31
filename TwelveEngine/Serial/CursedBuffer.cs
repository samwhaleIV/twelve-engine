using System.Diagnostics;
using System.Text;

namespace TwelveEngine.Serial {

    public readonly struct @object {

        private readonly object value;
        private @object(object value) => this.value = value;

        public bool HasValue => value != null;
        public object _ => value;

        public static implicit operator bool (@object value) => value.HasValue;
        public static implicit operator @object (int value) => new @object(value);
        public static implicit operator @object (char value) => new @object(value);

        /* Must be extended to support other types implicitly.. */
    }

    public class CursedBuffer {

        private readonly @object[] data;
        private readonly Pointer pointer;

        public CursedBuffer(int size) {
            data = new @object[size];
            pointer = new Pointer(size);
        }

        private sealed class Pointer {
            private readonly int limit;
            public Pointer(int limit) => this.limit = limit;

            private int _pointer = 0;

            public void Set(int value) => _pointer = value;

            public int Advance() {
                var value = _pointer;
                _pointer++;
                return value;
            }

            public bool AtEnd() => _pointer >= limit;
        }

        public @object _ {
            get {
                if(pointer.AtEnd()) {
                    return new @object();
                }
                return data[pointer.Advance()];
            }
            set {  
                if(value._ is int intValue) {
                    pointer.Set(intValue);
                    return;
                }
                if(pointer.AtEnd()) {
                    return;
                }
                data[pointer.Advance()] = value;
            }
        }
    }

    internal sealed class CursedBufferTest:CursedBuffer {

        public CursedBufferTest():base(13) {
            _ = 'H'; _ = 'e';  _ = 'l'; _ = 'l'; _ = 'o'; _ = ','; _ = ' ';
            _ = 'w'; _ = 'o'; _ = 'r'; _ = 'l'; _ = 'd'; _ = '!';  _ = 0;

            var sb = new StringBuilder();
            for(var value = _;value;value = _) sb.Append(value._);

            var result = sb.ToString();

            Debug.WriteLine(result); /* Output: Hello, world! */
        }
    }
}
