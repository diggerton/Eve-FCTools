using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCTools.Utilities.Interfaces
{
    public interface IFindEveChatlogs
    {
        DirectoryInfo ResolveChatlogDir(string s = null);
        IEnumerable<Models.ChatlogFile> GetActiveFiles(DirectoryInfo di, TimeSpan ts);
    }
}
