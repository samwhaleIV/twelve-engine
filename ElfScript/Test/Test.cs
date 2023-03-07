using ElfScript.VirtualMachine;
using ElfScript.Compiler;
using System.Text;

namespace ElfScript.Test {
    public static class Test {

        public static void CompilerTest() {
            var tokens = TokenTranslator.GenerateTokens(File.ReadAllLines("ElfScriptTest.txt"));
            Console.WriteLine(TokenTranslator.GetTokenDisassembly(tokens));
        }
    }
}
