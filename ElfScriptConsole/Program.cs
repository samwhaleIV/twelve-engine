using ElfScript.Test;

namespace ElfScriptConsole {
    internal class Program {
        static void Main() {
            //Test.GarbageCollectionTest();
            Test.TokenDecoderTest("ElfScriptBlockTest.txt");
            Console.ReadKey(true);
        }
    }
}