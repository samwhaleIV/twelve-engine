using System;
using System.Collections.Generic;

namespace EngineCMD {
    internal sealed partial class Program {

        const ConsoleColor ERROR_COLOR = ConsoleColor.DarkRed;
        const ConsoleColor SUCCESS_COLOR = ConsoleColor.DarkGreen;
        const ConsoleColor FOREGROUND_COLOR = ConsoleColor.White;

        private static (ConsoleColor,ConsoleColor) StartTextColor(
            ConsoleColor backgroundColor,ConsoleColor foregroundColor = FOREGROUND_COLOR
        ) {
            var colorFrame = (Console.BackgroundColor,Console.ForegroundColor);
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;
            return colorFrame;
        }
        private static void EndTextColor((ConsoleColor,ConsoleColor) colorFrame) {
            Console.BackgroundColor = colorFrame.Item1;
            Console.ForegroundColor = colorFrame.Item2;
        }
        private static void WriteColoredText(
            string text,ConsoleColor backgroundColor,ConsoleColor foregroundColor = FOREGROUND_COLOR,bool newLine = true
        ) {
            var oldColors = StartTextColor(backgroundColor,foregroundColor);
            if(newLine) {
                Console.WriteLine(text);
            } else {
                Console.Write(text);
            }
            EndTextColor(oldColors);
        }
        private static void RunTests(IEnumerable<ITest> tests,bool suppressNonErrors = false) {
            var passingCount = 0;
            var testCount = 0;
            foreach(var test in tests) {
                testCount++;
                WriteColoredText($"{testCount}. {test}",ConsoleColor.White,ConsoleColor.Black,false);
                Console.Write(" ");
                //try {
                    var testResult = test.GetResult();
                    if(testResult.Passed) {
                        passingCount++;
                    }
                    if(string.IsNullOrEmpty(testResult.Text)) {
                        continue;
                    }
                    if(testResult.Passed) {
                        if(suppressNonErrors) {
                            Console.WriteLine("Passed.");
                            continue;
                        }
                        Console.WriteLine(testResult.Text);
                    } else {
                        WriteColoredText(testResult.Text,ERROR_COLOR);
                    }
                //} catch(Exception error) {
                //    WriteColoredText(error.Message,ERROR_COLOR);
                //}
            }

            Console.WriteLine();
            var newColor = passingCount == testCount ? SUCCESS_COLOR : ERROR_COLOR;

            WriteColoredText($"EngineCMD: {passingCount} of {testCount} tests passed!",newColor,FOREGROUND_COLOR,false);
            Console.WriteLine();

        }
        private static void Main(string[] args) {
            RunTests(TestsList,SUPPRESS_NON_ERRORS);
        }
    }
}
