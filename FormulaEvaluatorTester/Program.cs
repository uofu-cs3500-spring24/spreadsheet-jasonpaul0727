using FormulaEvaluator;
Console.WriteLine(Evaluator.Evaluate("5+2*2",null));
Console.WriteLine(Evaluator.Evaluate("5+2", null));
try
{ 
    Evaluator.Evaluate(" -A- ", null);
}
catch (ArgumentException)
{
    // write a message saying that your code detected the invalid syntax of the formula
}