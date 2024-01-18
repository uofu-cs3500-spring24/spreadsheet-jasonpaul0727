using FormulaEvaluator;
using System;
// simple test plus and minus or single
Console.WriteLine(Evaluator.Evaluate("1+1",null));
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
Evaluator.Lookup variavle = new Evaluator.Lookup("A5");
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
    Console.WriteLine("wrong format");

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
try
{
    Evaluator.Evaluate("5/0", null);
}
catch (ArgumentException)
{
    Console.WriteLine("wrong format");

}