using System.Linq.Expressions;

namespace ExpressionTrees.Examples;

public class Delegates
{
    public static void ExecuteExpressions()
    {
        var xExpression = Expression.Parameter(typeof(int), "x");
        var constant12Expression = Expression.Constant(12);
        var constant4Expression = Expression.Constant(4);

        var greaterThan = Expression.GreaterThanOrEqual(xExpression, constant12Expression);
        var lessThan = Expression.LessThanOrEqual(xExpression, constant4Expression);

        // Expression => x > 12 or x < 4
        var orExp = Expression.Or(lessThan, greaterThan);
        // Expression => x > 12
        var expr1 = Expression.Lambda<Func<int, bool>>(body: greaterThan, parameters: [xExpression]);
        var func1 = expr1.Compile();

        var expr2 = Expression.Lambda<Func<int, bool>>(body: orExp, parameters: [xExpression]);
        var func2 = expr2.Compile();

        Console.WriteLine(func1(11));
        Console.WriteLine(func2(10));
    }
}
