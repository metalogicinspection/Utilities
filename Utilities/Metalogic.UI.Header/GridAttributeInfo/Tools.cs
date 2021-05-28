using System;
using System.Linq;
using System.Reflection;

namespace Metalogic.UI.Header.GridAttributeInfo
{
    public static class Tools
    {
        public static T GetAttribute<T>(this PropertyInfo prpty, string viewName) where T : GridViewFiedlAttributeBase
        {
            var flags = prpty.GetCustomAttributes(typeof(T), true)
                            .Cast<T>()
                            .ToArray();

            var colDsc = flags.FirstOrDefault(x => x.ViewName == viewName) ??
                         flags.FirstOrDefault(x => string.IsNullOrEmpty(x.ViewName));

            return colDsc;
        }

        public static GridViewFiedlAttributeBase GetAttribute(this PropertyInfo prpty, string viewName, Type attributeType)
        {
            var flags = prpty.GetCustomAttributes(attributeType, true)
                            .Cast<GridViewFiedlAttributeBase>()
                            .ToArray();

            var colDsc = flags.FirstOrDefault(x => x.ViewName == viewName) ??
                         flags.FirstOrDefault(x => string.IsNullOrEmpty(x.ViewName));

            return colDsc;
        }

        
    }
}
