/// <summary>
/// Author:    Yanxia Bu
/// Partner:    No
/// Date:     1/15/2024
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and Yanxia Bu - This work may not 
///            be copied for use in Academic Coursework.
///
/// I, Yanxia Bu, certify that I wrote this code from scratch and
/// did not copy it in part or whole from another source.  All 
/// references used in the completion of the assignments are cited 
/// in my README file.
///
/// File Contents
///
///   the spreadsheet 
/// </summary>
using FormulaEvaluator;
using System;
// simple test plus and minus or single
Console.WriteLine(Evaluator.Evaluate("1+1", null));
Console.WriteLine(Evaluator.Evaluate("5", null));
Console.WriteLine(Evaluator.Evaluate("5+2+2", null));
Console.WriteLine(Evaluator.Evaluate("5-2", null));
Console.WriteLine(Evaluator.Evaluate("5-2-2", null));
// simple test multi or division
Console.WriteLine(Evaluator.Evaluate("5*2", null));
Console.WriteLine(Evaluator.Evaluate("4/2", null));
Console.WriteLine(Evaluator.Evaluate("5/2", null));
Console.WriteLine(Evaluator.Evaluate("1*2", null));
Console.WriteLine(Evaluator.Evaluate("1/2", null));
Console.WriteLine(Evaluator.Evaluate("1*2*2", null));
Console.WriteLine(Evaluator.Evaluate("100/2/2", null));
// complicate test 
Console.WriteLine(Evaluator.Evaluate("10/5+2-1*2", null));
Console.WriteLine(Evaluator.Evaluate("10/5+(2-1)*2", null));
Console.WriteLine(Evaluator.Evaluate("((1+2)-3)*10/2", null));
Console.WriteLine(Evaluator.Evaluate("((4+2)-3)*10/2", null));
Console.WriteLine(Evaluator.Evaluate("((1)+1+(1+1+2)-1)", null));
// vairable test 
Console.WriteLine(Evaluator.Evaluate("10/5+A5", Lookup));
Console.WriteLine(Evaluator.Evaluate("10+A5", Lookup));
Console.WriteLine(Evaluator.Evaluate("A5", Lookup));
Console.WriteLine(Evaluator.Evaluate("10*A5+A5", Lookup));
Console.WriteLine(Evaluator.Evaluate("A5-1", Lookup));
// bug test such as wrong variable format, expression format, sysmbol format and dividing zero 
try
{
    Evaluator.Evaluate(" -A- ", null);
}
catch (ArgumentException)
{
    // write a message saying that your code detected the invalid syntax of the formula
}
try
{
    Evaluator.Evaluate(" ++ ", null);
}
catch (ArgumentException)
{
    Console.WriteLine("wrong format");

}
try
{
    Evaluator.Evaluate(" 1+ ", null);
}
catch (ArgumentException)
{
    Console.WriteLine("wrong format");

}
try
{
    Evaluator.Evaluate(" 1+1- ", null);
}
catch (ArgumentException)
{
    Console.WriteLine("wrong format");

}
try
{
    Evaluator.Evaluate(" 1/0 ", null);
}
catch (ArgumentException)
{
    Console.WriteLine("divide by zero");

}
try
{
    Evaluator.Evaluate(" +5+5 ", null);
}
catch (ArgumentException)
{
    Console.WriteLine("wrong format");

}
try
{
    Evaluator.Evaluate(" (1+2 ", null);
}
catch (ArgumentException)
{
    Console.WriteLine("wrong format");

}
try
{
    Evaluator.Evaluate(" 2+1) ", null);
}
catch (ArgumentException)
{
    Console.WriteLine("wrong format");

}
try
{
    Evaluator.Evaluate(" (2A+3)) ", null);
}
catch (ArgumentException)
{
    Console.WriteLine("wrong format");

}
static int Lookup(string s)
{
    return 2;
}