﻿using ExpressionTrees.Examples;
using ExpressionTrees.Model;

using System.Reflection;

//Delegates.ExecuteExpressions();

var loc = Assembly.GetExecutingAssembly();

var path = Path.Combine([Path.GetDirectoryName(loc.Location), @"Data\passengers.csv"]);


var filtering = new Filtering(path);
filtering.ExecuteFilters_ExpressionsOfT(survived: true, pClass: 2, gender: Gender.Female, age: null, minimumFare: null);

Console.ReadLine();


