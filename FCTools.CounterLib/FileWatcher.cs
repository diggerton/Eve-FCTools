using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FCTools.Utilities.Models;
using System.Diagnostics;

namespace FCTools.Utilities
{
    public class FileWatcher : Interfaces.IFileWatcher
    {
        public List<Line> AllLines { get; private set; }
        public LineQueue Lines { get; private set; }

        private bool cancel;
        private DateTime timeStarted;
        private bool ignoreDuplicateSenders;

        public FileWatcher(bool ignoreDuplicateSenders = true)
        {
            AllLines = new List<Line>();
            Lines = new LineQueue();
            this.ignoreDuplicateSenders = ignoreDuplicateSenders;
        }

        public void StartWatchingFile(FileInfo file)
        {
            if (!file.Exists)
                throw new FileNotFoundException();

            if (file.Extension != ".txt")
                throw new FileNotFoundException("File extension not supported.");

            this.timeStarted = DateTime.UtcNow;
            Task.Run(() => 
            {
                beginStream(file.FullName.ToString()); 
            });
        }
        public void StopWatchingFile()
        {
            this.cancel = true;
        }

        private void beginStream(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(stream))
                {
                    while (!this.cancel)
                    {
                        while (!reader.EndOfStream && !this.cancel)
                        {
                            processLine(reader.ReadLine());
                            Debug.WriteLine("");
                        }
                        while (reader.EndOfStream && !this.cancel)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
        }
        private void processLine(string s)
        {
            var line = new Line(s);
            this.AllLines.Add(line);

            Debug.WriteLine(string.Format("Timestamp:{0}\tSender:{1}\tMessage:{2}", line.Timestamp, line.Sender, line.Message));
            Debug.WriteLine("Line Universal: " + line.Timestamp.ToUniversalTime());
            
            if (line.Timestamp >= timeStarted)
            {
                bool _canAdd = true;
                if (ignoreDuplicateSenders)
                    _canAdd = !this.Lines.Any(l => l.Sender.Equals(line.Sender, StringComparison.OrdinalIgnoreCase));

                if(_canAdd)
                {
                    this.Lines.Enqueue(line);
                    Debug.WriteLine(string.Format("Line added to queue, Message Timestamp:{0}\tUTCDateTime:{1}", line.Timestamp, DateTime.UtcNow));
                }
            }
        }
    }
}
