namespace EngineCMD {
    internal struct TestResult {
        public TestResult(string text,bool passed) {
            Text = text;
            Passed = passed;
        }
        public string Text;
        public bool Passed;
    }
    internal interface ITest {
       TestResult GetResult();
    }
}
