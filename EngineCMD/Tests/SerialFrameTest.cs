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
                frame.GetSerializable("Color",Color);
            }
        }
        private class Dog:ISerializable {
            public long TailCount { get; set; } = 1;
            public Hair Hair { get; set; } = new Hair();
            public Name Name { get; } = new Name();
            public void Export(SerialFrame frame) {
                frame.Set("Name",Name);
                frame.Set("Hair",Hair);
                frame.Set("TailCount",TailCount);
            }

            public void Import(SerialFrame frame) {
                frame.GetSerializable("Name",Name);
                frame.GetSerializable("Hair",Hair);
                TailCount = frame.GetLong("TailCount");
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

            var serialFrame = new SerialFrame();
            dog.Export(serialFrame);
            var json = serialFrame.Export();

            var copySerialFrame = new SerialFrame(json);
            var cloneDog = new Dog();
            cloneDog.Import(copySerialFrame);

            var cloneDogFromOriginalDNA = new Dog();
            cloneDogFromOriginalDNA.Import(serialFrame);

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
