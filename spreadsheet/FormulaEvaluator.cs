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
///   the spreadsheet is functions are versatile tools in programming and mathematics, used for calculations, data manipulation, string operations, logical decision-making
///  ,and automating repetitive tasks, thereby streamlining complex processes and enhancing efficiency.
/// </summary>
using System.Text.RegularExpressions;
using System.Transactions;
namespace FormulaEvaluator
{
    public static class Evaluator
    {

        /// This delegate type lookup
        /// <param name="v"></String>
        /// <returns></type lookup>
        public delegate int Lookup(String v);
        /// <summary>
        ///This is my main method for evaluate the expression, In this method I accord with mutiple condition to identify such as Integer, variable and different symbol.
        ///I also care about multiple edge case and throw or avoid. 
        /// the action of operator stack and value stack
        /// <param name="exp"></string>
        /// <param name="variableEvaluator"></lookup>
        /// <returns></int>
        /// <exception cref="ArgumentException"></exception>
        public static int Evaluate(string input, Lookup variableEvaluator)
        {

            // create 2 stack one store value, another one store operator
            Stack<int> value_Stack = new Stack<int>();
            Stack<string> operator_Stack = new Stack<string>();
            string[] substrings = Regex.Split(input, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            int count_Left_Parentheis = 0;
            foreach (string i in substrings)
            {
                // because of the first one is empty which I need use if to skip it 
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
                        int result = multiOrdivide(value_Stack.Peek(), number, value_Stack, operator_Stack);
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
                    if (operator_Stack.Count() >0&& (operator_Stack.Peek() == "+" || (operator_Stack.Peek() == "-")))
                    {
                        if (value_Stack.Count >= 2)
                        {
                            // if the previous symbol it is + or - , I will add or minus 
                            int result = addOrminus(value_Stack.Pop(), value_Stack.Pop(), value_Stack, operator_Stack);
                            if (result != int.MaxValue)
                            {
                                value_Stack.Push(result);
                            }
                        }
                        else
                        {
                            throw new ArgumentException("wrong format");
                        }
                    }  
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
                    if(operator_Stack.Peek()=="+"|| (operator_Stack.Peek() == "-"))
                    {
                        lessthan2(value_Stack);
                        value_Stack.Push(addOrminus(value_Stack.Pop(),value_Stack.Pop(), value_Stack, operator_Stack));
                    }
                    if (count_Left_Parentheis!=0)
                    {
                        operator_Stack.Pop();
                        count_Left_Parentheis--;
                    }
                    // throw the catch for wrong format 
                    else
                    {
                        throw new ArgumentException("wrong format");
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
                        int result = multiOrdivide(value_Stack.Peek(), variableEvaluator(i), value_Stack, operator_Stack);
                        value_Stack.Pop();
                        value_Stack.Push(result);
                    }
                    else
                    {
                        value_Stack.Push(variableEvaluator(i));
                    }
                }

            }
            // after calculation we have two condition one it's no stack in operator and one value in value stack 
            if (operator_Stack.Count == 0)
            {
                if (value_Stack.Count !=1)
                {
                    throw new ArgumentException("wrong format");
                }
                int result = value_Stack.Pop();
                return result;
            }
            // another condition it we need plus or minus final 2 answer 
            else if (value_Stack.Count != 2 || operator_Stack.Count != 1)
            {
                throw new ArgumentException("wrong format");
            }
            return addOrminus(value_Stack.Pop(), value_Stack.Pop(), value_Stack, operator_Stack);
        }
        /// <summary>
        /// This is helper method for checking wheather it is / or * sign
        /// </summary>
        /// <param name="value_Stack"></param>
        /// <param name="operator_Stack"></param>
        /// <returns></returns>
        public static Boolean checkMultiOrDivideOperator(Stack<int> value_Stack, Stack<string> operator_Stack)
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
        public static int multiOrdivide(int first_Value, int second_Value, Stack<int> value_Stack, Stack<string> operator_Stack)
        {
            // check the value 
            zeroOrEmpty(first_Value, second_Value, value_Stack, operator_Stack);
            int result = 0;
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
        public static void zeroOrEmpty(int first_Value, int second_Value, Stack<int> value_Stack, Stack<string> operator_Stack)
        {
            if (operator_Stack.Peek() == "/")
            {
                if (value_Stack.Count == 0 || second_Value == 0)
                {
                    throw new ArgumentException("wrong format");
                }
            }

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
                throw new ArgumentException();
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
        public static int addOrminus(int first_Value, int second_Value, Stack<int> value_Stack, Stack<string> operator_Stack)
        {
            // check the edge case 
            zeroOrEmpty(first_Value, second_Value, value_Stack, operator_Stack);
            // check the symbol in stack is  plus or minus 
            if (operator_Stack.Peek() == "+" || operator_Stack.Peek() == "-")
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
            return int.MaxValue;
        }
        /// <summary>
        /// This function is check the value stack less than 2 or not
        /// </summary>
        /// <param name="value_Stack"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void lessthan2(Stack<int> value_Stack)
        {
            if (value_Stack.Count < 2)
            {
                throw new ArgumentException("wrong formar");
            }
        }
    }
}


