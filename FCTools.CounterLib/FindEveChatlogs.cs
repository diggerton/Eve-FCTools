using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace FCTools.Utilities
{
    public class FindEveChatlogs : Interfaces.IFindEveChatlogs
    {
        public FindEveChatlogs()
        {
            this.activeFiles = new List<FileSystemInfo>();
        }
        
        private List<FileSystemInfo> activeFiles;

        public DirectoryInfo ResolveChatlogDir(string location = null)
        {
            DirectoryInfo docsDir;

            if (!string.IsNullOrWhiteSpace(location))
            {
                if (location.Replace("\\", "").EndsWith("Chatlogs"))
                    return new DirectoryInfo(location);
            }
            else
            {
                docsDir = getDefaultEveChatlogsDir();
                if (docsDir != null)
                    return docsDir;
            }
            throw new DirectoryNotFoundException();
        }

        private DirectoryInfo getDefaultEveChatlogsDir()
        {
            var documentsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (Directory.Exists(documentsDir))
            {
                var evePath = Path.Combine(documentsDir, "EVE", "logs", "Chatlogs");
                if (Directory.Exists(evePath))
                    return new DirectoryInfo(evePath);
            }
            return null;
        }

        public IEnumerable<Models.ChatlogFile> GetActiveFiles(DirectoryInfo dirInfo, TimeSpan activeSince)
        {
            var files = dirInfo.EnumerateFiles();
            var fileInfos = files.Where(f => f.LastWriteTimeUtc > DateTime.UtcNow - activeSince);
            var chatlogFileList = new List<Models.ChatlogFile>();
            foreach (var fi in fileInfos)
            {
                chatlogFileList.Add(new Models.ChatlogFile 
                { 
                    LastActiveUTC = fi.LastAccessTimeUtc, 
                    ChatRoomName = getChatlogName(fi), 
                    FileInfoObject = fi 
                });
            }

            return chatlogFileList;
        }

        private string getChatlogName(FileInfo fi)
        {
            var fileSplitArr = fi.Name.Split('_');
            return fileSplitArr[0];
        }
    }
}
