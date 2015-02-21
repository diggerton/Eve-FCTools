using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FCTools.Utilities.Tests
{
    [TestClass]
    public class Line
    {
        [TestMethod]
        public void LineParse()
        {
            var m = @"[ 2015.02.08 22:12:13 ] malcotch > play sigue sigue sputnik";

            var line = new Utilities.Models.Line(m);

            Assert.IsTrue(line.Timestamp > DateTime.MinValue);
            Assert.AreEqual("malcotch", line.Sender);
            Assert.AreEqual("play sigue sigue sputnik", line.Message);
        }

        [TestMethod]
        public void LineParse_BadTimestampFormat()
        {
            var m = @"[ 2015.02.08 28:12:13 ] malcotch > play sigue sigue sputnik";

            var line = new Utilities.Models.Line(m);

            Assert.IsTrue(line.Timestamp == DateTime.MinValue);
            Assert.AreEqual("malcotch", line.Sender);
            Assert.AreEqual("play sigue sigue sputnik", line.Message);
        }

        [TestMethod]
        public void LineParse_NoMatch()
        {
            var m = @"[ 2015.:12:13 ] malctch > play sgue sputnik";

            var line = new Utilities.Models.Line(m);

            Assert.IsTrue(line.Timestamp == DateTime.MinValue);
            Assert.AreEqual(string.Empty, line.Sender);
            Assert.AreEqual(string.Empty, line.Message);
        }
    }
}
