using Microsoft.VisualBasic;
using NuGet.Frameworks;
using SpreadsheetUtilities;
using SS;
using System.Runtime.Intrinsics.X86;
using System.Xml;
using System.Xml.Linq;

[TestClass()]
public class AS5_Tests
{
    // Tests Constructor
    [TestMethod]
    public void testTheSpreadSheetConstructor()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "x");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void testTheSpreadSheet1Fail()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("//gyh", "x");
    }
    [TestMethod]
    public void testTheSpreadSheetConstructor1WithValue()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "hello");
        Assert.AreEqual("hello", s.GetCellContents("A1"));
        Assert.AreEqual("", s.GetCellContents("b1"));
        s.SetContentsOfCell("C1", "");
        s.SetContentsOfCell("D1", "6");
        s.SetContentsOfCell("F1", "= D1");
        s.SetContentsOfCell("E1", "= D1+F1");
        Assert.AreEqual(6.0, s.GetCellValue("F1"));
        Assert.AreEqual(12.0, s.GetCellValue("E1"));
    }
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void TesttheCircular()
    {
        Spreadsheet sheet = new Spreadsheet();
        DependencyGraph D = new DependencyGraph();
        Formula f0 = new Formula("778");
        Formula f1 = new Formula("a1+a2");
        Formula f2 = new Formula("a2+a1");
        sheet.SetContentsOfCell("a1", "778");
        sheet.SetContentsOfCell("a2", "hello");
        sheet.SetContentsOfCell("a3", "=a2");
        sheet.SetContentsOfCell("a1", "=a1+a2");
        sheet.SetContentsOfCell("a2", "=a2+a1");
        sheet.SetContentsOfCell("a3", "=a3+a1");
    }
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void TesttheCircularString()
    {
        Spreadsheet sheet = new Spreadsheet();
        DependencyGraph D = new DependencyGraph();
        sheet.SetContentsOfCell("a1", "Thanks");
        sheet.SetContentsOfCell("a2", "hello");
        sheet.SetContentsOfCell("a3", "=a2");
        sheet.SetContentsOfCell("a1", "=a1+a2");
        sheet.SetContentsOfCell("a2", "=a2+a1");
        sheet.SetContentsOfCell("a3", "=a3+a1");
    }
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void TesttheCircularFormula()
    {
        Spreadsheet sheet = new Spreadsheet();
        DependencyGraph D = new DependencyGraph();
        sheet.SetContentsOfCell("a5", "=15");
        sheet.SetContentsOfCell("a1", "=a5");
        sheet.SetContentsOfCell("a2", "=a5-1 ");
        sheet.SetContentsOfCell("a3", "=a2");
        sheet.SetContentsOfCell("a1", "=a1+a2");
        sheet.SetContentsOfCell("a2", "=a2+a1");
    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void TesttheCircularFormulaInvalid()
    {
        Spreadsheet sheet = new Spreadsheet();
        DependencyGraph D = new DependencyGraph();
        sheet.SetContentsOfCell("A/5", "=15");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TesttheCircularFormulaInvalidValue()
    {
        Spreadsheet sheet = new Spreadsheet();
        DependencyGraph D = new DependencyGraph();
        sheet.SetContentsOfCell("A5", "=/-]5");
    }
    [TestMethod]
    public void testTheSpreadSheetConstructor2()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
        ss.SetContentsOfCell("A1", "hello");
        Assert.AreEqual("hello", ss.GetCellContents("A1"));
        Assert.AreEqual("", ss.GetCellContents("B1"));
        ss.SetContentsOfCell("C1", "");
        ss.SetContentsOfCell("D1", "6");
        ss.SetContentsOfCell("F1", "= D1");
        ss.SetContentsOfCell("E1", "= D1+F1");
        Assert.AreEqual(6.0, ss.GetCellValue("F1"));
        Assert.AreEqual(12.0, ss.GetCellValue("E1"));

    }
    // save the test 
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Save1()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        ss.Save("save.txt");
        ss = new Spreadsheet("save.txt", s => true, s => s, "version");
    }
    [TestMethod]
    public void save2WithChceckVersion()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
        ss.Save("save.txt");
        Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("save.txt"));
    }
    [TestMethod]
    public void savewith2File()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
        ss.Save("save.txt");
        ss.Save("save1.txt");
        Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("save1.txt"));
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
        Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("saveRead.txt"));
        AbstractSpreadsheet sss = new Spreadsheet("saveRead.txt", s => true, s => s, "hello");
        Assert.AreEqual("abc", sss.GetCellContents("a1"));
        Assert.AreNotEqual("Fine", ss.GetSavedVersion("save.txt"));
    }
    [TestMethod]
    public void GetXML()
    {
        AbstractSpreadsheet s = new Spreadsheet(s => true, s => s, "hello");
        s.SetContentsOfCell("a1", "abc");
        s.SetContentsOfCell("a2", "1.0");
        s.SetContentsOfCell("a3", "=a2");
        AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
        ss.SetContentsOfCell("a1", "abc");
        ss.SetContentsOfCell("a2", "1.0");
        ss.SetContentsOfCell("a3", "=a2");
        AbstractSpreadsheet sss = new Spreadsheet(s => true, s => s, "hello");
        sss.SetContentsOfCell("a1", "abc");
        sss.SetContentsOfCell("a2", "1.0");
        sss.SetContentsOfCell("a3", "=a1");
        string xml = ss.GetXML();
        Assert.AreEqual(ss.GetXML(), xml);
        Assert.AreNotEqual(sss.GetXML(), xml);
    }
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void GetXMLException()
    {
        AbstractSpreadsheet s = new Spreadsheet(s => true, s => s, "hello");
        AbstractSpreadsheet ss = new Spreadsheet(s => false, s => "sfce", "hel");
        Assert.AreEqual("()o", new Spreadsheet().GetSavedVersion("()o.txt"));
        ss.Save("save.txt");
    }
    [TestMethod]
    public void equalFormula1()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "4.1");
        s.SetContentsOfCell("B1", "= A1 + 5.2");
        Assert.AreEqual(9.3, s.GetCellValue("B1"));
    }
    /// <summary>
    /// test throw the exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void equalFormulaException()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "4.1");
        s.SetContentsOfCell("B1", "= A1 + -/");
    }
    /// <summary>
    /// Test exception of wrong format of sting
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellContentsException4()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("/2d", "ss");
    }
    /// <summary>
    /// Test exception of wrong format of sting
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellContentsExceptiondouble()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("/2d", "5.8");
    }
    /// <summary>
    /// Get cell content of the 
    /// </summary>
    [TestMethod]
    public void GetEveryValueFromKey()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("a7", "1");
        sheet.SetContentsOfCell("ai7", "2");
        HashSet<string> strings = new HashSet<string>();
        strings.Add("a7");
        strings.Add("ai7");
        Assert.AreEqual(sheet.GetNamesOfAllNonemptyCells().First(), strings.First());
    }
    /// <summary>
    /// Test exception of circular
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellValueException()
    {
        Spreadsheet sheet = new Spreadsheet();
        HashSet<string> strings = new HashSet<string>();
        strings.Add("a//7");
        Assert.AreEqual(sheet.GetCellContents(strings.First()), "");
    }
    /// <summary>
    /// Test exception of empty and InvalidName
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellValueException2()
    {
        Spreadsheet sheet = new Spreadsheet();
        HashSet<string> strings = new HashSet<string>();
        strings.Add("a//7");
        Assert.AreEqual(sheet.GetCellValue(strings.First()), "");
    }
    /// <summary>
    /// Test  empty  string
    /// </summary>
    [TestMethod]
    public void GetCellValue2()
    {
        Spreadsheet sheet = new Spreadsheet();
        HashSet<string> strings = new HashSet<string>();
        strings.Add("a7");
        Assert.AreEqual(sheet.GetCellValue(strings.First()), "");
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
        sheet.SetContentsOfCell("a1", "4");
        sheet.SetContentsOfCell("a2", "5");
        sheet.SetContentsOfCell("a2", "=a3+a2");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void CheckFormulaException()
    {
        AbstractSpreadsheet s = new Spreadsheet(s => s[0] != 'A', s => s,"hello");
        s.SetContentsOfCell("B1", "=A1+A2");
    }
}