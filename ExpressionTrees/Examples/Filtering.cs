using AgileObjects.ReadableExpressions;

using ExpressionTrees.Model;

using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace ExpressionTrees.Examples;

public class Filtering
{
    private IEnumerable<Passenger> _passengers;

    public Filtering(string filePath)
    {
        _passengers = GetPassengers(filePath);
    }

    private IEnumerable<Passenger> GetPassengers(string filePath)
    {
        var lines = File.ReadAllLines(filePath);

        var passengers = new List<Passenger>();

        foreach (var line in lines)
        {
            var values = line.Split('\t');

            var passenger = new Passenger
            (
                Survived: values[0] == "1",
                PClass: int.Parse(values[1]),
                Name: values[2],
                Gender: values[3] == "male" ? Gender.Male : Gender.Female,
                Age: decimal.Parse(values[4]),
                SiblingsOrSpouse: int.Parse(values[5]),
                ParentOrChildren: int.Parse(values[6]),
                Fare: decimal.Parse(values[7])
            );

            passengers.Add(passenger);
        }

        return passengers;
    }

    public void ExecuteFilters_Linq(
        bool? survived,
        int? pClass,
        Gender? gender,
        decimal? age,
        decimal? minimumFare)
    {
        if (survived is not null)
        {
            _passengers = _passengers.Where(p => p.Survived == survived);
        }

        if (pClass is not null)
        {
            _passengers = _passengers.Where(p => p.PClass == pClass);
        }

        if (gender is not null)
        {
            _passengers = _passengers.Where(p => p.Gender == gender);
        }

        if (age is not null)
        {
            _passengers = _passengers.Where(p => p.Age == age);
        }

        if (minimumFare is not null)
        {
            _passengers = _passengers.Where(p => p.Fare == minimumFare);
        }

        foreach (var passenger in _passengers)
        {
            Console.WriteLine(passenger);
        }
    }

    public void ExecuteFilters_Expressions(
        bool? survived,
        int? pClass,
        Gender? gender,
        decimal? age,
        decimal? minimumFare)
    {
        Expression currentExpression = null;

        var passengerParameter = Expression.Parameter(type: typeof(Passenger), name: "p");

        if (survived is not null)
        {
            var value = survived.Value;
            var property = "Survived";

            var constantExpression = Expression.Constant(value: value);
            var memberExpression = Expression.Property(expression: passengerParameter, propertyName: property);
            var equalExpression = Expression.Equal(left: memberExpression, right: constantExpression);

            currentExpression = equalExpression;

            // _passengers = _passengers.Where(p => p.Survived == survived);
        }

        if (pClass is not null)
        {
            var value = pClass.Value;
            var property = "PClass";

            var constantExpression = Expression.Constant(value: value);
            var memberExpression = Expression.Property(expression: passengerParameter, propertyName: property);
            var equalExpression = Expression.Equal(left: memberExpression, right: constantExpression);

            if (currentExpression is null)
            {
                currentExpression = equalExpression;
            }
            else
            {
                // Use logical AND (&&) or OR (||) depending on the logic you need
                currentExpression = Expression.AndAlso(left: currentExpression, right: equalExpression);
            }

            //_passengers = _passengers.Where(p => p.PClass == pClass);
        }

        if (gender is not null)
        {
            var value = gender.Value;
            var property = "Gender";

            var constantExpression = Expression.Constant(value: value);
            var memberExpression = Expression.Property(expression: passengerParameter, propertyName: property);
            var equalExpression = Expression.Equal(left: memberExpression, right: constantExpression);

            if (currentExpression is null)
            {
                currentExpression = equalExpression;
            }
            else
            {
                currentExpression = Expression.AndAlso(left: currentExpression, right: equalExpression);
            }

            //_passengers = _passengers.Where(p => p.Gender == gender);
        }

        if (age is not null)
        {
            var value = age.Value;
            var property = "Age";

            var constantExpression = Expression.Constant(value: value);
            var memberExpression = Expression.Property(expression: passengerParameter, propertyName: property);
            var equalExpression = Expression.Equal(left: memberExpression, right: constantExpression);

            if (currentExpression is null)
            {
                currentExpression = equalExpression;
            }
            else
            {
                currentExpression = Expression.AndAlso(left: currentExpression, right: equalExpression);
            }

            //_passengers = _passengers.Where(p => p.Age == age);
        }

        if (minimumFare is not null)
        {
            var value = minimumFare.Value;
            var property = "Fare";

            var constantExpression = Expression.Constant(value: value);
            var memberExpression = Expression.Property(expression: passengerParameter, propertyName: property);
            var greaterThanOrEqualExpression = Expression.GreaterThanOrEqual(left: memberExpression, right: constantExpression);

            if (currentExpression is null)
            {
                currentExpression = greaterThanOrEqualExpression;
            }
            else
            {
                currentExpression = Expression.AndAlso(left: currentExpression, right: greaterThanOrEqualExpression);
            }

            //_passengers = _passengers.Where(p => p.Fare == minimumFare);
        }

        if (currentExpression is not null)
        {
            var finalExpr = Expression
                .Lambda<Func<Passenger, bool>>(
                    body: currentExpression,
                    parameters: [passengerParameter]);

            var func = finalExpr.Compile();

            _passengers = _passengers.Where(func);
        }

        foreach (var passenger in _passengers)
        {
            Console.WriteLine(passenger);
        }
    }

    public void ExecuteFilters_ExpressionsOfT(
        bool? survived,
        int? pClass,
        Gender? gender,
        decimal? age,
        decimal? minimumFare)
    {
        Expression currentExpression = null;

        var passengerParameter = Expression.Parameter(type: typeof(Passenger), name: "p");

        if (survived is not null)
        {
            currentExpression = CreateExpression<bool>(survived.Value, null, "Survived", passengerParameter);

            // _passengers = _passengers.Where(p => p.Survived == survived);
        }

        if (pClass is not null)
        {
            currentExpression = CreateExpression<int>(pClass.Value, currentExpression, "PClass", passengerParameter);

            //_passengers = _passengers.Where(p => p.PClass == pClass);
        }

        if (gender is not null)
        {
            currentExpression = CreateExpression<Gender>(gender.Value, currentExpression, "Gender", passengerParameter);

            //_passengers = _passengers.Where(p => p.Gender == gender);
        }

        if (age is not null)
        {
            currentExpression = CreateExpression<decimal>(age.Value, currentExpression, "Age", passengerParameter);

            //_passengers = _passengers.Where(p => p.Age == age);
        }

        if (minimumFare is not null)
        {
            currentExpression = CreateExpression<decimal>(minimumFare.Value, currentExpression, "Fare", passengerParameter, ">");

            //_passengers = _passengers.Where(p => p.Fare == minimumFare);
        }

        string query = string.Empty;
        if (currentExpression is not null)
        {
            var expr = Expression
                .Lambda<Func<Passenger, bool>>(
                    body: currentExpression,
                    tailCall: false,
                    parameters: [passengerParameter]);

            var staticFunc = expr.Compile();

            query = expr.ToReadableString();

            _passengers = _passengers.Where(staticFunc);
        }

        Console.WriteLine("########################## Static Query #############################");
        Console.WriteLine("");
        Console.WriteLine($"Query - {query}");
        Console.WriteLine("");
        foreach (var passenger in _passengers)
        {
            Console.WriteLine(passenger);
        }
        Console.WriteLine("");
    }

    public void ExecuteFilters_Dynmaic(string query)
    {
        if (query is not null)
        {
            var expr = DynamicExpressionParser
                .ParseLambda<Passenger, bool>(
                    parsingConfig: new ParsingConfig(),
                    createParameterCtor: true,
                    expression: query);

            var func = expr.Compile();

            _passengers = _passengers.Where(func);
        }

        Console.WriteLine("########################## Dyamic Query #############################");
        Console.WriteLine("");
        foreach (var passenger in _passengers)
        {
            Console.WriteLine(passenger);
        }
        Console.WriteLine("");
    }

    private Expression CreateExpression<T>(T value, Expression? currentExpression, string propertyName, ParameterExpression objectParameter, string operatorType = "=")
    {
        var valueToTest = Expression.Constant(value: value);

        var propertyToCall = Expression.Property(expression: objectParameter, propertyName: propertyName);

        Expression operatorExpression = operatorType switch
        {
            ">" => Expression.GreaterThan(propertyToCall, valueToTest),
            "<" => Expression.LessThan(propertyToCall, valueToTest),
            ">=" => Expression.GreaterThanOrEqual(propertyToCall, valueToTest),
            "<=" => Expression.LessThanOrEqual(propertyToCall, valueToTest),
            _ => Expression.Equal(propertyToCall, valueToTest),
        };

        if (currentExpression == null)
        {
            currentExpression = operatorExpression;
        }
        else
        {
            currentExpression = Expression.And(currentExpression, operatorExpression);
        }

        return currentExpression;
    }
};
