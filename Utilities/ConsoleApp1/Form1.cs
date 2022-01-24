using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            enovaPicklistEdit1.PickListType = typeof(Class1);
            enovaPicklistEdit1.AdditionalDropDownColumns.Add("Field1");
        }
    }
}
