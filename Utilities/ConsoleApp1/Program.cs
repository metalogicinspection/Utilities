using DropboxUploader.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = DropboxHelper.GetDirectClient();
            client.UploadOnline(@"C:\ProgramData\Metalogic\PendingCrossionFile\fbb48f9c-6f79-4de7-8163-cc6094e64885",
             "/corrosion monitoring/999-99999/2bb2f85c-f490-4b99-868c-bb6750e32e9a.cmreport55"); 


            //try
            //{
            //    var result = client.UploadOnline(
            //        "e:\\test.uvdata",
            //        "/test/test.uvdata");

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    throw;
            //}
           }
    }
}
