using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FCTools.Utilities.Tests
{
    [TestClass]
    public class LineQueue
    {
        [TestMethod]
        public void TriggerEvent()
        {
            var count = 0;
            var m = @"﻿[ 2015.02.08 22:12:13 ] malcotch > play sigue sigue sputnik";
            var lq = new Utilities.Models.LineQueue();

            var line = new Utilities.Models.Line(m);
            lq.OnLineEnqueued += (s, e) => { count++; };
            lq.Enqueue(line);

            Assert.AreEqual(1, lq.Count());
        }

        [TestMethod]
        public void IterateQueue()
        {
            var count = 0;
            var line1 = new Utilities.Models.Line(@"﻿[ 2015.02.08 22:12:13 ] malcotch > play sigue sigue sputnik");
            var line2 = new Utilities.Models.Line(@"[ 2015.02.08 22:12:47 ] Judy Mikakka > https://zkillboard.com/kill/44480836/");
            var line3 = new Utilities.Models.Line(@"﻿[ 2015.02.08 22:12:56 ] sapage1 > apple pie");
            ﻿var line4 = new Utilities.Models.Line(@"[ 2015.02.08 22:13:06 ] Einherijer > do we have any other timers tonight?");

             var lq = new Utilities.Models.LineQueue();
            lq.Enqueue(line1);
            lq.Enqueue(line2);
            lq.Enqueue(line3);
            lq.Enqueue(line4);

            foreach (var line in lq)
            {
                count++;
            }

            Assert.AreEqual(4, count);
        }
        
        [TestMethod]
        public void Dequeue()
        {
            var line1 = new Utilities.Models.Line(@"﻿[ 2015.02.08 22:12:13 ] malcotch > play sigue sigue sputnik");
            var line2 = new Utilities.Models.Line(@"[ 2015.02.08 22:12:47 ] Judy Mikakka > https://zkillboard.com/kill/44480836/");
            var line3 = new Utilities.Models.Line(@"﻿[ 2015.02.08 22:12:56 ] sapage1 > apple pie");
            ﻿var line4 = new Utilities.Models.Line(@"[ 2015.02.08 22:13:06 ] Einherijer > do we have any other timers tonight?");

             var lq = new Utilities.Models.LineQueue();
            lq.Enqueue(line1);
            lq.Enqueue(line2);
            lq.Enqueue(line3);
            lq.Enqueue(line4);

            var dequeued = lq.Dequeue();

            Assert.IsNotNull(dequeued);
            Assert.AreEqual(3, lq.Count());
        }
    }
}
