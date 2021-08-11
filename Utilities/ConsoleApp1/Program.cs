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
                "C:\\Users\\User\\Documents\\dummy.uvdata",
                "/dummy.uvdata");
        }
    }
}
