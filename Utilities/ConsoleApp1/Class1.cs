using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metalogic.DataUtil;
using Metalogic.UI.Header.GridAttributeInfo;

namespace ConsoleApp1
{
    public class Class1 : PicklistItem
    {
        [GridColumnVisibleIndex(1)]
        public string Field1 { get; set; }
        public static Class1 A = new Class1() {Field1 = "abc"};
        public static Class1 B = new Class1() { Field1 = "bcd" };
    }
}
