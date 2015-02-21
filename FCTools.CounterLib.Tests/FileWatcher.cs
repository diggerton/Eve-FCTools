using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;
using System.IO;

namespace FCTools.Utilities.Tests
{
    [TestClass]
    public class FileWatcher
    {
        [TestMethod]
        public void FindChatlogs()
        {
            Interfaces.IFindEveChatlogs logFinder = new Utilities.FindEveChatlogs();

            var directory = logFinder.ResolveChatlogDir();

            Assert.IsTrue(directory.Name.EndsWith("chatlogs", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void GetActiveChatlogs()
        {
            Interfaces.IFindEveChatlogs logFinder = new Utilities.FindEveChatlogs();

            var directory = logFinder.ResolveChatlogDir();
            var activeChatlogs = logFinder.GetActiveFiles(directory, new TimeSpan(6, 0, 0));
            var allianceChatlog = activeChatlogs.FirstOrDefault(l => l.ChatRoomName.Equals("alliance", StringComparison.OrdinalIgnoreCase));

            Assert.IsNotNull(allianceChatlog);
        }

        [TestMethod]
        public void Watching()
        {
            Interfaces.IFileWatcher watcher = new Utilities.FileWatcher();
            var tempDir = Path.GetTempPath();
            var tempFileName = Path.GetRandomFileName();
            var tempTextFileName = tempFileName.Substring(0, tempFileName.IndexOf('.'));
            var path = Path.Combine(tempDir, tempTextFileName) + ".txt.";

            using (var filestream = File.CreateText(path)) { }
            watcher.StartWatchingFile(new System.IO.FileInfo(path));
            using (var stream = new StreamWriter(path))
            {
                stream.WriteLine("[ 2015.02.08 22:13:06 ] Einherijer > ***");
                stream.WriteLine("﻿[ 2015.02.08 22:12:13 ] malcotch > x");
                stream.WriteLine("[ 2015.02.08 22:12:47 ] Judy Mikakka > x");
                stream.WriteLine("[ 2015.02.08 22:12:56 ] sapage1 > x");
                stream.Flush();
                stream.Close();
            }
            Thread.Sleep(500);  //ensures the watcher has enough time to notice changes
            watcher.StopWatchingFile();

            Assert.IsTrue(watcher.AllLines.Where(l=>l.Message.Equals("x", StringComparison.OrdinalIgnoreCase)).Count() == 3);
        }

        [TestMethod]
        public void Watching_IgnoreDuplicates()
        {
            Interfaces.IFileWatcher watcher = new Utilities.FileWatcher();
            var tempDir = Path.GetTempPath();
            var tempFileName = Path.GetRandomFileName();
            var tempTextFileName = tempFileName.Substring(0, tempFileName.IndexOf('.'));
            var path = Path.Combine(tempDir, tempTextFileName) + ".txt.";

            using (var filestream = File.CreateText(path)) { }
            watcher.StartWatchingFile(new System.IO.FileInfo(path));
            using (var stream = new StreamWriter(path))
            {
                stream.WriteLine(string.Format("[ {0} ] Einherijer > ***", 
                    DateTime.UtcNow.AddMinutes(5).ToString("yyyy.MM.dd HH:mm:ss")));
                
                stream.WriteLine(string.Format("﻿[ {0} ] malcotch > x", 
                    DateTime.UtcNow.AddMinutes(5).ToString("yyyy.MM.dd HH:mm:ss")));
                
                stream.WriteLine(string.Format("﻿[ {0} ] malcotch > x", 
                    DateTime.UtcNow.AddMinutes(5).ToString("yyyy.MM.dd HH:mm:ss")));
                
                stream.WriteLine(string.Format("﻿[ {0} ] malcotch > x", 
                    DateTime.UtcNow.AddMinutes(5).ToString("yyyy.MM.dd HH:mm:ss")));
                
                stream.WriteLine(string.Format("[ {0} ] Judy Mikakka > x", 
                    DateTime.UtcNow.AddMinutes(5).ToString("yyyy.MM.dd HH:mm:ss")));
                
                stream.WriteLine(string.Format("[ {0} ] sapage1 > x", 
                    DateTime.UtcNow.AddMinutes(5).ToString("yyyy.MM.dd HH:mm:ss")));
                
                stream.Flush();
                stream.Close();
            }
            Thread.Sleep(500);  //ensures the watcher has enough time to notice changes
            watcher.StopWatchingFile();

            Assert.IsTrue(watcher.Lines.Where(l=>l.Message.Equals("x", StringComparison.OrdinalIgnoreCase)).Count() == 3);
        }
    }
}
