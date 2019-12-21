using System.Collections.Generic;
using NUnit.Framework;
using KontrolSystem.KSP.Runtime.KSPConsole;

namespace KontrolSystem.KSP.Runtime.Test {
    [TestFixture]
    public class KSPConsoleBufferTests {
        [Test]
        public void TestPrint() {
            KSPConsoleBuffer console = new KSPConsoleBuffer(25, 40);

            Assert.AreEqual(25, console.VisibleLines.Count);
            Assert.AreEqual(0, console.CursorCol);
            Assert.AreEqual(0, console.CursorRow);

            console.Print("Line1");

            Assert.AreEqual(25, console.VisibleLines.Count);
            Assert.AreEqual(5, console.CursorCol);
            Assert.AreEqual(0, console.CursorRow);

            console.Print("Line1a");

            Assert.AreEqual(25, console.VisibleLines.Count);
            Assert.AreEqual(11, console.CursorCol);
            Assert.AreEqual(0, console.CursorRow);

            console.Print("Line1b\nLine2\nLine3\nLine4\nLine5");

            Assert.AreEqual(25, console.VisibleLines.Count);
            Assert.AreEqual(5, console.CursorCol);
            Assert.AreEqual(4, console.CursorRow);

            console.Print("Line5a");

            Assert.AreEqual(25, console.VisibleLines.Count);
            Assert.AreEqual(11, console.CursorCol);
            Assert.AreEqual(4, console.CursorRow);

            List<ConsoleLine> visibleLines = console.VisibleLines;

            Assert.AreEqual("Line1Line1aLine1b", visibleLines[0].ToString().TrimEnd('\0'));
            Assert.AreEqual("Line2", visibleLines[1].ToString().TrimEnd('\0'));
            Assert.AreEqual("Line3", visibleLines[2].ToString().TrimEnd('\0'));
            Assert.AreEqual("Line4", visibleLines[3].ToString().TrimEnd('\0'));
            Assert.AreEqual("Line5Line5a", visibleLines[4].ToString().TrimEnd('\0'));
        }

        [Test]
        public void TestPrintLineWithScroll() {
            KSPConsoleBuffer console = new KSPConsoleBuffer(25, 40);

            for (int i = 0; i < 60; i++) console.PrintLine($"Line{i}");

            Assert.AreEqual(25, console.VisibleLines.Count);
            Assert.AreEqual(0, console.CursorCol);
            Assert.AreEqual(24, console.CursorRow);

            List<ConsoleLine> visibleLines = console.VisibleLines;

            for (int i = 0; i < 24; i++) {
                Assert.AreEqual($"Line{i + 36}", visibleLines[i].ToString().TrimEnd('\0'));
                Assert.AreEqual(i + 36, visibleLines[i].lineNumber);
            }
            Assert.AreEqual("", visibleLines[24].ToString().TrimEnd('\0'));
        }

        [Test]
        public void TestClear() {
            KSPConsoleBuffer console = new KSPConsoleBuffer(25, 40);

            for (int i = 0; i < 60; i++) console.PrintLine($"Line{i}");

            Assert.AreEqual(25, console.VisibleLines.Count);
            Assert.AreEqual(0, console.CursorCol);
            Assert.AreEqual(24, console.CursorRow);

            List<ConsoleLine> visibleLines = console.VisibleLines;

            for (int i = 0; i < 24; i++) {
                Assert.AreEqual($"Line{i + 36}", visibleLines[i].ToString().TrimEnd('\0'));
                Assert.AreEqual(i + 36, visibleLines[i].lineNumber);
            }
            Assert.AreEqual("", visibleLines[24].ToString().TrimEnd('\0'));

            console.Clear();

            for (int i = 0; i < 10; i++) console.PrintLine($"Line{i}");

            Assert.AreEqual(25, console.VisibleLines.Count);
            Assert.AreEqual(0, console.CursorCol);
            Assert.AreEqual(10, console.CursorRow);

            visibleLines = console.VisibleLines;

            for (int i = 0; i < 10; i++) {
                Assert.AreEqual($"Line{i}", visibleLines[i].ToString().TrimEnd('\0'));
                Assert.AreEqual(i, visibleLines[i].lineNumber);
            }
            Assert.AreEqual("", visibleLines[10].ToString().TrimEnd('\0'));
        }

    }
}
