using System.Reflection;

namespace Gen3.Data
{
    public class DataListItemPropertyChanged : DataListChangedItem
    {
        public PropertyInfo Property { get; internal set; }

        public object NewPropertyValue { get; internal set; }
        public object OldPropertyValue { get; internal set; }

    }
}