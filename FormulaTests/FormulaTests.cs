using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using SpreadsheetUtilities;


namespace FormulaTests
{
    [TestClass]
    public class PS3Tests()
    {
        // test Excepetion case first
        /// <summary>
        /// this test is oing for testing empty string
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void constructorTestempty() {
            Formula f = new Formula("", s => s.ToUpper(), s => true);
        }
        /// <summary>
        /// test wrong adding format
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void constructorTestwrongFormat1()
        {
            Formula f = new Formula("x+", s => s.ToUpper(), s => true);
        }
        /// <summary>
        /// testing wrong right parathesis
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void constructorTestwrongFormat2()
        {
            Formula f = new Formula("x+6)", s => s.ToUpper(), s => true);
        }
        /// <summary>
        /// testing wrong left parathesis
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void constructorTestwrongFormat3()
        {
            Formula f = new Formula("((x+6)", s => s.ToUpper(), s => true);
        }
        /// <summary>
        /// testing wrong parathesis
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void constructorTestwrongFormat4()
        {
            Formula f = new Formula("((x+6(6))", s => s.ToUpper(), s => true);
        }
        /// <summary>
        /// testing wrong operator in constructor
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void constructorTestwrongFormat5()
        {
            Formula f = new Formula("((x+6(*6))", s => s.ToUpper(), s => true);
        }
        /// <summary>
        /// testing wrong  sysmbol in constructor
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void constructorTestwrongFormat6()
        {
            Formula f = new Formula("(5, +)", s => s.ToUpper(), s => true);
        }
        /// <summary>
        /// testing wrong  format in constructor
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void constructorTestwrongFormat7()
        {
            Formula f = new Formula("(5+7 7)", s => s.ToUpper(), s => true);
        }
        /// <summary>
        /// testing wrong  format  of variable
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void constructorTestwrongFormat8()
        {
            Formula f = new Formula("(5x+7u)", s => s.ToUpper(), s => true);
        }
        /// <summary>
        /// testing wrong  format  of variable
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void constructorTestwrongFormat9()
        {
            Formula f = new Formula("5-x12", s => s, s => (s == "y"));
        }
        /// test correct 
        
        /// <summary>
        /// create the test for correct in constructor
        /// </summary>
        [TestMethod]
        public void constructorSimple()
        {
            Formula f = new Formula("(5+7)", s => s.ToUpper(), s => true);
        }
        /// <summary>
        /// create the  test  varialbe for correct in constructor
        /// </summary>
        [TestMethod]
        public void constructorvariable()
        {
            Formula f = new Formula("x5+7");
            Assert.AreEqual(8, (double)f.Evaluate(s => 1));
        }

        /// <summary>
        /// create the  test  varialbe for correct in constructor
        /// </summary>
        [TestMethod]
        public void formula()
        {
            Formula f = new Formula("x5+7");
            Assert.AreEqual(8, (double)f.Evaluate(s => 1));
        }
    }
}