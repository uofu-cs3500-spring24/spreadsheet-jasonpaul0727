// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!
// Version 1.1 (9/22/13 11:45 a.m.)
// Change log:
// (Version 1.1) Repaired mistake in GetTokens
// (Version 1.1) Changed specification of second constructor to
// clarify description of how validation works
// (Daniel Kopta)
// Version 1.2 (9/10/17)
// Change log:
// (Version 1.2) Changed the definition of equality with regards
// to numeric tokens
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard
   /// precedence
    /// rules. The allowed symbols are non-negative numbers written using double-
   /// precision
    /// floating-point syntax (without unary preceeding '-' or '+');
    /// variables that consist of a letter or underscore followed by
    /// zero or more letters, underscores, or digits; parentheses; and the fouroperator
    /// symbols +, -, *, and /.
///
/// Spaces are significant only insofar that they delimit tokens. For example,
///"xy" is
/// a single variable, "x y" consists of two variables "x" and y; "x23" is a
///single variable;
    /// and "x 23" consists of a variable "x" and a number "23".
    ///
    /// Associated with every formula are two delegates: a normalizer and a
   /// validator.The
/// normalizer is used to convert variables into a canonical form, and the
///validator is used
/// to add extra restrictions on the validity of a variable (beyond the standard
///requirement
/// that it consist of a letter or underscore followed by zero or more letters,
///underscores,
/// or digits.) Their use is described in detail in the constructor and method
///comments.
/// </summary>
public class Formula
{
        String formula;
        List<string> variableList;
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression
        ///written as
        /// described in the class comment. If the expression is syntactically
        ///invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        ///
        /// The associated normalizer is the identity function, and the associated
        ///validator
        /// maps every string to true.
        /// </summary>
        public Formula(String formula) :
        this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression
        ///written as
        /// described in the class comment. If the expression is syntactically
        ///incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        ///
        /// The associated normalizer and validator are the second and third
        ///parameters,
        /// respectively.
        ///
        /// If the formula contains a variable v such that normalize(v) is not a legal
        ///variable,
        /// throws a FormulaFormatException with an explanatory message.
        ///
        /// If the formula contains a variable v such that isValid(normalize(v)) is
        ///false,
        /// throws a FormulaFormatException with an explanatory message.
        ///
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit. Then:
        ///
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string,
        bool> isValid)
        {
            List<string> token = new List<string>(GetTokens(formula));
            int leftCount = 0;
            int rightCount = 0;
            int leftParaCount = 0;
            int rightParaCount = 0;
            double result;
            if (token.Count < 1 && !((variableCheck(token[0]) || double.TryParse(token[0], out result) || token[0] != "("))
            && !((variableCheck(token[token.Count - 1]) || double.TryParse(token[token.Count - 1], out result) || token[token.Count - 1] != ")")))
            {
                throw new FormulaFormatException("the formula less than 1 ");
            }
            for (int index = 0; index < token.Count; index++)
            {
                {
                    if (variableCheck(token[index]) || double.TryParse(token[index], out result) || token[index] != "(" || token[index] != ")" || token[index] != "+" || token[index] != "-" || token[index] != "*" || token[index] != "/")
                    {
                        if (token[index] == ")")
                        {
                            rightCount++;
                        }
                        else if (token[index] == "(")
                        {
                            if (!(variableCheck(token[index + 1]) || double.TryParse(token[index + 1], out result)))
                            {
                                throw new FormulaFormatException("format error ");
                            }
                            leftCount++;
                        }
                        else if (variableCheck(token[index]) || double.TryParse(token[index], out result) || token[index] != ")")
                        {
                            if (token[index + 1] != "+" || token[index + 1] != "-" || token[index + 1] != "*" || token[index + 1] != "/" || token[index + 1] != ")")
                            {
                                throw new FormulaFormatException("format error ");
                            }
                        }
                        else if (variableCheck(token[index]) || double.TryParse(token[index], out result))
                        {
                            if (variableCheck(token[index]))
                            {
                                if (!isValid(normalize(token[index])))
                                {
                                    throw new FormulaFormatException("format error ");
                                }
                            }
                        }
                        else
                        {
                            throw new FormulaFormatException("the formula less than 1 ");
                        }
                    }
                        if (leftParaCount != rightParaCount || leftCount != rightCount)
                        {
                        throw new FormulaFormatException("parathesis not equal ");
                        }
                }
            }
                foreach( string s in token){
                if (variableCheck(s))
                {
                    formula += normalize(s);
                }
                else if(double.TryParse(s, out result))
                {
                    formula += result;
                }
                else
                {
                    formula += s;
                }
            }
        }
           
        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables. When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to
        /// the constructor.)
        ///
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters
        /// in a string to upper case:
        ///
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        ///
        /// Given a variable symbol as its parameter, lookup returns the variable's value
        /// (if it has one) or throws an ArgumentException (otherwise).
        ///
        /// If no undefined variables or divisions by zero are encountered when evaluating
        /// this Formula, the value is returned. Otherwise, a FormulaError is returned.
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        

        public object Evaluate(Func<string, double> lookup)
        {   // create 2 stack one store value, another one store operator
            Stack<double> value_Stack = new Stack<double>();
            Stack<string> operator_Stack = new Stack<string>();
            List<string> token = new List<string>(GetTokens(formula));
            int count_Left_Parentheis = 0;
            foreach (string i in token)
            {
                // because of the first one is empty which I need use if to skip it.
                if (i == "")
                {
                    continue;
                }
                // I use tryparse to identify it is integer or not
                if ((int.TryParse(i, out int number)) == true)
                {
                    // if it is integer I will identify it has multiply or divide symbol in operate stack if has I will calculate 
                    if (checkMultiOrDivideOperator(value_Stack, operator_Stack))
                    {
                        //find the resukt after multiplication or division
                       double result=(multiOrdivide(value_Stack.Peek(), number, value_Stack, operator_Stack));
                        value_Stack.Pop();
                        value_Stack.Push(result);
                    }
                    else
                    {
                        value_Stack.Push(number);
                    }
                    continue;
                }
                // if the sign is plus or minus check the previous sign is same or not and operate the different action 
                if (i == "+" || i == "-")
                {
                    if (operator_Stack.Count() > 0 && (operator_Stack.Peek() == "+" || (operator_Stack.Peek() == "-")))
                    {
                        if (value_Stack.Count >= 2)
                        {
                            // if the previous symbol it is + or - , I will add or minus 
                            double result = addOrminus(value_Stack.Pop(), value_Stack.Pop(), value_Stack, operator_Stack);
                            if (result != double.MaxValue)
                            {
                                value_Stack.Push(result);
                            }
                        }
                        else
                        {
                            return new FormulaError("wrong format");
                        }
                    }
                    // add to the stack
                    operator_Stack.Push(i); ;
                    continue;
                }
                // not operation if  matching this 3 symbols
                if (i == "*" || i == "/" || i == "(")
                {
                    operator_Stack.Push(i);
                    if (i == "(")
                    {
                        count_Left_Parentheis++;
                    }
                    continue;
                }
                // if this is ")" I will doing the check tthe add or minus and push to stack 
                if (i == ")")
                {
                    if (operator_Stack.Peek() == "+" || (operator_Stack.Peek() == "-"))
                    {
                        if (!lessthan2(value_Stack))
                        {
                            return new FormulaError("wrong format");
                        }
                        value_Stack.Push(addOrminus(value_Stack.Pop(), value_Stack.Pop(), value_Stack, operator_Stack));
                    }
                    if (count_Left_Parentheis != 0)
                    {
                        operator_Stack.Pop();
                        count_Left_Parentheis--;
                    }
                    // throw the catch for wrong format 
                    else
                    {
                        return new FormulaError("wrong format");
                    }
                    // checking the wheather it is wheather  it is "/" or "+"
                    if (checkMultiOrDivideOperator(value_Stack, operator_Stack))
                    {
                        lessthan2(value_Stack);
                        value_Stack.Push(multiOrdivide(value_Stack.Pop(), value_Stack.Pop(), value_Stack, operator_Stack));
                    }
                    continue;
                }
                // checking the it is variable or not
                if (variableCheck(i))
                {
                    // check it is divide or not
                    if (checkMultiOrDivideOperator(value_Stack, operator_Stack))
                    {
                        // find the result for multiplication or division 
                        double result = multiOrdivide(value_Stack.Peek(),lookup(i), value_Stack, operator_Stack);
                        value_Stack.Pop();
                        value_Stack.Push(result);
                    }
                    else
                    {
                        value_Stack.Push(lookup(i));
                    }
                }

            }
            // after calculation we have two condition one it's no stack in operator and one value in value stack 
            if (operator_Stack.Count == 0)
            {
                if (value_Stack.Count != 1)
                {
                    return new FormulaError("wrong format");
                }
                double result = value_Stack.Pop();
                return checkError(result);
            }
            // another condition it we need plus or minus final 2 answer 
            else if (value_Stack.Count != 2 || operator_Stack.Count != 1)
            {
                return new FormulaError("wrong format");
            }
            return checkError(addOrminus(value_Stack.Pop(), value_Stack.Pop(), value_Stack, operator_Stack));

        }
        /// <summary>
        /// This is helper method for checking wheather it is / or * sign
        /// </summary>
        /// <param name="value_Stack"></param>
        /// <param name="operator_Stack"></param>
        /// <returns></returns>
        public static bool checkMultiOrDivideOperator(Stack<double> value_Stack, Stack<string> operator_Stack)
        {
            if (operator_Stack.Count() >= 1)
            {
                if ((operator_Stack.Peek() == "/" || operator_Stack.Peek() == "*"))
                {
                    return true;
                }
            }
            return false;
        }
        public static object checkError(double method)
        {
            if (method >= double.MaxValue)
            {
                return new FormulaError("wrong format");
            }
            return method;
        }
        /// <summary>
        /// This function is the operation for multiplication or division
        /// </summary>
        /// <param name="first_Value"></param>
        /// <param name="second_Value"></param>
        /// <param name="value_Stack"></param>
        /// <param name="operator_Stack"></param>
        /// <returns></returns>
        public static double multiOrdivide(double first_Value, double second_Value, Stack<double> value_Stack, Stack<string> operator_Stack)
        {
            // check the value 
            if(zeroOrEmpty(first_Value, second_Value, value_Stack, operator_Stack))
            {
                return Double.MaxValue;
            }
            double result = 0;
            // multiplication
            if (operator_Stack.Pop() == "*")
            {
                result = first_Value * second_Value;
                return result;
            }
            result = first_Value / second_Value;
            return result;
        }
        /// <summary>
        /// This is the method for checking the the division is zero or value stack is zero
        /// </summary>
        /// <param name="first_Value"></param>
        /// <param name="second_Value"></param>
        /// <param name="value_Stack"></param>
        /// <param name="operator_Stack"></param>
        /// <exception cref="ArgumentException"></exception>
        public static bool zeroOrEmpty(double first_Value, double second_Value, Stack<double> value_Stack, Stack<string> operator_Stack)
        {
            if (operator_Stack.Peek() == "/")
            {
                if (value_Stack.Count == 0 || second_Value == 0)
                {
                    return false;
                }
            }
            return true;

        }
        /// <summary>
        ///This function is  use regex to check it is variable type or not
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool variableCheck(string s)
        {
            if (Regex.IsMatch(s, @"[a-zA-Z_$]+[0-9]+$") == false)
            {
                throw new ArgumentException("wrong type of variable");
            }

            return true;
        }
        /// <summary>
        /// This function is add or minus method
        /// </summary>
        /// <param name="first_Value"></param>
        /// <param name="second_Value"></param>
        /// <param name="value_Stack"></param>
        /// <param name="operator_Stack"></param>
        /// <returns></returns>
        public static double addOrminus(double first_Value, double second_Value, Stack<double> value_Stack, Stack<string> operator_Stack)
        {
            // check the symbol in stack is  plus or minus 
            if (operator_Stack.Peek() == "+" || operator_Stack.Peek() == "-" && zeroOrEmpty(first_Value, second_Value, value_Stack, operator_Stack))
            {
                // if it is +
                if (operator_Stack.Pop() == "+")
                {
                    return first_Value + second_Value;
                }
                else
                {
                    return second_Value - first_Value;
                }
            }
            return double.MaxValue;
        }
        /// <summary>
        /// This function is check the value stack less than 2 or not
        /// </summary>
        /// <param name="value_Stack"></param>
        /// <exception cref="ArgumentException"></exception>
        public static bool lessthan2(Stack<double> value_Stack)
        {
            if (value_Stack.Count < 2)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this
        /// formula. No normalization may appear more than once in the enumeration,even
        /// if it appears more than once in this Formula.
        ///
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X","Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return null;
        }
        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f). All of the
        /// variables in the string should be normalized.
        ///
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return null;
        }
        /// <summary>
        /// <change> make object nullable </change>
        ///
        /// If obj is null or obj is not a Formula, returns false. Otherwise, reports
        /// whether or not this Formula and obj are equal.
        ///
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order. To determine token equality, all tokens are compared as strings
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized"
        /// by C#'s standard conversion from string to double, then back to string. This
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as
        /// defined by the provided normalizer.
        ///
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1 + Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            return false;
        }
        /// <summary>
        /// <change> We are now using Non-Nullable objects. Thus neither f1 nor f2
        ///can be null!</change>
        /// Reports whether f1 == f2, using the notion of equality from the Equals
        ///method.
        ///
        /// </summary>
     public static bool operator ==(Formula f1, Formula f2)
     {
            return false;
     }
        /// <summary>
        /// <change> We are now using Non-Nullable objects. Thus neither f1 nor f2
       /// can be null!</change>
/// <change> Note: != should almost always be not ==, if you get my meaning
///</change>
/// Reports whether f1 != f2, using the notion of equality from the Equals
///method.
/// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return false;
        }
        /// <summary>
        /// Returns a hash code for this Formula. If f1.Equals(f2), then it must be
        ///the
        /// case that f1.GetHashCode() == f2.GetHashCode(). Ideally, the probability
        ///that two
        /// randomly-generated unequal Formulae have the same hash code should be
        ///extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return 0;
        }
        /// <summary>
        /// Given an expression, enumerates the tokens that compose it. Tokens are
        ///  left paren;
        /// right paren; one of the four operator symbols; a string consisting of a
        ///letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal;
        ///and anything that doesn't
        /// match one of those patterns. There are no empty tokens, and no token
        ///contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";
            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) |({5})",lpPattern, rpPattern, opPattern, varPattern,doublePattern, spacePattern);
            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern,
            RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }
    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
        : base(message)
        {
        }
    }
    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
        : this()
        {
            Reason = reason;
        }
        /// <summary>
        /// The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}
// <change>
// If you are using Extension methods to deal with common stack operations (e.g.,
///checking for
// an empty stack before peeking) you will find that the Non-Nullable checking is
//"biting" you.
//
// To fix this, you have to use a little special syntax like the following:
//
// public static bool OnTop<T>(this Stack<T> stack, T element1, T element2)
///where T : notnull
//
// Notice that the "where T : notnull" tells the compiler that the Stack can
///contain any object
// as long as it doesn't allow nulls!
// </change>