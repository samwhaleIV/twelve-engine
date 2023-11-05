using ElfScript.Test;

namespace ElfScriptConsole {
    internal class Program {
        static void Main() {
            //Test.GarbageCollectionTest();
            Test.TokenDecoderTest("ElfScriptTest.txt");
            Console.ReadKey(true);
        }
    }
}