using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Specialized;
using Dropbox.Api;
using Dropbox.Api.Files;

namespace DropboxUploader.Core
{
    public class DropboxDirectClient 
    {
        // Add an ApiKey (from https://www.dropbox.com/developers/apps) here
        private readonly string _accessToken;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly DirectoryInfo _directoryBase;
        
        
        
        private readonly DirectoryInfo _directoryInfo;

        private readonly DirectoryInfo _directoryCheckSum;

        private readonly DirectoryInfo _directoryCache;

        public enum QueryRunResults
        {
            ConnectionError,
            Exist,
            NotExist
        }

        public enum DownloadSavingCacheModes
        {
            NoLocalCache,
            ReadLocalCacheFirst,
            ForcedFromServerButSaveLocalCache
        }

        public enum RunResults
        {
            NoInternetConnection,
            LoginFailed,
            Finished,
            ReadLocalFileFailed,
        }
        

        public DropboxDirectClient(string localUploadDirBase, string token)
        {
            _directoryBase = new DirectoryInfo(localUploadDirBase);
            FileUploadHelper.CheckCreateDir(_directoryBase);
            
            _directoryInfo = new DirectoryInfo(Path.Combine(localUploadDirBase, "Information"));
            FileUploadHelper.CheckCreateDir(_directoryInfo);

            _directoryCache = new DirectoryInfo(Path.Combine(localUploadDirBase, "BigCache"));
            FileUploadHelper.CheckCreateDir(_directoryCache);

            _directoryCheckSum = new DirectoryInfo(Path.Combine(localUploadDirBase, "CheckSum"));
            FileUploadHelper.CheckCreateDir(_directoryCheckSum);
            

            _accessToken = token;
        }


        public QueryRunResults TryReadLocalCache(string remotePath, string serverRev, out byte[] retValue)
        {
            retValue = ReadFileBinaryFromLocalCache(remotePath, serverRev);
            return retValue == null ? QueryRunResults.NotExist : QueryRunResults.Exist;
        }


        public QueryRunResults TryDownFTPFileBinary(string remotePath, string user, string pass, out byte[] retValue, out string serverRev, bool forceupdate, bool saveLocalCache)
        {
            var request = (FtpWebRequest)WebRequest.Create(remotePath);

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(user, pass);

            request.Method = WebRequestMethods.Ftp.GetDateTimestamp;

            try
            {
                using (var resp = (FtpWebResponse)request.GetResponse())
                {
                    serverRev = resp.LastModified.Ticks.ToString();
                }
            }
            catch (Exception)
            {
                retValue = null;
                serverRev = string.Empty;
                return QueryRunResults.ConnectionError;
            }

            retValue = !forceupdate && saveLocalCache ? ReadFileBinaryFromLocalCache(remotePath, serverRev) : null;
            if (retValue != null)
            {
                return QueryRunResults.Exist;
            }


            request = (FtpWebRequest)WebRequest.Create(remotePath);
            request.Credentials = new NetworkCredential(user, pass);

            request.Method = WebRequestMethods.Ftp.DownloadFile;

            try
            {
                using (var ftpStream = request.GetResponse().GetResponseStream())
                {
                    using (var ms = new MemoryStream())
                    {
                        ftpStream.CopyTo(ms);
                        retValue = ms.ToArray();
                    }
                }

            }
            catch (Exception)
            {
                retValue = null;
                return QueryRunResults.ConnectionError;
            }


            SaveDataFileLocalCache(remotePath, serverRev, retValue);

            return QueryRunResults.Exist;
        }


        public QueryRunResults TryDownFileBinary(string remotePath, out byte[] retValue, string serverRev = null, DropboxDirectClient.DownloadSavingCacheModes saveLocalCacheMode = DropboxDirectClient.DownloadSavingCacheModes.NoLocalCache)
        {
            FileUploadHelper.WriteLog("download mode" + saveLocalCacheMode);
            retValue = saveLocalCacheMode == DownloadSavingCacheModes.ReadLocalCacheFirst ? ReadFileBinaryFromLocalCache(remotePath, serverRev) : null;
            if (retValue != null)
            {
                return QueryRunResults.Exist;
            }
            WaitForNextConnection();
            using (var httpClient = new HttpClient(new WebRequestHandler {ReadWriteTimeout = 10*1000})
            {
                // Specify request level timeout which decides maximum time taht can be spent on
                // download/upload files.
                Timeout = TimeSpan.FromMinutes(20)
            })
            {
                var config = new DropboxClientConfig("RT Report")
                {
                    HttpClient = httpClient
                };
                var client = new DropboxClient(_accessToken, config);

                var hashCode = FileUploadHelper.GenerateGuidStr(string.Concat(remotePath, _accessToken).ToUpper());

                var localCachedFilePath = Path.Combine(_directoryCache.FullName, hashCode);

                if (saveLocalCacheMode == DownloadSavingCacheModes.ReadLocalCacheFirst && File.Exists(localCachedFilePath) && string.IsNullOrWhiteSpace(serverRev)) //if this file may have local cached file, but just server version unknow
                {
                    //query the serverversion first
                    var qryServerVersionTsk = client.Files.GetMetadataAsync(remotePath);
                    try
                    {
                        serverRev = qryServerVersionTsk.Result.AsFile.Rev;
                        //now got the server version, try read from cached floder
                        retValue = ReadFileBinaryFromLocalCache(remotePath, serverRev);
                        if (retValue != null)
                        {
                            return QueryRunResults.Exist;
                        }
                    }
                    catch (Exception)
                    {
                        retValue = null;
                    }
                }
                var f = client.Files.DownloadAsync(remotePath, serverRev);
                try
                {
                    retValue = f.Result.GetContentAsByteArrayAsync().Result;
                    serverRev = f.Result.Response.Rev;
                }
                catch (Exception exception)
                {
                    retValue = null;
                }

                var result = f.Status == TaskStatus.RanToCompletion
                    ? QueryRunResults.Exist
                    : (f.Exception?.InnerException?.Message ?? string.Empty).Contains("not_found")
                        ? QueryRunResults.NotExist
                        : QueryRunResults.ConnectionError;
                
                if (result == QueryRunResults.Exist && saveLocalCacheMode != DownloadSavingCacheModes.NoLocalCache && retValue != null)
                {
                    SaveDataFileLocalCache(remotePath, serverRev, retValue);
                }
                
                
                FileUploadHelper.WriteLog("downloaded from server" + remotePath);

                return result;
            }

        }

        private static string _iv = "kHyVQwedvl4waiT6";

        private void SaveDataFileLocalCache(string remotePath, string serverRev, byte[] data)
        {
            new Thread((() =>
            {
                InMemoryCache.GetInstance().WriteFileBinaryToLocalCache(remotePath, serverRev, data);
                var hashCode = FileUploadHelper.GenerateGuidStr(string.Concat(remotePath, _accessToken).ToUpper());
                var localCachedFilePath = Path.Combine(_directoryCache.FullName, hashCode);


                try
                {
                    var encryped = AESEncryption.Encrypt(data, GetKey(serverRev), GetIV());
                    File.WriteAllBytes(localCachedFilePath, encryped);

                    File.WriteAllText(Path.Combine(_directoryInfo.FullName, hashCode),
                        GenerateLocalCachedFileVersionHash(new FileInfo(localCachedFilePath)));
                    FileUploadHelper.WriteLog("write to local cache from server" + remotePath);

                    File.WriteAllText(Path.Combine(_directoryCheckSum.FullName, hashCode),
                       FileUploadHelper.GenerateGuidStr(data));
                }
                catch (Exception)
                {
                    // ignored
                }
            })).Start();

        }

        private byte[] GetKey(string serverRev)
        {
            return Encoding.ASCII.GetBytes(string.Concat(serverRev, "f4@!a(-bd").Substring(0,16));

        }

        private byte[] GetIV()
        {
            return Encoding.ASCII.GetBytes(_iv);
        }

        private string GenerateLocalCachedFileVersionHash(FileInfo localCachedFileInfo)
        {
            var key = string.Concat(localCachedFileInfo.LastWriteTimeUtc.Ticks, localCachedFileInfo.Length, "fi0dfoFb89gji34");
            var versionHash = FileUploadHelper.GenerateGuidStr(key);

            return versionHash;
        }

        public QueryRunResults FileExist(string remotePath)
        {
            using (var httpClient = new HttpClient(new WebRequestHandler { ReadWriteTimeout = 10 * 1000 })
            {
                // Specify request level timeout which decides maximum time taht can be spent on
                // download/upload files.
                Timeout = TimeSpan.FromMinutes(20)
            })
            {
                try
                {
                    var config = new DropboxClientConfig("RT Report")
                    {
                        HttpClient = httpClient
                    };
                    var client = new DropboxClient(_accessToken, config);
                    var f = client.Files.GetMetadataAsync(remotePath);
                    var status = WaitTask(f).Status;
                    if (status == TaskStatus.RanToCompletion)
                    {
                        return QueryRunResults.Exist;
                    }
                    if(status == TaskStatus.Faulted)
                    {
                        return QueryRunResults.NotExist;
                    }
                    return QueryRunResults.ConnectionError;
                }
                catch (Exception)
                {
                    return QueryRunResults.ConnectionError;
                }
                
            }

        }
        
        private byte[] ReadFileBinaryFromLocalCache(string remotePath, string serverRev = null)
        {
            if (string.IsNullOrWhiteSpace(serverRev))
            {
                return null;
            }

            var tmp = InMemoryCache.GetInstance().ReadFileBinaryFromLocalCache(remotePath, serverRev);
            if (tmp != null)
            {
                return tmp;
            }

            var hashCode = FileUploadHelper.GenerateGuidStr(string.Concat(remotePath, _accessToken).ToUpper());
            var inFoFilePath = Path.Combine(_directoryInfo.FullName, hashCode);
            
            var localCachedFileInfo = new FileInfo(Path.Combine(_directoryCache.FullName, hashCode));
            if (!localCachedFileInfo.Exists)
            {
                return null;
            }
            var versionHash = GenerateLocalCachedFileVersionHash(localCachedFileInfo);

            var cachedRev = File.Exists(inFoFilePath) ? File.ReadAllText(inFoFilePath) : string.Empty;
            if (!cachedRev.Equals(versionHash))
            {
                return null;
            }

            var dataChecSumFileInfo = new FileInfo(Path.Combine(_directoryCheckSum.FullName, hashCode));
            if (!dataChecSumFileInfo.Exists)
            {
                return null;
            }
            var dataCheckSum = File.ReadAllText(dataChecSumFileInfo.FullName);


            byte[] binary;
            try
            {
                binary = ReadAllBytes2(localCachedFileInfo.FullName);
            }
            catch (Exception)
            {
                binary = null;
            }


            byte[] retValue = null;
            try
            {
                var start = DateTime.Now;
                retValue = AESEncryption.Decrypt(binary, GetKey(serverRev), GetIV());

                FileUploadHelper.WriteLog("Decrypt time spent "+ (DateTime.Now- start).TotalMilliseconds +"  " + remotePath);
            }
            catch (Exception)
            {
                return null;
            }

            if (!FileUploadHelper.GenerateGuidStr(retValue).Equals(dataCheckSum))
            {
                return null;
            }

            if (retValue != null)
            {
                InMemoryCache.GetInstance().WriteFileBinaryToLocalCache(remotePath, serverRev, retValue);
            }

            return retValue;
        }

        public byte[] DownloadBinary(string remotePath, string serverRev = null, DownloadSavingCacheModes saveLocalCacheMode = DownloadSavingCacheModes.NoLocalCache)
        {
            byte[] retvalue;
            return TryDownFileBinary(remotePath, out retvalue, serverRev, saveLocalCacheMode) != QueryRunResults.Exist? null : retvalue;
        }

        private static readonly Queue<DateTime> DownloadFileDataSetTimes = new Queue<DateTime>();
        /// <summary>
        /// dropbox has a cap on the max connections over a time period, if over the max, new connections will be block
        /// this only happens if the computer is in a VERY fast network
        /// Uses this timmer to slow down of creating new connections.
        /// 
        /// Maximum 120 connection to dropbox in every 10 seconds
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WaitForNextConnection()
        {
            if (DownloadFileDataSetTimes.Count < 120)
            {
                DownloadFileDataSetTimes.Enqueue(DateTime.Now);
                return;

            }
            while (DownloadFileDataSetTimes.Count > 120)
            {
                DownloadFileDataSetTimes.Dequeue();
            }
            var pre130Time = DownloadFileDataSetTimes.Peek();
            var span = (DateTime.Now - pre130Time).TotalMilliseconds;
            if (span < 10000)
            {
                Thread.Sleep(new TimeSpan(0, 0, 0, 0, (int)(10000d - span)));
            }
            DownloadFileDataSetTimes.Enqueue(DateTime.Now);
        }


        
        public List<RemoteFileInfo> ListFileVersions(string remotePath)
        {
            List<RemoteFileInfo> files = null;
            if (TryListFileVersions(remotePath, out files) == RunResults.Finished)
            {
                return files;
            }
            return null;
        }

        public RunResults TryListFileVersions(string remotePath, out List<RemoteFileInfo> files)
        {
            files = null;
            using (var httpClient = new HttpClient(new WebRequestHandler { ReadWriteTimeout = 10 * 1000 })
            {
                // Specify request level timeout which decides maximum time taht can be spent on
                // download/upload files.
                Timeout = TimeSpan.FromMinutes(20)
            })
            {
                var config = new DropboxClientConfig("RT Report")
                {
                    HttpClient = httpClient
                };
                var client = new DropboxClient(_accessToken, config);

                var f = client.Files.ListRevisionsAsync(remotePath);
                if (!WaitTask(f).IsCompleted || f.Status == TaskStatus.Faulted)
                {
                    return RunResults.NoInternetConnection;
                }

                files = f.Result.Entries
                    .Where(x => x.IsFile)
                    .Select(x => new RemoteFileInfo() { Name = x.Name, Path = x.PathLower, Modified = x.ServerModified, Rev = x.Rev })
                    .ToList();

                return RunResults.Finished;
            }

        }

        public RunResults TryListChildrenFiles(string remotePath, out List<RemoteFileInfo> files)
        {
            files = null;
            using (var httpClient = new HttpClient(new WebRequestHandler { ReadWriteTimeout = 10 * 1000 })
            {
                // Specify request level timeout which decides maximum time taht can be spent on
                // download/upload files.
                Timeout = TimeSpan.FromMinutes(20)
            })
            {
                var config = new DropboxClientConfig("RT Report")
                {
                    HttpClient = httpClient
                };
                var client = new DropboxClient(_accessToken, config);

                var f = client.Files.ListFolderAsync(remotePath);
                if (!WaitTask(f).IsCompleted || f.Status == TaskStatus.Faulted)
                {
                    return RunResults.NoInternetConnection;
                }

                files = f.Result.Entries.OfType<FileMetadata>()
                    .Where(x => x.IsFile)
                    .Select(x => new RemoteFileInfo() { Name = x.Name, Path = x.PathLower, Modified = x.ServerModified, Rev = x.Rev })
                    .ToList();

                while (f.Result.HasMore)
                {
                    f = client.Files.ListFolderContinueAsync(f.Result.Cursor);
                    if (!WaitTask(f).IsCompleted || f.Status == TaskStatus.Faulted)
                    {
                        break;
                    }

                    files.AddRange(f.Result.Entries.OfType<FileMetadata>()
                        .Where(x => x.IsFile)
                        .Select(x => new RemoteFileInfo() { Name = x.Name, Path = x.PathLower, Modified = x.ServerModified, Rev = x.Rev })
                        .ToList());
                }


                return RunResults.Finished;
            }

        }

        public RunResults DeleteFile(string remotePath)
        {
            var retValue = RunResults.NoInternetConnection;
            using (var httpClient = new HttpClient(new WebRequestHandler {ReadWriteTimeout = 10 * 1000})
            {
                // Specify request level timeout which decides maximum time taht can be spent on
                // download/upload files.
                Timeout = TimeSpan.FromMinutes(20)
            })
            {
                var config = new DropboxClientConfig("RT Report")
                {
                    HttpClient = httpClient
                };
                var client = new DropboxClient(_accessToken, config);
                var f = client.Files.DeleteAsync(remotePath);
                if (!WaitTask(f).IsCompleted || f.Status == TaskStatus.Faulted)
                {
                    retValue =  RunResults.NoInternetConnection;
                }
                retValue = RunResults.Finished;
            }

            return retValue;
        }

        public List<RemoteFileInfo> ListChildrenFiles(string remotePath)
        {
            List<RemoteFileInfo> files = null;
            if (TryListChildrenFiles(remotePath, out files) == RunResults.Finished)
            {
                return files;
            }
            return null;
        }

        public event EventHandler<UploadProgressEventArgs> UploadProgressed;

        public RunResults UploadOnline(string localFilePath, string remoteFilePath)
        {
            var localFile = new FileInfo(localFilePath);
            if (!localFile.Exists)
            {
                throw new Exception("Local File Not Found.");
            }

            using (HttpClient httpClient = new HttpClient(new WebRequestHandler { ReadWriteTimeout = 10 * 1000 })
            {
                // Specify request level timeout which decides maximum time taht can be spent on
                // download/upload files.
                Timeout = TimeSpan.FromMinutes(20)
            })
            {
                var config = new DropboxClientConfig("RT Report")
                {
                    HttpClient = httpClient
                };
                var client = new DropboxClient(_accessToken, config);

                Console.WriteLine("Chunk upload file...");
                // Chunk size is 128KB.
                const int chunkSize = 10 * 1024 * 1024;
                var pauseEvent = new ManualResetEvent(true);
                if (localFile.Length < chunkSize)
                {
                    var time = DateTime.Now;
                    var progressedEventArgs = new UploadProgressEventArgs()
                    {
                        LocalPath = localFilePath,
                        RemotePath = remoteFilePath,
                        ChunkSizeBytes = chunkSize,
                        UploadStarted = time,

                    };

                    var bytesToUpload = ReadAllBytes2(localFilePath);
                    //var blobClient = GetBlobClient(remoteFilePath);
                    //var result = RunResults.NoInternetConnection;
                    //try
                    //{
                    //    blobClient.Upload(new MemoryStream(bytesToUpload));
                    //    result = RunResults.Finished;
                    //}
                    //catch (Exception e)
                    //{
                    //    result = RunResults.NoInternetConnection;
                    //}

                    //if (result == RunResults.Finished)
                    //{
                    //    result = UploadOnline(bytesToUpload, remoteFilePath);
                    //}

                    var result = UploadOnline(bytesToUpload, remoteFilePath);



                    var current = DateTime.Now;
                    progressedEventArgs.CurrentChunkIndex = 0;
                    progressedEventArgs.CurrentChunkSpentTime = current - time;
                    progressedEventArgs.TotalSpentTime = current - time;
                    progressedEventArgs.ChunksCount = 1;
                    progressedEventArgs.EastimatedRemainingTime = new TimeSpan(0);
                    progressedEventArgs.Result = result;
                    //notify last chunk which is the only trunk in this small file!
                    UploadProgressed?.Invoke(this, progressedEventArgs);
                }


                try
                {
                    var time = DateTime.Now;
                    var currentChunkReult = TaskStatus.Faulted;
                    var progressedEventArgs = new UploadProgressEventArgs()
                    {
                        LocalPath = localFilePath,
                        RemotePath = remoteFilePath,
                        ChunkSizeBytes = chunkSize,
                        UploadStarted = time,

                    };
                    var length = new FileInfo(localFilePath).Length;
                    var numChunks = (int)Math.Ceiling(1d * length / chunkSize);

                    var sessionId = string.Empty;
                    progressedEventArgs.FileSizeBytes = length;
                    progressedEventArgs.ChunksCount = numChunks;

                    var blobClient = GetBlobClient(remoteFilePath);
                    var uploadedBlobBlocks = new List<string>();

                    for (byte idx = 0; idx < numChunks; idx++)
                    {
                        Console.WriteLine("Start uploading chunk {0}/{1} ", idx, numChunks);
                        var buffer = new byte[chunkSize];
                        var byteRead = 0;
                        using (var stream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read,
                            FileShare.ReadWrite))
                        {
                            byteRead = stream.Read(buffer, 0, chunkSize);
                        }


                        var chunkStartTime = DateTime.Now;

                        try
                        {
                            using (var memStream = new MemoryStream(buffer, 0, byteRead))
                            {
                                var blockIdBytes = new byte[] { idx };
                                var blockIdBase64 = Convert.ToBase64String(blockIdBytes); // "MA=="

                                //var blockIds2 = BitConverter.GetBytes(idx);

                                //var blockIdBase64 = Convert.ToBase64String(BitConverter.GetBytes(idx));

                                var stageResponse = blobClient.StageBlock(blockIdBase64, memStream);
                                var responseInfo = stageResponse.GetRawResponse(); // 201: Created
                                uploadedBlobBlocks.Add(blockIdBase64);
                            }

                            if (idx == numChunks - 1)
                            {
                                blobClient.CommitBlockList(uploadedBlobBlocks);
                            }

                            progressedEventArgs.Result = RunResults.Finished;
                        }
                        catch (Exception e)
                        {
                            progressedEventArgs.Result = RunResults.NoInternetConnection;
                        }

                        if (progressedEventArgs.Result == RunResults.Finished)
                        {
                            using (var memStream = new MemoryStream(buffer, 0, byteRead))
                            {
                                if (idx == 0)
                                {
                                    client.Files.BeginUploadSessionStart(memStream, ar =>
                                    {
                                        var result =
                                            ar as Task<UploadSessionStartResult>;
                                        currentChunkReult = result?.Status ?? TaskStatus.Faulted;
                                        sessionId = currentChunkReult == TaskStatus.Faulted ? string.Empty : result?.Result?.SessionId ?? string.Empty;
                                        pauseEvent.Set();
                                    });
                                }
                                else if (idx == numChunks - 1)
                                {
                                    Console.WriteLine("Last chunk uploading");
                                    var cursor = new UploadSessionCursor(sessionId, (ulong)(chunkSize * idx));
                                    client.Files.BeginUploadSessionFinish(cursor, new CommitInfo(remoteFilePath), memStream,
                                        ar =>
                                        {
                                            var result = ar as Task<Dropbox.Api.Files.FileMetadata>;

                                            currentChunkReult = result?.Status ?? TaskStatus.Faulted;
                                            pauseEvent.Set();
                                        });
                                }
                                else
                                {
                                    var cursor = new UploadSessionCursor(sessionId, (ulong)(chunkSize * idx));
                                    client.Files.BeginUploadSessionAppend(cursor, memStream,
                                        ar =>
                                        {
                                            var result = ar as Task<bool>;

                                            currentChunkReult = result?.Status ?? TaskStatus.Faulted;
                                            pauseEvent.Set();
                                        });
                                }

                                pauseEvent.Reset();
                                pauseEvent.WaitOne(new TimeSpan(0, 30, 30));

                                var current = DateTime.Now;
                                Console.WriteLine("remaining {0}", (current - time).TotalMinutes / (idx + 1) * (numChunks - idx - 1));
                                progressedEventArgs.CurrentChunkIndex = idx;
                                progressedEventArgs.CurrentChunkSpentTime = current - chunkStartTime;
                                progressedEventArgs.TotalSpentTime = current - time;
                                progressedEventArgs.EastimatedRemainingTime =
                                    new TimeSpan(progressedEventArgs.CurrentChunkSpentTime.Ticks * (numChunks - idx - 1));
                                progressedEventArgs.Result = currentChunkReult == TaskStatus.RanToCompletion
                                    ? RunResults.Finished
                                    : RunResults.NoInternetConnection;

                                //DO NOT notify the last chunk
                                //Notify last chunk at below when the file stream is closed.
                                if (idx < numChunks - 1)
                                {
                                    UploadProgressed?.Invoke(this, progressedEventArgs);
                                }

                                if (currentChunkReult != TaskStatus.RanToCompletion)
                                {
                                    Console.WriteLine("Upload Failed");
                                    return RunResults.NoInternetConnection;
                                }
                            }
                        }




                    }
                    //notify last chunk
                    UploadProgressed?.Invoke(this, progressedEventArgs);
                }
                catch (System.IO.IOException)
                {
                    return RunResults.ReadLocalFileFailed;
                }


            }

            Console.WriteLine("Upload Succeed");
            return RunResults.Finished;
        }


        /// <summary>
        /// File.ReadAllBytes alternative to avoid read and/or write locking
        /// </summary>
        private static byte[] ReadAllBytes2(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        public RunResults UploadOnline(byte[] bytes, string remoteFilePath, bool saveToCache = false)
        {
            var blobClient = GetBlobClient(remoteFilePath);
            try
            {
                blobClient.Upload(new MemoryStream(bytes));
            }
            catch (Exception e)
            {
                return RunResults.NoInternetConnection;
            }

            WaitForNextConnection();
            using (var httpClient = new HttpClient(new WebRequestHandler { ReadWriteTimeout = 10 * 1000 })
            {
                // Specify request level timeout which decides maximum time taht can be spent on
                // download/upload files.
                Timeout = TimeSpan.FromMinutes(20)
            })
            {
                var config = new DropboxClientConfig("RT Report")
                {
                    HttpClient = httpClient
                };
                var client = new DropboxClient(_accessToken, config);

                FileUploadHelper.WriteLog(string.Concat("Uploading binary file to remote ", remoteFilePath));
                using (var stream = new MemoryStream(bytes))
                {
                    var response = client.Files.UploadAsync(remoteFilePath, WriteMode.Overwrite.Instance, body: stream);
                    if (!WaitTask(response).IsCompleted || response.Status == TaskStatus.Faulted)
                    {
                        FileUploadHelper.WriteLog("Failed " + response.Exception);
                        return RunResults.NoInternetConnection;
                    }
                    if (saveToCache)
                    {
                        SaveDataFileLocalCache(remoteFilePath, response.Result.Rev, bytes);
                    }
                }

                return RunResults.Finished;
            }

        }
        

        private Task WaitTask(Task task)
        {
            while (!(task.IsCompleted || task.IsCanceled || task.IsFaulted))
            {
                Thread.Sleep(100);
            }
            return task;
        }

        private static BlockBlobClient GetBlobClient(string remotePath)
        {
            var azureRemotePath = (remotePath.StartsWith("/") ? remotePath.Substring(1) : remotePath).ToLower();

            return new BlockBlobClient(
                "DefaultEndpointsProtocol=https;AccountName=metalogicreportingblob;AccountKey=3vWVwJcTe/2cmbKYnux7v+qk3cSkW1gbsyE1oVCKJ2kPk1uao4KiZMTxv65Sq/LK2UeDynvo4ZgvKOlHIC6wdA==;EndpointSuffix=core.windows.net",
                azureRemotePath.StartsWith("UT Reports\\Data", StringComparison.CurrentCultureIgnoreCase) ? "pdf" : "reports", azureRemotePath);
        }
    }
}
