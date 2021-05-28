using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data;

namespace Gen3.Data
{
    public interface IGen3DataList : IBindingListWithMemberType, IRelationList
    {
        IList ToIList();

        string Name { get; set; }


        List<object> Where(Func<object, bool> condition);

        bool HasRelation(string relationName);

        IGen3DataList GetEmptyCopy();


        List<DataListChangedItem> Diff(IGen3DataList other, List<string> ignoreColumnsNames = null);

        DRelation[] Relations { get; }

        ItemAndItsListPair[] FindAllOffsprings(object item);
        
    }


    public class ItemSpecificChildrenList<T> : BindingList<T>, IItemSpecificChildrenListInternal
    {
        public void Set(object parentObject, DRelation relation, object key)
        {
            ParentObject = parentObject;
            ParentRelation = relation;
            Key = key;
        }

        public object Key { get; internal set; }

        /// <summary>
        /// the parent object that genrate this child list
        /// </summary>
        public object ParentObject { get; internal set; }

        /// <summary>
        /// The relation that genreate this child list
        /// </summary>
        public DRelation ParentRelation { get; internal set; }

        public Type MemberType => typeof (T);
    }

    internal interface IItemSpecificChildrenListInternal : IItemSpecificChildrenList
    {
        void Set(object parentObject, DRelation relation, object key);
    }

    public interface IBindingListWithMemberType : IBindingList
    {
        Type MemberType { get; }
    }

    public interface IItemSpecificChildrenList : IBindingListWithMemberType
    {

        /// <summary>
        /// the parent object that genrate this child list
        /// </summary>
        object ParentObject { get;  }

        /// <summary>
        /// The relation that genreate this child list
        /// </summary>
        DRelation ParentRelation { get;  }

        object Key { get; }
    }

    public class ItemAndItsListPair
    {
        public object Item { get; internal set; } 
        public IGen3DataList DataList { get; internal set; }
    }

    internal interface IHasChildableList
    {
        void AddRelation(string relationName, string parentPropertyName, IGen3DataList childList, string childPropertyName);
        void RemoveRelation(string relationName);
    }
}