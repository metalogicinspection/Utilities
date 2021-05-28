using System;
using Metalogic.UI.Header.GridAttributeInfo;

namespace Metalogic.UI.Header.DataTypeAttributeInfo
{
    public class DecimalPlaces : GridViewFiedlAttributeBase
    {
        public DecimalPlaces(int places)
        {
            Places = places;
        }

        public int Places { get; set; }
    }
}