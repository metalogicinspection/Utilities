using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DropboxUploader.Core
{
    internal class InMemoryCache
    {
        private static InMemoryCache _instance;
        public static InMemoryCache GetInstance()
        {
            var retValue = _instance;
            if (retValue != null)
            {
                return retValue;
            }
            retValue = new InMemoryCache();
            _instance = retValue;
            return retValue;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal byte[] ReadFileBinaryFromLocalCache(string remotePath, string serverRev)
        {
            var remoteHashStr = FileUploadHelper.GenerateGuidStr(remotePath);
            Guid remoteHash;
            Guid.TryParse(remoteHashStr, out remoteHash);

            var index = _queue.FindIndex(0, container => container.RemotePathHash == remoteHash);
            if (index < 0)
            {
                return null;
            }

            var c = _queue[index];
            _queue.RemoveAt(index);
            if (c.ServerRev.Equals(serverRev))
            {
                _queue.Insert(0, c);
                FileUploadHelper.WriteLog("Read from memory " + remotePath);
                return c.DataFileBinary;
            }

            return null;
        }

        private const int QueueMaxLength = 30;

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal byte[] WriteFileBinaryToLocalCache(string remotePath, string serverRev, byte[] data)
        {
            var remoteHashStr = FileUploadHelper.GenerateGuidStr(remotePath);
            Guid remoteHash;
            Guid.TryParse(remoteHashStr, out remoteHash);
            var index = _queue.FindIndex(0, container => container.RemotePathHash == remoteHash);
            if (index >= 0)
            {
                _queue.RemoveAt(index);
            }

            _queue.Insert(0, new CachedDataFileContainer { DataFileBinary = data, RemotePath = remotePath, ServerRev = serverRev, RemotePathHash = remoteHash });

            if (_queue.Count > QueueMaxLength)
            {
                _queue.RemoveAt(QueueMaxLength - 1);
            }
            return null;
        }

        private readonly List<CachedDataFileContainer> _queue = new List<CachedDataFileContainer>();

        private class CachedDataFileContainer
        {
            public byte[] DataFileBinary { get; set; }

            public Guid RemotePathHash { get; set; }
            public string RemotePath { get; set; }

            public string ServerRev { get; set; }



        }




    }
}
