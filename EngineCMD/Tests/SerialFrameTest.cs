using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine;

namespace EngineCMD.Tests {
    internal sealed class SerialFrameTest:ITest {

        private class Name:ISerializable {
            public string First { get; set; }
            public string Last { get; set; }
            public string Middle { get; set; }
            public void Export(SerialFrame frame) {
                frame.Set("First",First);
                frame.Set("Last",Last);
                frame.Set("Middle",Middle);
            }

            public void Import(SerialFrame frame) {
                First = frame.GetString("First");
                Middle = frame.GetString("Middle");
                Last = frame.GetString("Last");
            }
        }
        private class Color:ISerializable {
            public string Value { get; set; } = "White";

            public void Export(SerialFrame frame) {
                frame.Set("Value",Value);
            }

            public void Import(SerialFrame frame) {
                Value = frame.GetString("Value");
            }
        }
        private class Hair:ISerializable {

            public Color Color { get; } = new Color();
            public string Type { get; set; } = "Fuzzy";
            public void Export(SerialFrame frame) {
                frame.Set("Color",Color);
                frame.Set("Type",Type);
            }

            public void Import(SerialFrame frame) {
                frame.Get("Color",Color);
            }
        }
        private class Dog:ISerializable {
            public long TailCount { get; set; } = 1;
            public Hair Hair { get; set; } = new Hair();
            public Name Name { get; } = new Name();
            public float Weight { get; set; } = 479.34f;
            public bool IsAlive { get; set; } = true;
            public int[,] MapData { get; set; } = new int[0,0];
            public float[,] Coordinates { get; set; } = new float[,] {
                {2.432f, 1.441f},
                {453.23432f, 34f }
            };
            public double[,] CoordinatesDouble { get; set; } = new double[,] {
                {2.4323292394923432d, 1.44239423949329d},
                {453.2346546456456432d, 34.319429349239432942d }
            };
            public bool[,] Flags { get; set; } = new bool[,] {
                {true, false },
                {false, true },
                {true, true }
            };
            public string[,] InfoTable { get; set; } = new string[,] {
                { "a","b","c" },
                { "d","e","f" },
                { "g","h","i" }
            };
            public Dog[] Dogs { get; set; } = new Dog[0];

            public int[] Numbers = new int[] { 1,2,3,4 };

            public void Export(SerialFrame frame) {
                frame.Set("Name",Name);
                frame.Set("Hair",Hair);
                frame.Set("TailCount",TailCount);
                frame.Set("Weight",Weight);
                frame.Set("IsAlive",IsAlive);
                frame.Set("MapData",MapData);
                frame.Set("Flags",Flags);
                frame.Set("CoordinatesDouble",CoordinatesDouble);
                frame.Set("Coordinates",Coordinates);
                frame.Set("InfoTable",InfoTable);
                frame.Set("Dogs",Dogs);
                frame.Set("Numbers",Numbers);
            }

            public void Import(SerialFrame frame) {
                frame.Get("Name",Name);
                frame.Get("Hair",Hair);
                Weight = frame.GetFloat("Weight");
                TailCount = frame.GetLong("TailCount");
                IsAlive = frame.GetBool("IsAlive");
                MapData = frame.GetIntArray2D("MapData");
                Flags = frame.GetBoolArray2D("Flags");
                CoordinatesDouble = frame.GetDoubleArray2D("CoordinatesDouble");
                Coordinates = frame.GetFloatArray2D("Coordinates");
                InfoTable = frame.GetStringArray2D("InfoTable");
                Dogs = frame.GetArray<Dog>("Dogs");
                Numbers = frame.GetIntArray("Numbers");
            }
        }

        public TestResult GetResult() {
            var dog = new Dog();
            dog.Name.First = "I";
            dog.Name.Middle = "AM";
            dog.Name.Last = "DOGGIE";
            dog.Hair.Type = "Weird";
            dog.Hair.Color.Value = "Red";
            dog.TailCount = 2;
            dog.IsAlive = false;
            dog.Weight = float.PositiveInfinity;
            dog.MapData = new int[,] {
                { 1,2,4,5,6 },
                { 7,8,9,10,11 },
                { 12,13,14,15,16 }
            };
            dog.Dogs = new Dog[] {
                new Dog(), new Dog(), new Dog(), new Dog()
            };

            var serialFrame = new SerialFrame();
            dog.Export(serialFrame);
            var json = serialFrame.Export();

            var copySerialFrame = new SerialFrame(json);
            var cloneDog = new Dog();
            cloneDog.Import(copySerialFrame);

            var copyJson = copySerialFrame.Export();
            var jsonMatches = json == copyJson;
            var message = jsonMatches ? json : "Serial frame export JSON data does not match! This could just be an order issue, compare the following data:";
            if(!jsonMatches) {
                message += $"\nJSON 1: {json}\nJSON 2: {copyJson}";
            }

            return new TestResult(message,jsonMatches);
        }
    }
}
