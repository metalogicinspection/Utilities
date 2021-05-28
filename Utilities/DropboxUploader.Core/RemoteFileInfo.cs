using System;

namespace DropboxUploader.Core
{
    public class RemoteFileInfo
    {
        public string Path { get; set; }
        public string Name { get; set; }

        public string Rev { get; set; }

        public DateTime Modified { get; set; }
    }
    
}
