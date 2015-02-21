using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCTools.Utilities;
using System.IO;
using System.Drawing;

namespace FCTools.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(100, 30);
    
            Console.WriteLine("::Counter:");
            Console.WriteLine("Counts x's in an Eve chat log file.");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine();

            var finder = new FindEveChatlogs();
            
        ResolveChatlogsDirectory:
            var logFilesDirectory = ResolveChatlogsDirectory(finder);
        
        loadFiles:
            var filesDictionary = GetActiveFiles(finder, logFilesDirectory);

        menu:
            Console.Clear();
            Console.WriteLine("Available chat logs:");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine("{0, 5}  {1, -20} {2, -10}"
                    , "Num"
                    , "Chat Name"
                    , "Last Accessed");
            Console.WriteLine(new string('-', 50));
            foreach (var file in filesDictionary)
            {
                Console.WriteLine("{0, 5}  {1, -20} {2, -10}"
                    , file.Key
                    , file.Value.ChatRoomName
                    , file.Value.LastActiveUTC.ToShortTimeString());
            }
            Console.WriteLine();
            Console.WriteLine("r. Refresh List");
            Console.WriteLine("q. Quit");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine("Select one to watch.");
            
            var userInput = Console.ReadLine();
            var inputInt = -1;
            Utilities.Models.ChatlogFile selectedChatlogFile = null;
            if (userInput.Equals("r", StringComparison.OrdinalIgnoreCase))
                goto loadFiles;
            else if (userInput.Equals("q", StringComparison.OrdinalIgnoreCase))
                Environment.Exit(0);
            else if (!int.TryParse(userInput, out inputInt) || !filesDictionary.TryGetValue(inputInt, out selectedChatlogFile))
                goto menu;

            var counters = SetupCounters();

            Console.Clear();
            Console.WriteLine("Watching {0}...", selectedChatlogFile.ChatRoomName);
            Console.WriteLine();

            for (int i = 0; i < counters.Count; i++)
            {
                counters[i].CountMarksObj.PropertyChanged += (s, e) =>
                {
                    UpdateCounters(counters);
                };
            }

            var watcher = new FileWatcher();
            watcher.Lines.OnLineEnqueued += (s, e) => 
            {
                while (watcher.Lines.HasItems())
                {
                    var line = watcher.Lines.Dequeue();
                    counters.ForEach(c => c.CountMarksObj.ProcessLine(line));
                }
            };

            watcher.StartWatchingFile(selectedChatlogFile.FileInfoObject);
            Console.WriteLine("Press any key to quit watching and return to main menu.");
            Console.ReadKey();
            watcher.StopWatchingFile();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(new string('*', 50));
            counters.ForEach(c => Console.WriteLine("\tTotal for <{0}>: {1}", c.Name, c.CountMarksObj.Count));
            Console.WriteLine(new string('*', 50));
            Console.WriteLine("Press any key to return to main menu.");
            Console.ReadKey();
            goto menu;
        }

        private static void UpdateCounters(List<CounterTracker> counters)
        {
            var counts = new StringBuilder();
            counters.ForEach(c => counts.AppendFormat("{0}:{1}  ", c.Name, c.CountMarksObj.Count));
            var fullString = "Totals\t" + counts.ToString();
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(fullString);
        }

        private static List<CounterTracker> SetupCounters()
        {
            var listOfCounterTracker = new List<CounterTracker>();
            Console.WriteLine();
            Console.Write("Sequence that restarts the count(s) (default is '***'): ");
            var resetCountString = Console.ReadLine();
            resetCountString = string.IsNullOrWhiteSpace(resetCountString) ? "***" : resetCountString;

        howManyCounters:
            Console.WriteLine();
            Console.WriteLine("Multiple counters allows you to count with different iterators.");
            Console.WriteLine("This means you could count both titans and supers at the same time.");
            Console.WriteLine("No maximum on the number of counters, however I recommend no more than 3 or 4.");
            Console.Write("How many counters would you like to setup? ");
            var numCountersInput = Console.ReadKey();
            
            int numCounters = 0;
            if (!int.TryParse(numCountersInput.KeyChar.ToString(), out numCounters) && numCounters <= 0)
                goto howManyCounters;

            Console.WriteLine();
            for (int i = 0; i < numCounters; i++)
            {
                Console.Write("Enter the name of counter #{0}: ", i);
                var counterName = Console.ReadLine();
                Console.Write("Sequence to iterate count for counter <{0}> (default is 'x'): ", counterName);
                var iterateCountString = Console.ReadLine();
                iterateCountString = string.IsNullOrWhiteSpace(iterateCountString) ? "x" : iterateCountString;
                
                listOfCounterTracker.Add(
                    new CounterTracker
                    {
                        Name = counterName,
                        CountMarksObj = new CountMarks(resetCountString, iterateCountString)
                    });
            }

            return listOfCounterTracker;
        }

        private static Dictionary<int, Utilities.Models.ChatlogFile> GetActiveFiles(FindEveChatlogs finder, DirectoryInfo logFilesDirectory)
        {
            var files = finder.GetActiveFiles(logFilesDirectory, new TimeSpan(6, 0, 0));
            Dictionary<int, Utilities.Models.ChatlogFile> filesDictionary = new Dictionary<int, Utilities.Models.ChatlogFile>();
            if (files.Count() == 0)
            {
                Console.WriteLine("Either the directory supplied was incorrect or");
                Console.WriteLine("there were no active chat logs found.");
                Console.WriteLine("Active, meaning, appended to within the last 6 hours.");
            }
            filesDictionary = new Dictionary<int, Utilities.Models.ChatlogFile>();
            var num = 1;
            foreach (var file in files.OrderByDescending(f => f.LastActiveUTC))
            {
                filesDictionary.Add(num, file);
                num++;
            }
            return filesDictionary;
        }
        private static DirectoryInfo ResolveChatlogsDirectory(FindEveChatlogs finder)
        {
            DirectoryInfo logFilesDirectory = null;
            Console.WriteLine("Enter the Eve chat log directory and hit enter.");
            Console.WriteLine("To use the default file location, hit enter.");
            var input = Console.ReadLine();
            try
            {
                logFilesDirectory = finder.ResolveChatlogDir(input);
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine();
                Console.WriteLine("Attempt to resolve default chat log directory has failed.");
            enterPath:
                Console.WriteLine(@"Please enter the path to your Eve Online chat logs directory.");
                Console.WriteLine(@"The EVE directory is usually found in your user directory and");
                Console.WriteLine(@"typically ends with '\EVE\logs\Chatlogs'.");
                Console.WriteLine("If you wish to exit, enter 'q'.");
                var directoryInput = Console.ReadLine();

                if (directoryInput.Equals("q", StringComparison.OrdinalIgnoreCase))
                    Environment.Exit(0);
                if (!Directory.Exists(directoryInput))
                {
                    Console.WriteLine("Entered path was invalid.");
                    goto enterPath;
                }

                logFilesDirectory = new DirectoryInfo(directoryInput);
            }
            return logFilesDirectory;
        }
    }

    class CounterTracker
    {
        public string Name { get; set; }
        public int CursorRow { get; set; }
        public CountMarks CountMarksObj { get; set; }
    }
}
