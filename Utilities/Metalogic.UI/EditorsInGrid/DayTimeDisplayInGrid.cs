using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;

namespace Metalogic.UI.EditorsInGrid
{
    public class DayTimeDisplayInGrid : RepositoryItemTextEdit
    {
        public DayTimeDisplayInGrid()
        {
            Mask.MaskType = MaskType.DateTime;
            Mask.EditMask = "yyyy-MM-dd HH:mm";
            Mask.UseMaskAsDisplayFormat = true;
        }
    }

    public class DayDisplayInGrid : RepositoryItemTextEdit
    {
        public DayDisplayInGrid()
        {
            Mask.MaskType = MaskType.DateTime;
            Mask.EditMask = "yyyy-MM-dd";
            Mask.UseMaskAsDisplayFormat = true;
        }
    }
}
