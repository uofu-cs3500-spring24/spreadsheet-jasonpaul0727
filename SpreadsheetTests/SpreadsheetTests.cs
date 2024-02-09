using SpreadsheetUtilities;
using SS;
using System;
using System.Security.Cryptography;

namespace SpreadsheetTests
{
    /// <summary>
    /// Author:    Yanxia Bu
    /// Partner:    No
    /// Date:     1/29/2024
    /// Course:    CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and Yanxia Bu - This work may not 
    ///            be copied for use in Academic Coursework.
    ///
    /// I, Yanxia Bu, certify that I wrote this code from scratch and
    /// did not copy it in part or whole from another source.  All 
    /// references used in the completion of the assignments are cited 
    /// in my README file.
    /// 
    /// This is the tester of the formula  I test every method include the tru case and bad case 
    ///
    ///   Formula
    /// </summary>
    [TestClass]
    public class SpreadsheetTest
    {
        /// <summary>
        /// Test exception of wrong format of sting
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsException()
        {
            Spreadsheet sheet = new Spreadsheet();

            sheet.GetCellContents("1");
        }
        /// <summary>
        /// Test exception of wrong format of sting
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsException2()
        {
            Spreadsheet sheet = new Spreadsheet();

            sheet.GetCellContents("a155-");
        }
        /// <summary>
        /// Test exception of wrong format of formula
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsExceptionFormula()
        {
            Spreadsheet sheet = new Spreadsheet();
            Formula f0 = new Formula("778");
            sheet.SetCellContents("781  923", f0);
        }
        /// <summary>
        /// Test exception of wrong format of formula
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetCellContentsExceptionFormulaWrongContent()
        {
            Spreadsheet sheet = new Spreadsheet();
            Formula f0 = null;
            sheet.SetCellContents("a8", f0);
        }
        /// <summary>
        /// Test exception of wrong format of sting
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetCellContentsException3()
        {
            Spreadsheet sheet = new Spreadsheet();
            string s = null;
            sheet.SetCellContents("a7", s);
        }
        /// <summary>
        /// Test exception of wrong format of sting
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsException4()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("/2d", "ss");
        }
        /// <summary>
        /// Test exception of wrong format of sting
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsExceptiondouble()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("/2d", 5.0);
        }
        /// <summary>
        /// Test exception of circular
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void Circularsimple()
        {
            Spreadsheet sheet = new Spreadsheet();
            DependencyGraph D = new DependencyGraph();
            Formula f1 = new Formula("a1+a2");
            Formula f2 = new Formula("a2+a1");
            sheet.SetCellContents("a1", 2.0);
            sheet.SetCellContents("a1", f2);
            sheet.SetCellContents("a2", f2);

        }
        /// <summary>
        /// Test exception of circular
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void CircularsimpleString()
        {
            Spreadsheet sheet = new Spreadsheet();
            DependencyGraph D = new DependencyGraph();
            Formula f1 = new Formula("a1+a2");
            Formula f2 = new Formula("a2+a1");
            sheet.SetCellContents("a1", "2.0");
            sheet.SetCellContents("a1", f2);
            sheet.SetCellContents("a2", f2);

        }
        /// <summary>
        /// Test exception of circular
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void CircularsimpleFormula()
        {
            Spreadsheet sheet = new Spreadsheet();
            DependencyGraph D = new DependencyGraph();
            Formula f0 = new Formula("778");
            Formula f1 = new Formula("a1+a2");
            Formula f2 = new Formula("a2+a1");
            sheet.SetCellContents("a1", f0);
            sheet.SetCellContents("a1", f2);
            sheet.SetCellContents("a2", f2);
        }

        /// <summary>
        /// Test exception of circular
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void CircularComplex()
        {
            Spreadsheet sheet = new Spreadsheet();
            DependencyGraph D = new DependencyGraph();
            Formula f0 = new Formula("a3+a2");
            Formula f1 = new Formula("a1+a2");
            Formula f2 = new Formula("a2+a1");
            sheet.SetCellContents("a1", 4);
            sheet.SetCellContents("a2", 5);
            sheet.SetCellContents("a1", f0);
            sheet.SetCellContents("a1", f1);
            sheet.SetCellContents("a1", f2);
            sheet.SetCellContents("a2", f0);

        }
        /// <summary>
        /// Get cell content of the 
        /// </summary>
        [TestMethod]
        public void GetEveryValueFromKey()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("a7", 1);
            sheet.SetCellContents("ai7", 2);
            HashSet<string> strings = new HashSet<string>();
            strings.Add("a7");
            strings.Add("ai7");
            Assert.AreEqual(sheet.GetNamesOfAllNonemptyCells().First(), strings.First());
        }
        /// <summary>
        /// Get cell content of the  double
        /// </summary>
        [TestMethod]
        public void GetCellContents()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("a7", 1);
            sheet.SetCellContents("ai7", 2);
            sheet.SetCellContents("ai7", 1);
            Assert.AreEqual(sheet.GetCellContents("a7"), 1.0);
            Assert.AreEqual(sheet.GetCellContents("ai7"), 1.0);
        }
        /// <summary>
        /// Get cell content of the string
        /// </summary>
        [TestMethod]
        public void GetCellContents2()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("a7", "1");
            sheet.SetCellContents("ai7", "2");
            Assert.AreEqual(sheet.GetCellContents("a7"), "1");
            Assert.AreNotEqual(sheet.GetCellContents("a7"), "2");
        }
        /// <summary>
        /// Get cell content of the formula
        /// </summary>
        [TestMethod]
        public void GetCellContentscomplexFormula()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("a1", new Formula("1+1"));
            sheet.SetCellContents("a2", new Formula("a1+1"));
            sheet.SetCellContents("a3", new Formula("a2+a1"));
            Formula f = (Formula)sheet.GetCellContents("a1");
            Formula f2 = (Formula)sheet.GetCellContents("a2");
            Formula f3 = (Formula)sheet.GetCellContents("a3");
            Assert.AreEqual(new Formula("1+1"), f);
            Assert.AreEqual(new Formula("a1+1"), f2);
            Assert.AreNotEqual(new Formula("2"), f);
        }
        /// <summary>
        /// Get cell content of the formula StreesTest
        /// </summary>
        [TestMethod]
        public void StressTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("a1", new Formula("a2+a3"));
            sheet.SetCellContents("a1", new Formula("a3+a4"));
            sheet.SetCellContents("a1", new Formula("a5+a6"));
            sheet.SetCellContents("a1", new Formula("a6+a7"));
            sheet.SetCellContents("a1", new Formula("a8+a9"));
            sheet.SetCellContents("a1", new Formula("a9+b1"));
            sheet.SetCellContents("a1", new Formula("b1+b2"));
            sheet.SetCellContents("a1", new Formula("b3+b4"));
            sheet.SetCellContents("a1", new Formula("b4+b5"));
            sheet.SetCellContents("a1", new Formula("1"));
            Assert.AreNotEqual(sheet.GetCellContents("a7"), "1");
        }
        /// <summary>
        /// Get cell content of the formula StreesTest
        /// </summary>
        [TestMethod]
        public void StressTest1()
        {
            StressTest();
        }
        /// <summary>
        /// Get cell content of the formula StreesTest
        /// </summary>
        [TestMethod]
        public void StressTest2()
        {
            StressTest1();
        }
        /// <summary>
        /// Get cell content of the formula StreesTest
        /// </summary>
        [TestMethod]
        public void StressTest3()
        {
            StressTest2();
        }
    }
}