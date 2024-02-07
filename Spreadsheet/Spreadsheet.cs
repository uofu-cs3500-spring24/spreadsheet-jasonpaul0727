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

    public class Spreadsheet : AbstractSpreadsheet
    {
        private class Cell
        {
            private string name;
            
            public object content
            {
                get;
                set;
            }
            private object value;
            public Cell(string name, double content)
            {
                this.name = name;
                this.content = content;
                value = content;
            }
            public Cell(string name , Formula content)
            {
                this.name = name;
                this.content = content;
                value = content;
            }
            public Cell(string name ,string content)
            {
                this.name = name;
                this.content = content;
                value = content;
            }
        }
        // create the dict <string, Cell>
        private Dictionary<string,Cell> cells = new Dictionary<string,Cell>();
        DependencyGraph DG =new DependencyGraph();
        /// <summary>
        /// Returns an Enumerable that can be used to enumerates 
        /// the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override object GetCellContents(string name)
        {
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
            foreach (string name in cells.Keys)
            {
                if (cells.TryGetValue(name,out Cell? value) && value.content!="")
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
            if (!variableCheck(name)||ReferenceEquals(null,name))
            {
                throw new InvalidNameException();
            }
            Cell c = new Cell(name, number);
            cells[name]= c;
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
            if (!variableCheck(name) || ReferenceEquals(null, name))
            {
                throw new InvalidNameException();
            }
            if(ReferenceEquals(null, text)){
                throw new ArgumentException();
            }
            object n = GetCellContents(name);
            DG.ReplaceDependees(name, new HashSet<string>());
            Cell c = new Cell(name, text);
             cells[name]= c;
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
            
             if(ReferenceEquals(null, formula))
              {
               throw new ArgumentException();
             }
            if (ReferenceEquals(null, name)||!variableCheck(name))
            {
                throw new InvalidNameException();
            }
            object old_Value = GetCellContents(name);
            HashSet<string> s = new HashSet<string>(DG.GetDependees(name));
            ISet<string> cellSet;
            DG.ReplaceDependees(name, formula.GetVariables());
            try
            {
               cellSet= new HashSet<string>(GetCellsToRecalculate(name));
            }
            catch (CircularException)
            {
                if (old_Value.GetType().Equals(typeof(double)))
                {
                   if(cells.ContainsKey(name))
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
            if (ReferenceEquals(null, name))
            {
                throw new ArgumentException();
            }
            if (ReferenceEquals(null, name) || !variableCheck(name))
            {
                throw new InvalidNameException();
            }
            return DG.GetDependents(name);
        }
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
