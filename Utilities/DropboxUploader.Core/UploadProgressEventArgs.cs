using System;

namespace DropboxUploader.Core
{
    public class UploadProgressEventArgs
    {
        public string LocalPath { get; internal set; }
        public string RemotePath { get; internal set; }

        public long FileSizeBytes { get; internal set; }
        public long ChunkSizeBytes { get; internal set; }
        public int ChunksCount { get; internal set; }
        public int CurrentChunkIndex { get; internal set; }

        public DateTime UploadStarted { get; internal set; }

        public TimeSpan TotalSpentTime { get; internal set; }
        public TimeSpan CurrentChunkSpentTime { get; internal set; }
        public TimeSpan EastimatedRemainingTime { get; internal set; }

        public DropboxDirectClient.RunResults Result { get; internal set; }
    }
}
