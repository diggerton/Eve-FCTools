using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCTools.Utilities.Models
{
    public class ChatlogFile
    {
        public DateTime LastActiveUTC { get; set; }
        public FileInfo FileInfoObject { get; set; }
        public string ChatRoomName { get; set; }
    }
}
