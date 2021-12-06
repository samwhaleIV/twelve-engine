using TwelveEngine;

namespace EngineCMD.Tests {
    internal class SerialFrameMeta:ITest {
        public TestResult GetResult() {

            int[] data = new int[] { 1, 2, 3, 4  };

            var frame = new SerialFrame(flipEndianness: true);
            frame.Set(data);

            var exportData = frame.Export();

            var reconstructionFrame = new SerialFrame(exportData,flipEndianness: true);
            var reconstructionData = reconstructionFrame.GetIntArray();

            bool passed = true;

            for(var i = 0;i<data.Length;i++) {
                if(data[i] != reconstructionData[i]) {
                    passed = false;
                    break;
                }
            }

            return new TestResult() { Passed = passed };
        }
    }
}
