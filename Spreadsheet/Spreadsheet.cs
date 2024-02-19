using SpreadsheetUtilities;
using SS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml;


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
            public string name
            { get; set; }

            /// <summary>
            /// object content
            /// </summary>
            public object content
            {
                get;
                set;
            }
            public object value;
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
        private Dictionary<string, Cell> cells;
        private DependencyGraph DG;
        public override bool Changed { get; protected set; }
        public Spreadsheet() : this(s => true, s => s, "default")
        {
            cells = new Dictionary<string, Cell>();
            DG = new DependencyGraph();
            Changed = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>();
            DG = new DependencyGraph();

        }
        public Spreadsheet(string filePath, Func<string, bool> isValid, Func<string, string> normalize, string version) : this(isValid, normalize, version)
        {
            string CellName = "";
            cells = new Dictionary<string, Cell>();
            DG = new DependencyGraph();
            if (filePath == null || GetSavedVersion(filePath) != version) throw new SpreadsheetReadWriteException("Wrong Format of Spreadsheet");
            
                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "contents":
                                    reader.Read();
                                    string s = reader.ReadContentAsString();
                                    s = s.Trim();
                                    SetContentsOfCell(CellName, s);
                                    break;
                                case "name":
                                    reader.Read();
                                    CellName = reader.Value;
                                    break;
                                case "cell":
                                    break;
                                case "spreadsheet":
                                    break;
                            }
                        }

                    }
                }
            }

            // read xml 
            // read every cell from Cell Xml 
            // validate name of every cell
            // add every cell to Spreadsheet using setContentOf cell . 
        /// <summary>
        /// Returns an Enumerable that can be used to enumerates 
        /// the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override object GetCellContents(string name)
        {
            // check invalid type
            if (!variableCheck(Normalize(name)) || ReferenceEquals(null, Normalize(name)))
            {
                throw new InvalidNameException();
            }
            if (cells.ContainsKey(Normalize(name)))
            {
                return cells[Normalize(name)].content;
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
        protected override IList<string> SetCellContents(string name, double number)
        {
            // check whether cell contain the name before and recalcuate the relationship 
            Cell c = new Cell(name, number);
            cells[name] = c;
            List<string> cellSet = new List<string>(GetCellsToRecalculate(name));
            DG.ReplaceDependees(name, new HashSet<string>());
            return cellSet;

        }
        /// <summary>
        /// The contents of the named cell becomes the text.  
        /// </summary>
        /// 
        /// <requires> 
        ///   The name parameter must be valid/non-empty ""
        /// </requires>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>       
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="text"> The new content/value of the cell</param>
        /// 
        /// <returns>
        ///   <para>
        ///       This method returns a LIST consisting of the passed in name followed by the names of all 
        ///       other cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///
        ///   <para>
        ///       The order must correspond to a valid dependency ordering for recomputing
        ///       all of the cells, i.e., if you re-evaluate each cell in the order of the list,
        ///       the overall spreadsheet will be consistently updated.
        ///   </para>
        ///
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned, i.e., A1 was changed, so then A1 must be 
        ///     evaluated, followed by B1 re-evaluated, followed by C1 re-evaluated.
        ///   </para>
        /// </returns>
        protected override IList<string> SetCellContents(string name, string text)
        {
            object n = GetCellContents(name);
            // check whether cell contain the name before and recalcuate the relationship 
            DG.ReplaceDependees(name, new HashSet<string>());
            DG.ReplaceDependents(name, new HashSet<string>());
            Cell c = new Cell(name, text);
            cells[name] = c;
            List<string> cellSet = new List<string>(GetCellsToRecalculate(name));
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
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            // storing the old value and denpendent
            object old_Value = GetCellContents(name);
            HashSet<string> s = new HashSet<string>(DG.GetDependees(name));
            List<string> cellSet;
            // replace the dependent
            DG.ReplaceDependees(name, formula.GetVariables());
            IEnumerable<string> Check = formula.GetVariables();
            foreach (string str in Check)
            {
                if (!IsValid(str))
                {
                    throw new FormulaFormatException("wrong format" + str);
                }
            }
            try
            {
                cellSet = new List<string>(GetCellsToRecalculate(name));
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
            if (!Regex.IsMatch(s, @"^[a-zA-Z_](?:[a-zA-Z_]|\d)*$"))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        ///   <para>Sets the contents of the named cell to the appropriate value. </para>
        ///   <para>
        ///       First, if the content parses as a double, the contents of the named
        ///       cell becomes that double.
        ///   </para>
        ///
        ///   <para>
        ///       Otherwise, if content begins with the character '=', an attempt is made
        ///       to parse the remainder of content into a Formula.  
        ///       There are then three possible outcomes:
        ///   </para>
        ///
        ///   <list type="number">
        ///       <item>
        ///           If the remainder of content cannot be parsed into a Formula, a 
        ///           SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       </item>
        /// 
        ///       <item>
        ///           If changing the contents of the named cell to be f
        ///           would cause a circular dependency, a CircularException is thrown,
        ///           and no change is made to the spreadsheet.
        ///       </item>
        ///
        ///       <item>
        ///           Otherwise, the contents of the named cell becomes f.
        ///       </item>
        ///   </list>
        ///
        ///   <para>
        ///       Finally, if the content is a string that is not a double and does not
        ///       begin with an "=" (equal sign), save the content as a string.
        ///   </para>
        /// </summary>
        ///
        /// <exception cref="InvalidNameException"> 
        ///   If the name parameter is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <exception cref="SpreadsheetUtilities.FormulaFormatException"> 
        ///   If the content is "=XYZ" where XYZ is an invalid formula, throw a FormulaFormatException.
        /// </exception>
        /// 
        /// <exception cref="CircularException"> 
        ///   If changing the contents of the named cell to be the formula would 
        ///   cause a circular dependency, throw a CircularException.  
        ///   (NOTE: No change is made to the spreadsheet.)
        /// </exception>
        /// 
        /// <param name="name"> The cell name that is being changed</param>
        /// <param name="content"> The new content of the cell</param>
        /// 
        /// <returns>
        ///       <para>
        ///           This method returns a list consisting of the passed in cell name,
        ///           followed by the names of all other cells whose value depends, directly
        ///           or indirectly, on the named cell. The order of the list MUST BE any
        ///           order such that if cells are re-evaluated in that order, their dependencies 
        ///           are satisfied by the time they are evaluated.
        ///       </para>
        ///
        ///       <para>
        ///           For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///           list {A1, B1, C1} is returned.  If the cells are then evaluate din the order:
        ///           A1, then B1, then C1, the integrity of the Spreadsheet is maintained.
        ///       </para>
        /// </returns>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            string names = Normalize(name);
            IList<string> cellsContents = new List<string>();
            if (name == null || !variableCheck(name) || IsValid(Normalize(name)) != true)
            {
                throw new InvalidNameException();
            }
            if (double.TryParse(content, out double result))
            {
                cellsContents = new List<string>(SetCellContents(Normalize(name), result));
            }
            else if (content != "" && content[0] == '=')
            {

                string formula = content.Substring(1, content.Length - 1);
                Formula f = new Formula(Normalize(formula));
                cellsContents = new List<string>(SetCellContents(Normalize(name), f));
            }
            else
            {
                cellsContents = new List<string>(SetCellContents(Normalize(name), content.ToString()));
            }
            Changed = true;
            foreach (string s in cellsContents)
            {
                if (cells.ContainsKey(s))
                {
                    if (cells[s].content.GetType() == typeof(Formula))
                    {
                        Formula F = (Formula)cells[s].content;
                        cells[s].value = F.Evaluate(lookup);
                    }
                }
            }
            return cellsContents;
        }
        /// <summary>
        ///   Look up the version information in the given file. If there are any problems opening, reading, 
        ///   or closing the file, the method should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        /// 
        /// <remarks>
        ///   In an ideal world, this method would be marked static as it does not rely on an existing SpreadSheet
        ///   object to work; indeed it should simply open a file, lookup the version, and return it.  Because
        ///   C# does not support this syntax, we abused the system and simply create a "regular" method to
        ///   be implemented by the base class.
        /// </remarks>
        /// 
        /// <exception cref="SpreadsheetReadWriteException"> 
        ///   1Thrown if any problem occurs while reading the file or looking up the version information.
        /// </exception>
        /// 
        /// <param name="filename"> The name of the file (including path, if necessary)</param>
        /// <returns>Returns the version information of the spreadsheet saved in the named file.</returns>
        public override string GetSavedVersion(string filename)
        {
            string result=" ";
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            if (reader.Name == "spreadsheet")
                            {
                                 result = reader["version"];
                            }
                        }
                    }
                    return result;
                }
            }
            catch
            {
                throw new SpreadsheetReadWriteException("can not found File");
            }
        }
        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);
                    foreach (Cell c in cells.Values)
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", (string?)c.name);
                        switch (c.content)
                        {
                            case double:
                                writer.WriteElementString("contents", c.content.ToString());
                                break;
                            case string:
                                writer.WriteElementString("contents", (string)c.content);
                                break;
                            case Formula:
                                writer.WriteElementString("contents", "=" + ((Formula)c.content).ToString());
                                break;
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Close();
                    Changed = false;
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }
        /// <summary>
        ///   Return an XML representation of the spreadsheet's contents
        /// </summary>
        /// <returns> contents in XML form </returns>
        public override string GetXML()
        {

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            StringBuilder strbuild = new StringBuilder();
            try
            {
                using (XmlWriter writer = XmlWriter.Create(strbuild, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);
                    foreach (Cell c in cells.Values)
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", (string?)c.name);
                        switch (c.content)
                        {
                            case double:
                                writer.WriteElementString("contents", c.content.ToString());
                                break;
                            case string:
                                writer.WriteElementString("contents", (string)c.content);
                                break;
                            case Formula:
                                writer.WriteElementString("contents", "=" + ((Formula)c.content).ToString());
                                break;

                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Close();
                    Changed = false;
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
            return strbuild.ToString();

        }
        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// </summary>
        ///
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell that we want the value of (will be normalized)</param>
        /// 
        /// <returns>
        ///   Returns the value (as opposed to the contents) of the named cell.  The return
        ///   value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </returns>
        public override object GetCellValue(string name)
        {
            if (!variableCheck(name))
            {
                throw new InvalidNameException();
            }
            if (cells.TryGetValue(name, out Cell? cell))
            {
                return cells[name].value;
            }
            return "";
        }
        private double lookup(string variable)
        {
            if (cells.TryGetValue(Normalize(variable), out Cell? cell))
            {
                if (cell.value is double)
                {
                    return (double)cell.value;
                }
            }
            throw new ArgumentException("Formula Error");
        }
    }
}