using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metalogic.DataUtil
{
    public static class UtilLinqHelper
    {
        public static void SetObjPropertyValue(this object obj, string propertyName, object value)
        {
            if (obj == null)
            {
                throw new NullReferenceException();
            }
            var prpty = obj.GetType().GetProperties().FirstOrDefault(x => x.Name.Equals(propertyName));
            if (prpty == null)
            {
                throw new Exception("prperty " + propertyName + " not found!");
            }
            prpty.SetValue(obj, value);

        }

        public static object GetObjPropertyValue(this object obj, string propertyName)
        {
            if (obj == null)
            {
                throw new NullReferenceException();
            }
            var prpty = obj.GetType().GetProperties().FirstOrDefault(x => x.Name.Equals(propertyName));
            if (prpty == null)
            {
                throw new Exception("prperty " + propertyName + " not found!");
            }
            return prpty.GetValue(obj);
        }
    }
}
