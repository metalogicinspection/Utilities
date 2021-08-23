using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace DropboxUploader.Core
{
    public static class FileUploadHelper
    {
        internal static void CheckCreateDir(DirectoryInfo dir)
        {
            if (!dir.Exists)
            {
                dir.Create();
                try
                {
                    var sec = Directory.GetAccessControl(dir.FullName);
                    // Using this instead of the "Everyone" string means we work on non-English systems.
                    var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                    sec.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.FullControl | FileSystemRights.Synchronize, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                    dir.SetAccessControl(sec);
                }
                catch (Exception e)
                {
                }
            }
        }

        internal static void WriteLog(string log)
        {
#if DEBUG
            Console.WriteLine("Log: " + log);
#endif
        }

        public static void RemotePathCombine(object rootFolder, string v, object p)
        {
            throw new NotImplementedException();
        }

        public static string RemoteFileName(string remotePath)
        {
            var lastIndex = remotePath.LastIndexOf('/');
            return lastIndex < 0 ? string.Empty : remotePath.Substring(lastIndex +1);
        }

        public static string RemoteDirName(string remotePath)
        {
            var lastIndex = remotePath.LastIndexOf('/');
            return remotePath.Substring(0,lastIndex);
        }

        private static void AddPath(StringBuilder sb, string current)
        {
            if (sb.Length > 0 && sb[sb.Length-1] == '/' )
            {
                if (current[0] == '/')
                {
                    sb.Append(current.Substring(1));
                    return;
                }
                sb.Append(current);
                return;
            }

            if (current[0] == '/')
            {
                sb.Append(current);
                return;
            }

            sb.Append('/');
            sb.Append(current);

        }
        public static string GenerateGuidStr(byte[] data)
        {
            var guid = new byte[16];

            Array.Copy(data, guid, Math.Min(data.Length, guid.Length));

            // Don't use Guid constructor, it tends to swap bytes. We want to preserve original string as hex dump.
            var guidS = string.Concat("{",
                           $"{guid[0]:X2}{guid[1]:X2}{guid[2]:X2}{guid[3]:X2}-{guid[4]:X2}{guid[5]:X2}-{guid[6]:X2}{guid[7]:X2}-{guid[8]:X2}{guid[9]:X2}-{guid[10]:X2}{guid[11]:X2}{guid[12]:X2}{guid[13]:X2}{guid[14]:X2}{guid[15]:X2}", "}");

            return guidS;
        }


        public static string GenerateGuidStr(string name)
        {
            var buf = Encoding.UTF8.GetBytes(name);
            var guid = new byte[16];
            if (buf.Length < 16)
            {
                Array.Copy(buf, guid, buf.Length);
            }
            else
            {
                using (var sha1 = SHA1.Create())
                {
                    var hash = sha1.ComputeHash(buf);
                    // Hash is 20 bytes, but we need 16. We loose some of "uniqueness", but I doubt it will be fatal
                    Array.Copy(hash, guid, 16);
                }
            }
            
            // Don't use Guid constructor, it tends to swap bytes. We want to preserve original string as hex dump.
            var guidS = string.Concat("{",
                           $"{guid[0]:X2}{guid[1]:X2}{guid[2]:X2}{guid[3]:X2}-{guid[4]:X2}{guid[5]:X2}-{guid[6]:X2}{guid[7]:X2}-{guid[8]:X2}{guid[9]:X2}-{guid[10]:X2}{guid[11]:X2}{guid[12]:X2}{guid[13]:X2}{guid[14]:X2}{guid[15]:X2}", "}");

            return guidS;
        }

        public static string RemotePathCombine(string p1)
        {
            var sb = new StringBuilder();
            AddPath(sb, p1);


            return sb.ToString();
        }

        public static string RemotePathCombine(string p1, string p2)
        {
            var sb = new StringBuilder();
            AddPath(sb, p1);
            AddPath(sb, p2);


            return sb.ToString();
        }

        public static string RemotePathCombine(string p1, string p2, string p3)
        {
            var sb = new StringBuilder();
            AddPath(sb, p1);
            AddPath(sb, p2);
            AddPath(sb, p3);

            return sb.ToString();
        }

        public static string RemotePathCombine(string p1, string p2, string p3, string p4)
        {
            var sb = new StringBuilder();
            AddPath(sb, p1);
            AddPath(sb, p2);
            AddPath(sb, p3);
            AddPath(sb, p4);

            return sb.ToString();
        }

        public static string RemotePathCombine(string p1, string p2, string p3, string p4, string p5)
        {
            var sb = new StringBuilder();
            AddPath(sb, p1);
            AddPath(sb, p2);
            AddPath(sb, p3);
            AddPath(sb, p4);
            AddPath(sb, p5);

            return sb.ToString();
        }
    }
}