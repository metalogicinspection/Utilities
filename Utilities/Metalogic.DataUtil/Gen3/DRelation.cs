using System.Collections.Generic;
using System.Reflection;

namespace Gen3.Data
{
    public class DRelation
    {
        public int RelationIndex { get; internal set; }
        public string RelationName { get; internal set; }

        public PropertyInfo ParentProperty { get; internal set; }

        public IGen3DataList ChildList { get; internal set; }
        public PropertyInfo ChildProperty { get; internal  set; }

        internal List<object> ChildListBackup { get; set; } 
    }


}