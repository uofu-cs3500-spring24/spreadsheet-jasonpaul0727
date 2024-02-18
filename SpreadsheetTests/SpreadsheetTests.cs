using SpreadsheetUtilities;
using SS;
using System.Xml;
using System.Xml.Linq;

[TestClass()]
public class AS5_Tests
{
    // Tests IsValid
    [TestMethod]
    public void I1()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "x");
    }
    [TestMethod, Timeout(2000)]
    public void N1()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "hello");
        Assert.AreEqual("", s.GetCellContents("b1"));
    }

    [TestMethod]
    public void N2()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
        ss.SetContentsOfCell("B1", "hello");
        Assert.AreEqual("hello", ss.GetCellContents("b1"));
    }

    [TestMethod]
    public void Nt3()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("a1", "5");
        s.SetContentsOfCell("A1", "6");
        s.SetContentsOfCell("B1", "= a1");
        Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
    }

    [TestMethod]
    public void N4()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
        ss.SetContentsOfCell("a1", "5");
        ss.SetContentsOfCell("A1", "6");
        ss.SetContentsOfCell("B1", "= a1");
        Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
    }
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Read1()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        ss.Save("save3.txt");
        ss = new Spreadsheet("save3.txt", s => true, s => s, "version");
    }
    [TestMethod]
    public void VersionTest()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
        ss.Save("save.txt");
        Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("save.txt"));
    }
    [TestMethod]
    public void SaveAndRead()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
        ss.SetContentsOfCell("a1", "abc");
        ss.SetContentsOfCell("a2", "1.0");
        ss.SetContentsOfCell("a3", "=a2");
        ss.Save("saveRead.txt");
        Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("save.txt"));
        AbstractSpreadsheet sss = new Spreadsheet("saveRead.txt", s => true, s => s, "hello");
        Assert.AreEqual("abc", sss.GetCellContents("a1"));
    }
    [TestMethod]
    public void GetXML()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
        ss.SetContentsOfCell("a1", "abc");
        ss.SetContentsOfCell("a2", "1.0");
        ss.SetContentsOfCell("a3", "=a2");
        AbstractSpreadsheet sss = new Spreadsheet(s => true, s => s, "hello");
        sss.SetContentsOfCell("a1", "abc");
        sss.SetContentsOfCell("a2", "1.0");
        sss.SetContentsOfCell("a3", "=a2");
        string xml = ss.GetXML();
        Assert.AreEqual(sss.GetXML(), xml);
    }
    [TestMethod]
    public void wrongTypeTrue()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "2.1");
        ss.SetContentsOfCell("A2", "2.1");
        ss.SetContentsOfCell("A3", "=A1+B1");
        ss.SetContentsOfCell("B1", "=A1");
        Assert.AreNotEqual(ss.GetCellValue("B1"), new FormulaError(""));
    }
    [TestMethod]
    public void equalFormula1()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "4.1");
        ss.SetContentsOfCell("B1", "= A1 + 5.2");
        Assert.AreEqual(9.3, ss.GetCellValue("B1"));
    }
}