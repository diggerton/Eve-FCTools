using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace FCTools.Utilities.Tests
{
    [TestClass]
    public class FindEveChatlogs
    {
        [TestMethod]
        public void ResolveCCPChatlogDir()
        {
            Interfaces.IFindEveChatlogs fw = new Utilities.FindEveChatlogs();

            var directory = fw.ResolveChatlogDir();

            Assert.IsTrue(directory.FullName.EndsWith("chatlogs", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void ResolveActiveChatlogs()
        {
            Interfaces.IFindEveChatlogs fw = new Utilities.FindEveChatlogs();

            var directory = fw.ResolveChatlogDir();
            // Only works if there are actual active files in the dir within the given timespan.
            var activeFiles = fw.GetActiveFiles(directory, new TimeSpan(6, 0, 0));

            Assert.IsTrue(activeFiles.Count() > 0);
        }
    }
}
