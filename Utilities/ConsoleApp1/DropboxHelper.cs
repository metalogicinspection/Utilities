using System;
using System.IO;
using System.Net;
using DropboxUploader.Core;

namespace ConsoleApp1
{
    internal class DropboxHelper
    {
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://www.dropbox.com"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        internal static DropboxDirectClient GetDirectClient()
        {
            return new DropboxDirectClient(GetLocalDirBase(), _dropboxKey);

        }

        private static readonly string _dropboxKey = "TeATRlb9VWgAAAAAAAAAQlsO5puMgPEBYzp0zo16I8ii5kTwsHsKDlg4eNVVymjK";
        //jiahui@hotmail.com "GYCluLX9W_AAAAAAAAAAXZqNZZcR2X8kBlqsACvIGwXcVOREH_eKhJcs0PK-MkZO";

        internal static string GetLocalDirBase()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Metalogic",
                FileUploadHelper.GenerateGuidStr(string.Concat("rAo!b8a4", _dropboxKey, "4a&8(")));
        }

        internal static string GetLocalUvDataPendingUploadFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Metalogic",
               "PendingUVDataUpload");
        }



    }
}
