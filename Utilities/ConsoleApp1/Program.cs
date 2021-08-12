using DropboxUploader.Core;
using System;
using System.Collections.Generic;
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


            client.UploadOnline(
                "C:\\Users\\tduggan\\Documents\\dummy.uvdata",
                 "/ut reports/Details/41bd96b3-6fb6-0ebf-4a88-24aaff5acb4b.uvdata");
        }
    }
}
