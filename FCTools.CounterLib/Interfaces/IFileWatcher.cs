using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCTools.Utilities.Interfaces
{
    public interface IFileWatcher
    {
        List<Models.Line> AllLines { get; }
        Models.LineQueue Lines { get; }

        void StartWatchingFile(FileInfo fi);
        void StopWatchingFile();
    }
}
