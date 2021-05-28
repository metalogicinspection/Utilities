using System;
using Gen3.Data;
using Metalogic.DataUtil;
using Metalogic.UI.Editors;

namespace Metalogic.UI
{
    public static class DataModelBindingHelper
    {
        public static void WriteValueIntoDataModel(IMetadataControl contrl, DataModel model)
        {
            var list = model.GetListByName(contrl.QueryTableName);
            if (list == null || list.Count < 1)
            {
                return;
            }
            var prpty = list.MemberType.GetProperty(contrl.QueryFiledName);
            if (prpty == null)
            {
                return;
            }

            var editValue = contrl.EditValue;
            if (prpty.PropertyType == typeof(double))
            {
                editValue = Convert.ToDouble(editValue);
            }
            else if (prpty.PropertyType == typeof(float))
            {
                editValue = Convert.ToSingle(editValue);
            }
            else if (prpty.PropertyType == typeof(int))
            {
                editValue = Convert.ToInt32(editValue);
            }
            else if (prpty.PropertyType == typeof(short))
            {
                editValue = Convert.ToInt16(editValue);
            }
            else if (prpty.PropertyType == typeof(long))
            {
                editValue = Convert.ToInt64(editValue);
            }
            else if (prpty.PropertyType == typeof(bool))
            {
                editValue = Convert.ToBoolean(editValue);
            }

            prpty.SetValue(list[0], editValue);
        }

        public static void ReadValueFromDataModel(IMetadataControl contrl, DataModel model)
        {
            var list = model.GetListByName(contrl.QueryTableName);
            if (list == null || list.Count < 1)
            {
                return;
            }

            var prpty = list.MemberType.GetProperty(contrl.QueryFiledName);
            if (prpty == null)
            {
                return;
            }
            contrl.EditValue = prpty.GetValue(list[0]);

            var contrlAsPicklist = contrl as EnovaPicklistEdit;
            if (contrlAsPicklist == null || prpty.PropertyType == typeof(PicklistItem))
            {
                return;
            }


            contrlAsPicklist.PickListType = prpty.PropertyType;
        }
    }
}
