using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Metalogic.UI.Header.GridAttributeInfo;


namespace Metalogic.UI.Header.DataTypeAttributeInfo
{
    public class DataMaxString : GridViewFiedlAttributeBase
    {
        public DataMaxString(int length)
        {
            Length = length;
        }

        public int Length { get; set; }
    }
}
