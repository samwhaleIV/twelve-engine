using ElfScript.VirtualMachine;
using ElfScript.Expressions.Block;
using ElfScript.Expressions.Variable.Types;
using ElfScript.Expressions.Variable;
using ElfScript.Expressions.System;
using ElfScript.Compiler;
using System.Text;

namespace ElfScript.Test
{
    public static class Test {
        public static void ExpressionTest() {
            var sm = new StateMachine();

            var expression2 = new BodyExpression(new Expression[] {
                new SetVariableExpression("firstClassFunctionsAreCool",new FunctionExpression(new string[] {
                    "a","b","c"
                },new Expression[] {
                    new SetVariableExpression("z",new NumberExpression(2)),
                    new ReturnExpression(new GetVariableExpression("z")),
                    new NumberExpression(1),
                    new NumberExpression(2),
                    new NumberExpression(3),
                    new NumberExpression(4),
                    new ReturnExpression(new StringExpression("Hello, World!"))
                })),
                new SetVariableExpression("ayy_lmao",new ExecuteFunctionExpression("firstClassFunctionsAreCool",new Expression[] {
                    new NumberExpression(1),
                    new NumberExpression(2),
                    new NumberExpression(3)
                })),
                new WriteConsoleLineExpression(new GetVariableExpression("ayy_lmao"))
            });

            expression2.Evaluate(sm);
        }

        public static void CompilerTest() {
            TokenTranslator.GenerateExpressions(File.ReadAllLines("ElfScriptTest.txt"),Console.Write);
        }
    }
}
