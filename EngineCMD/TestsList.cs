using EngineCMD.Tests;
using System.Collections.Generic;

namespace EngineCMD {
    internal sealed partial class Program {
        private static readonly List<ITest> TestsList = new List<ITest>() {
            new SerialFrameTest()
        };
    }
}
