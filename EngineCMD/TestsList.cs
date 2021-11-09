using EngineCMD.Tests;
using System.Collections.Generic;

namespace EngineCMD {
    internal sealed partial class Program {

        const bool SUPPRESS_NON_ERRORS = false;

        private static readonly List<ITest> TestsList = new List<ITest>() {
            new SerialFrameTest()
        };
    }
}
