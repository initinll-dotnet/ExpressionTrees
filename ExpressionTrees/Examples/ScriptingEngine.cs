using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace ExpressionTrees.Examples;

public class ScriptingEngine
{
    public static Expression ExpressionFromString<T, T1>(string value)
    {
        return DynamicExpressionParser.ParseLambda<T, T1>(new ParsingConfig(), true, value);
    }

    /*
     public static bool IsPrime(int value)
    {
        if (value <= 1)
            return false;
        if (value == 2)
            return true;
        if (value % 2 == 0)
            return false;

        int boundary = (int)Math.Floor(Math.Sqrt(value));

        for (int i = 3; i <= boundary; i += 2)
        {
            if (value % i == 0)
                return false;
        }

        return true;
    }
     */
    public static Expression IsPrime(ParameterExpression value)
    {
        var label = Expression.Label();

        var result = Expression.Parameter(type: typeof(bool), name: "result");

        var returnLabel = Expression.Label(type: typeof(bool));

        // value <= 1
        var valueLessThanEqualToOne = Expression
            .LessThanOrEqual(
                left: value,
                right: Expression.Constant(1));

        // value == 2
        var valueEqualTwo = Expression
            .Equal(
                left: value,
                right: Expression.Constant(2));

        // value % 2 - Expression.Modulo(value, Expression.Constant(2))
        // value % 2 == 0
        var valueModTwoZero = Expression
            .Equal(
                left: Expression.Modulo(left: value, right: Expression.Constant(2)),
                right: Expression.Constant(0));

        // Math.Sqrt()
        var sqRt = typeof(Math).GetMethod(name: "Sqrt");

        // Math.Sqrt(value)
        var valueSqRt = Expression
            .Call(
                instance: null,
                method: sqRt,
                arguments: Expression.Convert(value, typeof(double)));

        // Math.Floor
        var floor = typeof(Math).GetMethod(name: "Floor", types: [typeof(double)]);

        // (int)Math.Floor(Math.Sqrt(value))
        var evalFunction = Expression
            .Convert(
                expression: Expression.Call(instance: null, method: floor, arguments: valueSqRt),
                type: typeof(int));

        // int boundary 
        var boundary = Expression.Variable(typeof(int), "boundary");

        // int i
        var i = Expression.Variable(typeof(int), "i");

        // if (value % i == 0)
        //      return false;
        Expression modBlock = Expression.IfThen(
            test: Expression
                    .Equal(
                        left: Expression.Modulo(value, i),
                        right: Expression.Constant(0)),
            ifTrue: Expression
                    .Return(
                        target: returnLabel,
                        value: Expression.Constant(false))
        );

        // i += 2
        Expression incrementI = Expression
            .AddAssign(
                left: i,
                right: Expression.Constant(2));

        BlockExpression block = Expression.Block(
            // variables - result, i & boundary
            variables: [result, i, boundary],
            expressions: [
                // if (value <= 1)
                //  return false;
                Expression.IfThen(
                        test: valueLessThanEqualToOne,
                        ifTrue: Expression.Return(target: returnLabel, value: Expression.Constant(false))
                ),
                // if (value == 2)
                //  return true;
                Expression.IfThen(
                        test: valueEqualTwo,
                        ifTrue: Expression.Return(target: returnLabel, value:Expression.Constant(true))
                ),
                // if (value % 2 == 0)
                //  return false;
                Expression.IfThen(
                        test: valueModTwoZero,
                        ifTrue: Expression.Return(target: returnLabel, value:Expression.Constant(false))
                ),
                //int i = 3;
                Expression
                    .Assign(
                        left: i,
                        right: Expression.Constant(3)),
                // int boundary = (int)Math.Floor(Math.Sqrt(value));
                Expression
                    .Assign(
                        left: boundary,
                        right: evalFunction),
                // for (int i = 3; i <= boundary; i += 2)
                Expression.Loop(
                    body: Expression.IfThenElse
                    (
                        // i <= boundary;
                        test: Expression.LessThanOrEqual(left: i, right: boundary),
                        //  if (value % i == 0)
                        //    return false;
                        ifTrue: Expression.Block(arg0: modBlock, arg1: incrementI),
                        ifFalse: Expression.Break(target: label)
                    ),
                    @break: label
                ),
                Expression.Return(target: returnLabel, value : Expression.Constant(true)),
                Expression.Label(target: returnLabel, defaultValue: Expression.Constant(true))
            ]);

        return block;
    }

    /// <summary>
    /// From Microsoft Docs: https://docs.microsoft.com/en-us/dotnet/csharp/expression-trees-building
    /// </summary>
    /// <returns>Returns a factorial expression</returns>
    public static Expression Factorial(ParameterExpression value)
    {
        ParameterExpression result = Expression.Variable(typeof(int), "result");

        // Creating a label that represents the return value
        LabelTarget label = Expression.Label(typeof(int));

        var initializeResult = Expression.Assign(result, Expression.Constant(1));

        // This is the inner block that performs the multiplication,
        // and decrements the value of 'n'
        var block = Expression.Block(
            Expression.Assign(result,
                Expression.Multiply(result, value)),
            Expression.PostDecrementAssign(value)
        );

        // Creating a method body.
        BlockExpression body = Expression.Block(
            [result],
            initializeResult,
            Expression.Loop(
                Expression.IfThenElse(
                    Expression.GreaterThan(value, Expression.Constant(1)),
                    block,
                    Expression.Break(label, result)
                ),
                label
            )
        );

        return body;
    }
}
