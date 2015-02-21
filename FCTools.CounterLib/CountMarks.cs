using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FCTools.Utilities
{
    public class CountMarks : INotifyPropertyChanged
    {
        private string restartString;
        private string iterateString;
        private Regex iterateRegex;
        private Regex restartRegex;

        private int count;
        public int Count
        {
            get { return count; }
            set
            {
                count = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Count"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Class that counts lines according to provided restart and iterate strings.
        /// </summary>
        /// <param name="restartString">String that will trigger a counter reset.</param>
        /// <param name="iterateString">String that will trigger an iteration of the counter.</param>
        public CountMarks(string restartString, string iterateString)
        {
            this.restartString = restartString;
            this.iterateString = iterateString;

            this.restartRegex = new Regex(string.Format("^{0}", EscapeString(this.restartString)), RegexOptions.IgnoreCase);
            this.iterateRegex = new Regex(string.Format("^[{0}]+", EscapeString(this.iterateString)), RegexOptions.IgnoreCase);
        }

        private string EscapeString(string p)
        {
            var charsToReplace = @"[]{}\/|.?+*".ToCharArray();
            var sb = new StringBuilder();

            for (int i = 0; i < p.Length; i++)
            {
                var escapeChar = false;
                foreach (var c in charsToReplace)
                {
                    if (p[i].Equals(c))
                    {
                        escapeChar = true;
                        break;
                    }
                }
                if (escapeChar)
                    sb.Append("\\" + p[i]);
                else
                    sb.Append(p[i]);
            }
            
            return sb.ToString();
        }

        /// <summary>
        /// Iterates or restarts count according to provided restart and iterate strings.
        /// Will search the beginning of Line.Message for the string match which means 
        /// any additional message will not interfere with the operation.  Iteration will
        /// occur even if the iterate string is repeated.
        /// </summary>
        /// <param name="line">Line to process.</param>
        public void ProcessLine(Models.Line line)
        {
            var mIterate = iterateRegex.Match(line.Message);
            var mRestart = restartRegex.Match(line.Message);

            if (mRestart.Success)
                this.Count = 0;

            if (mIterate.Success)
                this.Count++;
        }
    }
}
