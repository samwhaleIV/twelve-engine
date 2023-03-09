﻿using ElfScript.VirtualMachine;
using ElfScript.Compiler;
using System.Text;
using ElfScript.Errors;

namespace ElfScript.Test
{
    public static class Test {

        private const string TestName = "test-item";

        private static void RunGCWithDiagnostics(VirtualMemory virtualMemory) {
            Console.Write("Before: ");
            Console.WriteLine(virtualMemory.GetDiagnostics());
            virtualMemory.Sweep();
            Console.Write("After: ");
            Console.WriteLine(virtualMemory.GetDiagnostics());
        }

        private static void NestedItemCleanUpTest() {
            Console.WriteLine("Nested Item Cleanup Test");

            var VM = new VirtualMemory();

            var string1 = VM.Create("Hello, World!");
            var string2 = VM.Create("Hello, World!");
            var string3 = VM.Create("Hello, World!");

            var table1 = VM.CreateTable();
            var virtualTable1 = VM.GetTable(table1.Address);

            virtualTable1.Set("1",string1);
            virtualTable1.Set("2",string2);
            virtualTable1.Set("3",string3);

            RunGCWithDiagnostics(VM);
        }

        private static void DeepNestedItemCleanUpTest() {
            Console.WriteLine("Deep Nested Item Cleanup Test");

            var VM = new VirtualMemory();

            var string1 = VM.Create("Hello, World!");
            var string2 = VM.Create("Hello, World!");
            var string3 = VM.Create("Hello, World!");

            var table1 = VM.CreateTable();
            var virtualTable1 = VM.GetTable(table1.Address);

            virtualTable1.Set("1",string1);
            virtualTable1.Set("2",string2);
            virtualTable1.Set("3",string3);

            var table2 = VM.CreateTable();
            var virtualTable2 = VM.GetTable(table2.Address);

            virtualTable2.Set(TestName,table1);

            RunGCWithDiagnostics(VM);
        }

        private static void MultipleCleanUpTest() {
            Console.WriteLine("Multiple Clean Up Test");

            var VM = new VirtualMemory();

            var string1 = VM.Create("Hello, World!");

            var table1 = VM.CreateTable();
            var virtualTable1 = VM.GetTable(table1.Address);

            virtualTable1.Set("1",string1);
            virtualTable1.Set("2",string1);
            virtualTable1.Set("3",string1);

            var table2 = VM.CreateTable();
            var virtualTable2 = VM.GetTable(table2.Address);
            virtualTable2.Set("1",string1);
            virtualTable2.Set("2",string1);
            virtualTable2.Set("3",string1);

            virtualTable2.Set(TestName,table1);

            RunGCWithDiagnostics(VM);
        }

        private static void CircularReferenceTest() {
            Console.WriteLine("Circular Reference Test");

            var VM = new VirtualMemory();

            var table1 = VM.CreateTable();
            var virtualTable1 = VM.GetTable(table1.Address);

            var table2 = VM.CreateTable();
            var virtualTable2 = VM.GetTable(table2.Address);

            var table3 = VM.CreateTable();
            var virtualTable3 = VM.GetTable(table3.Address);

            virtualTable1.Set(TestName,table2);
            virtualTable2.Set(TestName,table3);

            try {
                virtualTable3.Set(TestName,table1);
            } catch(ElfScriptException exception) {
                Console.WriteLine(exception.ToString());
            }

            RunGCWithDiagnostics(VM);
        }

        public static void GarbageCollectionTest() {
            NestedItemCleanUpTest();
            Console.WriteLine();
            DeepNestedItemCleanUpTest();
            Console.WriteLine();
            MultipleCleanUpTest();
            Console.WriteLine();
            CircularReferenceTest();
            return;
        }

        public static void TokenDecoderTest(string file) {
            var tokenDecoder = new TokenDecoder();
            tokenDecoder.Import(File.ReadAllLines(file));
            tokenDecoder.Export(out string disassembly);
            Console.WriteLine(disassembly);
            return;
        }
    }
}
