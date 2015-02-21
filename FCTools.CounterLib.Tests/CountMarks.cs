using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;

namespace FCTools.Utilities.Tests
{
    [TestClass]
    public class CountMarks
    {
        [TestMethod]
        public void ProcessLine()
        {
            var count = 0;
            var counter = new Utilities.CountMarks("***", "x");
            counter.PropertyChanged += (s, e) => { count = counter.Count; };
            var lines = new List<Models.Line> 
            { 
                new Utilities.Models.Line("[ 2015.02.08 22:13:06 ] Einherijer > ***"),
                new Utilities.Models.Line("﻿[ 2015.02.08 22:12:13 ] malcotch > x"),
                new Utilities.Models.Line("[ 2015.02.08 22:12:47 ] Judy Mikakka > x"),
                new Utilities.Models.Line("[ 2015.02.08 22:12:56 ] sapage1 > x")
            };

            foreach (var line in lines)
                counter.ProcessLine(line);

            Assert.AreEqual(3, counter.Count);
        }

        [TestMethod]
        public void ProcessLine_complexMessage()
        {
            var count = 0;
            var counter = new Utilities.CountMarks("***", "x");
            counter.PropertyChanged += (s, e) => { count = counter.Count; };
            var lines = new List<Models.Line> 
            { 
                new Utilities.Models.Line("[ 2015.02.08 22:13:06 ] Einherijer > ***"),
                new Utilities.Models.Line("﻿[ 2015.02.08 22:12:13 ] malcotch > xXX"),
                new Utilities.Models.Line("[ 2015.02.08 22:12:56 ] sapage1 > xx"),
                new Utilities.Models.Line("[ 2015.02.08 22:12:56 ] sapage12 > X")
            };

            foreach (var line in lines)
                counter.ProcessLine(line);

            Assert.AreEqual(3, counter.Count);
        }
    }
}
