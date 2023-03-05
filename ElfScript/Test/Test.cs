using ElfScript.Expressions.Block;
using ElfScript.Expressions.Variable.Types;
using ElfScript.Expressions.Variable;
using ElfScript.IR;


namespace ElfScript.Test {
    public static class Test {
        public static void Main() {
            var sm = new StateMachine();


            var expression = new FunctionExpression(new string[] {
                "a","b","c"
            },new Expression[] {
                new SetVariableExpression("z",new NumberExpression(2)),
                new ReturnExpression(new GetVariableExpression("z")),
                new NumberExpression(1),
                new NumberExpression(2),
                new NumberExpression(3),
                new NumberExpression(4)
            });

            var value = expression.EvaluateFunction(sm,new Span<Value>(new Value[] {
                new Value(), new Value(), new Value()
            }));

            Console.ReadKey(true);
        }
    }
}
