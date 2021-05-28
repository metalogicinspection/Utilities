using System;
using System.Linq;

namespace Metalogic.DataUtil
{
    public static class PickListExtentions
    {
        public static bool IsCodeOfPickList<T>(this string code) where T : PicklistItem
        {
            return ToPickListItem<T>(code)!= null;
        }

        public static T ToPickListItem<T>(this string code) where T : PicklistItem
        {
            var items = PicklistItem.GetPickList<T>();
            return items.FirstOrDefault(x => code.Equals(x.Code))
                   ?? items.FirstOrDefault(x => code.Equals(x.Code, StringComparison.OrdinalIgnoreCase))
                   ?? items.FirstOrDefault(x => code.Equals(x.BackupCode1, StringComparison.OrdinalIgnoreCase))
                   ?? items.FirstOrDefault(x => code.Equals(x.BackupCode2, StringComparison.OrdinalIgnoreCase))
                   ?? items.FirstOrDefault(x => code.Equals(x.BackupCode3, StringComparison.OrdinalIgnoreCase));
        }
    }
}
