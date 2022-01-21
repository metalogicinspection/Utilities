using DropboxUploader.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metalogic.UI.Header.GridAttributeInfo;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var propertyInfo in typeof(Class1).GetProperties())
            {
                var attributes = propertyInfo.GetCustomAttributes(typeof(GridColumnVisibleIndex), false).ToList();
            }

            var columns = typeof(Class1).GetProperties()
                .Select(x => new
                {
                    Prpty = x,
                    VisibleIndex =
                        (x.GetCustomAttributes(typeof(GridColumnVisibleIndex), false).FirstOrDefault() as
                            GridColumnVisibleIndex)
                })
            .Where(x => (x.VisibleIndex?.VisibleIndex??-1) >= 0).OrderBy(x => x.VisibleIndex.VisibleIndex)
            .Select(x => x.Prpty.Name).ToList();
            //Application.Run(new Form1());
            //var client = DropboxHelper.GetDirectClient();
            //client.UploadOnline(@"C:\ProgramData\Metalogic\PendingCrossionFile\fbb48f9c-6f79-4de7-8163-cc6094e64885",
            // "/corrosion monitoring/999-99999/2bb2f85c-f490-4b99-868c-bb6750e32e9a.cmreport55"); 


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
