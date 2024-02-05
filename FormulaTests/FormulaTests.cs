using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using SpreadsheetUtilities;
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
namespace FormulaTests
{
    /// <summary>
    /// this is tester class for formula 
    /// </summary>
    [TestClass]
    public class PS3Tests()
    {
        // test Excepetion case first
        /// <summary>
        /// this test is oing for testing empty string
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void constructorTestempty()
        {
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
        /// <summary>
        /// create the  test  varialbe for correct in constructor
        /// </summary>
        [TestMethod]
        public void formulafirst()
        {
            Formula f = new Formula("x5+7");
            Assert.AreEqual(8, (double)f.Evaluate(s => 1));
        }
        /// <summary>
        /// create the  test  varialbe for correct in constructor
        /// </summary>
        [TestMethod]
        public void formula3()
        {
            Formula f = new Formula("x5*x5*x5");
            Assert.AreEqual(1, (double)f.Evaluate(s => 1));
        }
        /// <summary>
        /// create the  test  varialbe for correct in constructor
        /// </summary>
        [TestMethod]
        public void formula4()
        {
            Formula f = new Formula("x5*x5(x5-x5)");
            Assert.AreEqual(0, (double)f.Evaluate(s => 1));
        }
        /// <summary>
        /// create the  test  varialbe for correct in constructor
        /// </summary>
        [TestMethod]
        public void formula1()
        {
            Formula f = new Formula("(x5+7)");
            Assert.AreEqual(8, (double)f.Evaluate(s => 1));
            Formula f2 = new Formula("(x5+7-7)+(x5)");
            Assert.AreEqual(2, (double)f2.Evaluate(s => 1));
            Formula f3 = new Formula("((((((x5+7-7))))))+(x5)*1/1");
            Assert.IsTrue(f2 != f3);
        }
        /// <summary>
        /// test for hashcode
        /// </summary>
        [TestMethod()]
        public void hashcodetest()
        {
            Formula f1 = new Formula("1+1-1*1/1.00");
            Formula f2 = new Formula("1+1-1*1/1");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }
        /// <summary>
        /// test for  equal method
        /// </summary>
        [TestMethod()]
        public void equalsNon()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(f1.Equals(null));
            Assert.IsTrue(f1.Equals(f2));
        }

        /// <summary>
        /// create the  test expression for correct in constructor
        /// </summary>
        [TestMethod]
        public void formula2()
        {
            Formula f = new Formula("x5*7*7/7/7");
            Formula f2 = new Formula("x5*7*7/7/7");
            Assert.AreEqual(1, (double)f.Evaluate(s => 1));
            Assert.IsTrue(f == f2);
        }
        /// test wrong case format for evaluate function
        /// <summary>
        /// test wrong case format for evaluate
        /// </summary>
        [TestMethod()]
        public void divideZero()
        {
            Formula f = new Formula("(1+1)/(x1-1)");
            Assert.IsInstanceOfType(f.Evaluate(s => 1), typeof(FormulaError));
        }
        /// test wrong case format for evaluate function
        /// <summary>
        /// test wrong case format for evaluate
        /// </summary>
        [TestMethod()]
        public void divideZerointeger()
        {
            Formula f = new Formula("(1+1)/(1-1)");
            Assert.IsInstanceOfType(f.Evaluate(s => 1), typeof(FormulaError));
        }
        /// <summary>
        /// test wrong case format for evaluate
        /// </summary>
        [TestMethod()]
        public void formatError1()
        {
            Formula f = new Formula("1+x5");
            Assert.IsInstanceOfType(f.Evaluate(s => { throw new ArgumentException("-wrong variable type"); }), typeof(FormulaError));
        }
        /// <summary>
        /// test wrong case format for evaluate
        /// </summary>
        [TestMethod()]
        public void Hashcode()
        {
            Formula f1 = new Formula("1+1");
            Formula f2 = new Formula("1+1");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }
        /// <summary>
        /// test get variable method
        /// </summary>
        [TestMethod()]
        public void getvariableempty()
        {
            Formula f = new Formula("2*5");
            Assert.AreEqual(0, f.GetVariables().ToArray().Length);
        }
        /// <summary>
        /// test get variable method
        /// </summary>
        [TestMethod()]
        public void getvariablenonempty()
        {
            Formula f = new Formula("2*c1");
            Assert.AreEqual(1, f.GetVariables().ToArray().Length);
        }
    }
}