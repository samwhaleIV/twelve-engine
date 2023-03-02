using System;
using System.Threading.Tasks;
using TwelveEngine;

namespace Elves.Battle.Scripting {
    public readonly struct PolyThreadSelection<T> {

        public readonly bool IsItem { get; private init; }
        public readonly LowMemoryList<T> ItemList { get; private init; }
        public readonly Func<Task> Task { get; private init; }

        public PolyThreadSelection(Func<Task> value) {
            IsItem = false; Task = value; ItemList = default;
        }
        public PolyThreadSelection(LowMemoryList<T> value) {
            IsItem = true; Task = null; ItemList = value;
        }
        public PolyThreadSelection(T value) {
            IsItem = true; Task = null; ItemList = value;
        }

        public PolyThreadSelection(T value1, T value2) {
            IsItem = true; Task = null; ItemList = new(value1,value2);
        }
        public PolyThreadSelection(T value1, T value2, T value3) {
            IsItem = true; Task = null; ItemList = new(value1,value2,value3);
        }
        public PolyThreadSelection(T value1, T value2, T value3, T value4) {
            IsItem = true; Task = null; ItemList = new(value1,value2,value3,value4);
        }
        public PolyThreadSelection(T value1, T value2, T value3, T value4, T value5) {
            IsItem = true; Task = null; ItemList = new(value1,value2,value3,value4,value5);
        }
        public PolyThreadSelection(T value1, T value2, T value3, T value4, T value5, T value6) {
            IsItem = true; Task = null; ItemList = new(value1,value2,value3,value4,value5,value6);
        }
        public PolyThreadSelection(T value1, T value2, T value3, T value4, T value5, T value6, T value7) {
            IsItem = true; Task = null; ItemList = new(value1,value2,value3,value4,value5,value6,value7);
        }
        public PolyThreadSelection(T value1, T value2, T value3, T value4, T value5, T value6, T value7, T value8) {
            IsItem = true; Task = null; ItemList = new(value1,value2,value3,value4,value5,value6,value7,value8);
        }

        public static implicit operator PolyThreadSelection<T>(Func<Task> value) => new(value);
        public static implicit operator PolyThreadSelection<T>(T value) => new(value);
        public static implicit operator PolyThreadSelection<T>((T Value1, T Value2) values) => new(values);
        public static implicit operator PolyThreadSelection<T>((T Value1, T Value2, T Value3) values) => new(values);
        public static implicit operator PolyThreadSelection<T>((T Value1, T Value2, T Value3, T Value4) values) => new(values);
        public static implicit operator PolyThreadSelection<T>((T Value1, T Value2, T Value3, T Value4, T Value5) values) => new(values);
        public static implicit operator PolyThreadSelection<T>((T Value1, T Value2, T Value3, T Value4, T Value5, T Value6) values) => new(values);
        public static implicit operator PolyThreadSelection<T>((T Value1, T Value2, T Value3, T Value4, T Value5, T Value6, T Value7) values) => new(values);
        public static implicit operator PolyThreadSelection<T>((T Value1, T Value2, T Value3, T Value4, T Value5, T Value6, T Value7, T Value8) values) => new(values);
    }
}
