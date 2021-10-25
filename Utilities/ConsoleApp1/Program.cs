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
            //client.UploadOnline(@"E:\Reporting\Radiographic\Radiographic\bin\Release\Radiographic.exe",
            //    "/Radiographic.exe"); 


            try
            {
                var bytes = File.ReadAllBytes("e:\\test.mtreport");
                client.UploadOnline(
                    bytes,
                    "/test/test.text");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           }
    }
}
