using System;
using System.Reflection;

namespace Gen3.Data
{
    public class IngoreOperationBase
    {

        public Type DataListMemberType { get; protected set; }
    }

    public class IngoreOperationOnType : IngoreOperationBase
    {
        public IngoreOperationOnType(Type dataListMemberType)
        {DataListMemberType = dataListMemberType;
        }
    }

    public class IngoreOperationOnTypeProperty : IngoreOperationBase
    {

        public PropertyInfo Property { get; protected set; }

        protected IngoreOperationOnTypeProperty()
        {
        }

        public IngoreOperationOnTypeProperty(Type dataListMemberType, string propertyName)
        {
            DataListMemberType = dataListMemberType;
            Property = dataListMemberType.GetProperty(propertyName);
        }

        internal bool IgnoreAllPropertiesOfMemberType
            => Property !=null;
    }

}
