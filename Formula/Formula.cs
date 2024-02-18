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
/// <summary>
/// Author:    Yanxia Bu
/// Partner:    No
/// Date:     1/28/2024
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and Yanxia Bu - This work may not 
///            be copied for use in Academic Coursework.
///
/// I, Yanxia Bu, certify that I wrote this code from scratch and
/// did not copy it in part or whole from another source.  All 
/// references used in the completion of the assignments are cited 
/// in my README file.
///
/// This is the class for calculate the spreadsheet which including the calculation of function in different format,it was only used once, returned a result, 
/// and then it's "life was over". In a spreadsheet the formula in a given cell will be used over and over again, 
/// as many times as the related cells are modified. 
///   Formula
/// </summary>
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
        private readonly String output;
        private readonly HashSet<string> variableLset = new HashSet<string>();
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
        /// /// <summary>
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
        ///create the constructor for checking formulas' format 
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="normalize"></param>
        /// <param name="isValid"></param>
        /// <exception cref="FormulaFormatException"></exception>
        public Formula(String formula, Func<string, string> normalize, Func<string,
        bool> isValid)
        {
            ///create the token for getting the data of string and create left and right parameter count 
            List<string> token = new List<string>(GetTokens(formula));
            int leftParaCount = 0;
            int rightParaCount = 0;
            // getting the out result for parsing double type
            double result;
            // check the  format pattern for checking the string format of operrator
            String opPattern = @"[\+\-*/]";      
            // thorw out empty case
            if (token.Count < 1 ){
                throw new FormulaFormatException("the formula less than 1 ");
            }
            // check the edge case 
            if (token.Count >= 1)
            {
                if (variableCheck(token[token.Count - 1]) == false && !double.TryParse(token[token.Count - 1], out double test) && token[token.Count - 1] != ")")
                {
                    throw new FormulaFormatException("last element error ");
                }
            }
            // start looping formula
            for (int index = 0; index < token.Count; index++)
            {
                // thorw out wrong variable format or wrong operator 
                if (!variableCheck(token[index]) || variableCheck(token[index]))
                {
                    // check the string is operator or left parathesis and throw the edge case or operation
                    if (token[index] == "(" || formatCheck(token[index], opPattern))
                    {
                        if (!variableCheck(token[index + 1]) && !double.TryParse(token[index + 1], out result) && token[index + 1] != "(")
                        {
                            throw new FormulaFormatException("format error ");
                        }
                        if (token[index] == "(")
                        {
                            leftParaCount++;
                        }
                    }
                    // check the string is variable or integer or right parathesis and throw the edge case or doing operation
                    else if ((variableCheck(token[index]) || double.TryParse(token[index], out result)) || token[index] == ")")
                    {
                        if (variableCheck(token[index]))
                        {
                            // wrong type of variable 
                            if (!isValid(normalize(token[index])))
                            {
                                throw new FormulaFormatException("format error ");
                            }
                        }
                        // throw the edge case if have another variable or integer
                        else if (index != token.Count - 1 && (variableCheck(token[index + 1]) || double.TryParse(token[index + 1], out result)))
                        {
                            throw new FormulaFormatException("format error ");
                        }
                        else if (token[index] == ")")
                        {
                            rightParaCount++;
                        }
                    }
                }
            }
            // wrong parathesis format 
            if (leftParaCount != rightParaCount)
            {
                throw new FormulaFormatException("parathesis not equal ");
            }
            // add the token to output string 
            foreach (string s in token) {
                if (variableCheck(s))
                {
                    output += normalize(s);
                    variableLset.Add(normalize(s));
                }
                else if (double.TryParse(s, out double r))
                {
                    output += r;
                }
                else
                {
                    output += s;
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
            List<string> token = new List<string>(GetTokens(output));
            int count_Left_Parentheis = 0;
            foreach (string i in token)
            {
                // I use tryparse to identify it is integer or not
                if ((double.TryParse(i, out double number)) == true)
                {
                    // if it is integer I will identify it has multiply or divide symbol in operate stack if has I will calculate 
                    if (checkMultiOrDivideOperator(value_Stack, operator_Stack))
                    {
                        //find the resukt after multiplication or division
                        double result = (multiOrdivide(value_Stack.Peek(), number, value_Stack, operator_Stack));
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
                        value_Stack.Push(addOrminus(value_Stack.Pop(), value_Stack.Pop(), value_Stack, operator_Stack));
                    }
                    if (count_Left_Parentheis != 0)
                    {
                        operator_Stack.Pop();
                        count_Left_Parentheis--;
                    }
                    // checking the wheather it is wheather  it is "/" or "+"
                    if (checkMultiOrDivideOperator(value_Stack, operator_Stack))
                    {
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
                            double result = multiOrdivide(value_Stack.Peek(), lookup(i), value_Stack, operator_Stack);
                            value_Stack.Pop();
                            value_Stack.Push(result);
                    }
                    else
                    // check variable type 
                    {
                        try
                        {
                            value_Stack.Push(lookup(i));
                        }
                        catch
                        {
                            return new FormulaError("wrong format");
                        }
                    }
                }
            }
            // after calculation we have two condition one it's no stack in operator and one value in value stack 
            if (operator_Stack.Count == 0)
            {
                double result = value_Stack.Pop();
                return result;
            }
            // another condition it we need plus or minus final 2 answer 
            else if (value_Stack.Count != 2 || operator_Stack.Count != 1)
            {
                return new FormulaError("wrong format");
            }
            return addOrminus(value_Stack.Pop(), value_Stack.Pop(), value_Stack, operator_Stack);
        }
        /// <summary>
        /// This is helper method for checking wheather it is / or * sign
        /// </summary>
        /// <param name="value_Stack"></param>
        /// <param name="operator_Stack"></param>
        /// <bool></returns>
        private static bool checkMultiOrDivideOperator(Stack<double> value_Stack, Stack<string> operator_Stack)
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
        /// <summary>
        /// This function is the operation for multiplication or division
        /// </summary>
        /// <param name="first_Value"></param>
        /// <param name="second_Value"></param>
        /// <param name="value_Stack"></param>
        /// <param name="operator_Stack"></param>
        /// <returns></returns>
        private static double multiOrdivide(double first_Value, double second_Value, Stack<double> value_Stack, Stack<string> operator_Stack)
        {
            // check the value if is wrong type division we will return the double max value which will return formula error later
            if (zeroOrEmpty(first_Value, second_Value, value_Stack, operator_Stack))
            {
                return double.MaxValue;
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
        private static bool zeroOrEmpty(double first_Value, double second_Value, Stack<double> value_Stack, Stack<string> operator_Stack)
        {
            if (operator_Stack.Peek() == "/")
            {
                if (value_Stack.Count == 0 || second_Value == 0)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        ///This function is  use regex to check it is variable type or not
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static bool variableCheck(string s) {
            if (s!="+" && s!= "-"&& s != "*"&& s != "/"&&(!double.TryParse(s,out double d))&& s != "(" && s != ")" )
            {
                if (Regex.IsMatch(s, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*") == false)
                {
                    throw new FormulaFormatException("wrong type of variable");
                }
                else { return true; }
            }
            return false;
        }
        /// <summary>
        ///This function is  use regex to check it is variable type or not
        /// </summary>
        /// <param name="s"></param>
        /// <bool></returns>
        /// <exception cref="ArgumentException"></exception>
        private static bool formatCheck(string s, string regexPattern)
        {
            return (Regex.IsMatch(s, regexPattern) && !double.TryParse(s, out double test)); 
        }
        /// <summary>
        /// This function is add or minus method
        /// </summary>
        /// <param name="first_Value"></param>
        /// <param name="second_Value"></param>
        /// <param name="value_Stack"></param>
        /// <param name="operator_Stack"></param>
        /// <double></returns>
        public static double addOrminus(double first_Value, double second_Value, Stack<double> value_Stack, Stack<string> operator_Stack)
        {
            if (operator_Stack.Pop() == "+")
            {
                return first_Value + second_Value;
            }
            return second_Value - first_Value;
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
            return variableLset;
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
            return output;
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
            Formula? str = obj as Formula;
            // else compare two string and equal.
            if(ReferenceEquals(null,str))  return false;
            return str.ToString().Equals(ToString());
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
            return f1.Equals(f2);
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
            return (!f1.Equals(f2));
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
            return ToString().GetHashCode();
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