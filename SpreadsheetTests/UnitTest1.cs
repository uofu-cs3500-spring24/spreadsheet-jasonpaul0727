using SS;
using System;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTest
    {

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void GetCellContentsException()
        {
            Spreadsheet sheet = new Spreadsheet();

            sheet.GetCellContents("1");
        }
        [TestMethod]
        public void GetCellContents()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("a7", 1);
            sheet.SetCellContents("ai7", 2);
            Assert.AreEqual(sheet.GetCellContents("a7"), 1);
        }
    }
}