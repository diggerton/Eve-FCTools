using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FCTools.Utilities.Models
{
    public class Line
    {
        private Line() { }
        public Line(string s)
        {
            processLine(s);
        }
        public DateTime Timestamp { get; set; }
        public string Sender { get; set; }
        public string Message { get; set; }

        private void processLine(string s)
        {
            var match = Regex.Match(s, 
                @"\[ (?<timestamp>\d{4}.\d{2}.\d{2} \d{2}:\d{2}:\d{2}) \] (?<sender>[\w\s]+)> (?<message>.+)$", 
                RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

            if (match.Success)
            {
                DateTime _dt;
                string _sender;
                string _message;

                if (!DateTime.TryParseExact(match.Groups["timestamp"].Value, "yyyy.MM.dd HH:mm:ss", 
                    CultureInfo.InvariantCulture, 
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out _dt))
                    _dt = DateTime.MinValue;

                _sender = match.Groups["sender"].Value;
                _message = match.Groups["message"].Value;

                this.Timestamp = _dt;
                this.Sender = _sender.Trim();
                this.Message = _message.Trim();
            }
            else
            {
                this.Timestamp = DateTime.MinValue;
                this.Sender = string.Empty;
                this.Message = string.Empty;
            }
        }
    }
}
