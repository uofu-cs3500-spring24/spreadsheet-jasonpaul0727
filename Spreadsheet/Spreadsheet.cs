using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SS
{
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
    /// This is the class for calculate the spreadsheet which spreadsheet consists of an infinite number of named cells.
    /// we try to create the cell in difference type of different cell. 
    /// as many times as the related cells are modified. 
    ///   Formula
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        ///   An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
        ///    spreadsheet consists of an infinite number of named cells.
        /// </summary>
        private class Cell
        {
            private string name;
            /// <summary>
            /// object content
            /// </summary>
            public object content
            {
                get;
                set;
            }
            private object value;
            /// <summary>
            /// cell type with double
            /// </summary>
            /// <param name="name"></param>
            /// <param name="content"></param>
            public Cell(string name, double content)
            {
                this.name = name;
                this.content = content;
                value = content;
            }
            /// <summary>
            /// cell type with fomula
            /// </summary>
            /// <param name="name"></param>
            /// <param name="content"></param>
            public Cell(string name, Formula content)
            {
                this.name = name;
                this.content = content;
                value = content;
            }
            /// <summary>
            /// cell type with string
            /// </summary>
            /// <param name="name"></param>
            /// <param name="content"></param>
            public Cell(string name, string content)
            {
                this.name = name;
                this.content = content;
                value = content;
            }
        }
        // create the dict <string, Cell>
        private Dictionary<string, Cell> cells = new Dictionary<string, Cell>();
        DependencyGraph DG = new DependencyGraph();
        /// <summary>
        /// Returns an Enumerable that can be used to enumerates 
        /// the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override object GetCellContents(string name)
        {
            // check invalid type
            if (!variableCheck(name))
            {
                throw new InvalidNameException();
            }
            if (cells.ContainsKey(name))
            {
                return cells[name].content;
            }
            return "";
        }
        /// <summary>
        /// Returns an Enumerable that can be used to enumerates 
        /// the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            HashSet<string> cellSet = new HashSet<string>();
            // use for loop visit each cell
            foreach (string name in cells.Keys)
            {
                if (cells.TryGetValue(name, out Cell? value) && value.content != "")
                {
                    cellSet.Add(name);
                }
            }
            return cellSet;
        }

        /// <summary>
        ///  Set the contents of the named cell to the given number.  
        /// </summary>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="number"> The new contents/value </param>
        /// 
        /// <returns>
        ///   <para>
        ///      The method returns a set consisting of name plus the names of all other cells whose value depends, 
        ///      directly or indirectly, on the named cell.
        ///   </para>
        /// 
        ///   <para>
        ///      For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///      set {A1, B1, C1} is returned.
        ///   </para>
        /// </returns>
        public override ISet<string> SetCellContents(string name, double number)
        {
            // check invaid format
            if (!variableCheck(name) || ReferenceEquals(null, name))
            {
                throw new InvalidNameException();
            }
            // check whether cell contain the name before and recalcuate the relationship 
            Cell c = new Cell(name, number);
            cells[name] = c;
            HashSet<string> cellSet = new HashSet<string>(GetCellsToRecalculate(name));
            DG.ReplaceDependees(name, new HashSet<string>());
            return cellSet;

        }
        /// <summary>
        /// The contents of the named cell becomes the text.  
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException"> 
        ///   If text is null, throw an ArgumentNullException.
        /// </exception>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="text"> The new content/value of the cell</param>
        /// 
        /// <returns>
        ///   The method returns a set consisting of name plus the names of all 
        ///   other cells whose value depends, directly or indirectly, on the 
        ///   named cell.
        /// 
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned.
        ///   </para>
        /// </returns>
        public override ISet<string> SetCellContents(string name, string text)
        {
            // check the invaid form of text and name 
            if (!variableCheck(name) || ReferenceEquals(null, name))
            {
                throw new InvalidNameException();
            }
            if (ReferenceEquals(null, text))
            {
                throw new ArgumentException();
            }
            object n = GetCellContents(name);
            // check whether cell contain the name before and recalcuate the relationship 
            DG.ReplaceDependees(name, new HashSet<string>());
            Cell c = new Cell(name, text);
            cells[name] = c;
            ISet<string> cellSet = new HashSet<string>(GetCellsToRecalculate(name));
            return cellSet;
        }
        /// <summary>
        /// Set the contents of the named cell to the formula.  
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException"> 
        ///   If formula parameter is null, throw an ArgumentNullException.
        /// </exception>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <exception cref="CircularException"> 
        ///   If changing the contents of the named cell to be the formula would 
        ///   cause a circular dependency, throw a CircularException.  
        ///   (NOTE: No change is made to the spreadsheet.)
        /// </exception>
        /// 
        /// <param name="name"> The cell name</param>
        /// <param name="formula"> The content of the cell</param>
        /// 
        /// <returns>
        ///   <para>
        ///     The method returns a Set consisting of name plus the names of all other 
        ///     cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///   <para> 
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned.
        ///   </para>
        /// 
        /// </returns>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            // check the variable of the formula
            if (ReferenceEquals(null, formula))
            {
                throw new ArgumentException();
            }
            if (ReferenceEquals(null, name) || !variableCheck(name))
            {
                throw new InvalidNameException();
            }
            // storing the old value and denpendent
            object old_Value = GetCellContents(name);
            HashSet<string> s = new HashSet<string>(DG.GetDependees(name));
            ISet<string> cellSet;
            // replace the dependent
            DG.ReplaceDependees(name, formula.GetVariables());
            try
            {
                cellSet = new HashSet<string>(GetCellsToRecalculate(name));
            }
            // if catch the exception look back type of the old value ad put back to the cell
            catch (CircularException)
            {
                if (old_Value.GetType().Equals(typeof(double)))
                {
                    if (cells.ContainsKey(name))
                    {
                        cells[name].content = Convert.ToDouble(old_Value);
                    }
                    DG.ReplaceDependees(name, s);

                }
                else if (old_Value.GetType().Equals(typeof(string)))
                {
                    if (cells.ContainsKey(name))
                    {
                        cells[name].content = Convert.ToString(old_Value);
                    }
                    DG.ReplaceDependees(name, s);
                }
                else
                {
                    if (cells.ContainsKey(name))
                    {
                        cells[name].content = (Formula)(old_Value);
                    }
                    DG.ReplaceDependees(name, s);
                }
                throw new CircularException();
            }
            // check the whether cell contain the value before 
            Cell c = new Cell(name, formula);
            cells[name] = c;
            return cellSet;
        }
        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell. 
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException"> 
        ///   If the name is null, throw an ArgumentNullException.
        /// </exception>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"></param>
        /// <returns>
        ///   Returns an enumeration, without duplicates, of the names of all cells that contain
        ///   formulas containing name.
        /// 
        ///   <para>For example, suppose that: </para>
        ///   <list type="bullet">
        ///      <item>A1 contains 3</item>
        ///      <item>B1 contains the formula A1 * A1</item>
        ///      <item>C1 contains the formula B1 + A1</item>
        ///      <item>D1 contains the formula B1 - C1</item>
        ///   </list>
        /// 
        ///   <para>The direct dependents of A1 are B1 and C1</para>
        /// 
        /// </returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {

            return DG.GetDependents(name);
        }
        /// <summary>
        /// check it is valid of the type of variable
        /// </summary>
        /// <param name="s"></param>
        /// <returns> valid format or not </returns>
        private static bool variableCheck(string s)
        {
            if (Regex.IsMatch(s, @"^[a-zA-Z_](?:[a-zA-Z_]|\d)*$") == false)
            {
                return false;
            }
            return true;
        }
    }
}