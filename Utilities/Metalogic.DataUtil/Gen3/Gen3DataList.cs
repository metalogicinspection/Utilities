using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Gen3.Data
{
    public class Gen3DataList<T> : BindingList<T>, IGen3DataList, IHasChildableList where T : class
    {

        public T AddPassThrough(T item)
        {
            Add(item);
            return item;
        }


        public Type MemberType => typeof(T);
        
        public Gen3DataList()
        {
            Name = MemberType.Name;
        }


        public string Name { get; set; }

        public Gen3DataList(IEnumerable<T> list) : this()
        {
            foreach (var item in list)
            {
                Add(item);
            }
        }


        public static Gen3DataList<T> CreateTableByItems(IEnumerable<T> list)
        {

            return new Gen3DataList<T>(list);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public IList ToIList()
        {
            return this.ToList();
        }
        
        
        public int IndexOf(object value)
        {
            var item = value as T;
            if (item == null)
            {
                return -1;
            }

            var i = 0;
            foreach (var curItem in this)
            {
                if (curItem == item)
                {
                    return i;
                }
                ++i;
            }
            return -1;
        }
        
        public string GetRelationName(int index, int relationIndex)
        {
            return _relations[relationIndex].RelationName;
        }

        public bool IsMasterRowEmpty(int index, int relationIndex)
        {
            return GetDetailListInternal(index, relationIndex).Count < 1;
        }

        private void Condense_parentToChilDataLists()
        {
            if (_parentToChilDataLists == null)
            {
                return;
            }
            //************clear child lists' listerner if the parent object is removed from this list
            foreach (var itemChildListContainer in _parentToChilDataLists.ToList())
            {
                var curParent = itemChildListContainer.Parent as T;
                if (curParent == null)
                {
                    throw new Exception("itemChildListContainer.Parent is not a type of " + typeof(T).Name);
                }
                if (Contains(curParent))
                {
                    continue;
                }
                itemChildListContainer.ChildRelationList.ListChanged -= Ret_ListChanged;
                _parentToChilDataLists.Remove(itemChildListContainer);
            }
            //************end of clearing
        }


        public IList GetDetailList(int index, int relationIndex)
        {
            var relation = _relations[relationIndex];
            var parent = this[index];
            return GetItemSpecificChildrenList(parent, relation);
        }

        public ItemSpecificChildrenList<TChildrenType> GetItemSpecificChildrenList<TChildrenType>(T item) where TChildrenType : class
        {
            var relation = _relations.FirstOrDefault(x => x.ChildList.MemberType == typeof (TChildrenType));
            if (relation == null)
            {
                throw new Exception("Child Relation of Type " + typeof(TChildrenType).Name + " have not added.");
            }
            return GetItemSpecificChildrenList(item, relation) as ItemSpecificChildrenList<TChildrenType>;
        }

        public IList GetItemSpecificChildrenList(T parent, DRelation relation)
        {
            Condense_parentToChilDataLists();
            var key = relation.ParentProperty.GetValue(parent, null);

            var container = _parentToChilDataLists?.FirstOrDefault(x => x.Parent == parent && x.Relation == relation);
            if (container != null)
            {
                return container.ChildRelationList;
            }

            if (_parentToChilDataLists == null)
            {
                _parentToChilDataLists = new List<ItemRelationChildrenContainer>();
            }
            var listType = typeof(ItemSpecificChildrenList<>).MakeGenericType(relation.ChildList.MemberType);
            var ret = Activator.CreateInstance(listType) as IItemSpecificChildrenListInternal;
            ret.Set(parent, relation, key);
            var back = new List<object>();

            var tmp = relation.ChildList.Where(x => key.Equals(relation.ChildProperty.GetValue(x, null)));
            foreach (var item in tmp)
            {
                ret.Add(item);
                back.Add(item);
            }

            container = new ItemRelationChildrenContainer
            {
                Parent = parent,
                Relation = relation,
                Key = key,
                ChildRelationList = ret,
                ChildRelationListBackup = back
            };

            _parentToChilDataLists.Add(container);


            ret.ListChanged += Ret_ListChanged;
            return ret;
        }

        private void Ret_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                var container = _parentToChilDataLists.FirstOrDefault(x => x.ChildRelationList == sender);
                if (container == null)
                {
                    return;
                }

                var childItem = container.ChildRelationList[e.NewIndex];
                container.Relation.ChildProperty.SetValue(childItem, container.Key);
                container.Relation.ChildList.ListChanged -= ChildRelationFullList_ListChanged;
                container.Relation.ChildList.Add(childItem);
                container.Relation.ChildListBackup.Add(childItem);
                container.ChildRelationListBackup.Add(childItem);
                container.Relation.ChildList.ListChanged += ChildRelationFullList_ListChanged;
                return;
            }

            if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                var container = _parentToChilDataLists.FirstOrDefault(x => x.ChildRelationList == sender);
                if (container == null)
                {
                    return;
                }
                var delItem = container.ChildRelationListBackup[e.NewIndex];
                container.Relation.ChildList.ListChanged -= ChildRelationFullList_ListChanged;
                container.ChildRelationListBackup.RemoveAt(e.NewIndex);
                container.Relation.ChildList.Remove(delItem);
                container.Relation.ChildListBackup.Remove(delItem);
                container.Relation.ChildList.ListChanged += ChildRelationFullList_ListChanged;
                return;
            }

            
        }

        private class ItemRelationChildrenContainer
        {
            public object Parent { get; set; }

            public IBindingList ChildRelationList { get; set; }

            public DRelation Relation { get; set; }
            public object Key { get; set; }

            public List<object> ChildRelationListBackup { get; set; }

        }

        private List<ItemRelationChildrenContainer> _parentToChilDataLists;

        

        public IList GetDetailListInternal(int index, int relationIndex)
        {
            var relation = _relations[relationIndex];
            var parentValue = relation.ParentProperty.GetValue(this[index], null);
            return relation.ChildList.Where(x => parentValue.Equals(relation.ChildProperty.GetValue(x, null)));
        }

        public int RelationCount => _relations?.Count ?? 0;

        private List<DRelation> _relations;
        void IHasChildableList.AddRelation(string relationName, string parentPropertyName, IGen3DataList childList, string childPropertyName)
        {

            var parentProperty = MemberType.GetProperties().FirstOrDefault(x => x.Name.Equals(parentPropertyName));
            if (parentProperty == null)
            {
                throw new Exception(parentPropertyName + " does not belongs to type " + MemberType.Name);
            }
            
            if (childList == null)
            {
                throw new Exception("childlist is null");
            }

            var childProperty = childList.MemberType.GetProperties().FirstOrDefault(x => x.Name.Equals(childPropertyName));
            if (childProperty == null)
            {
                throw new Exception(childPropertyName + " does not belongs to type " + childList.MemberType.Name);
            }

            if (_relations == null)
            {
                _relations = new List<DRelation>();
            }
            else if( _relations.Any(x=> x.ChildList == childList))
            {
                throw new Exception("child list already added before");
            }


            if (HasRelation(relationName))
            {
                throw new Exception("Relation " + relationName + " already existed.");
            }

            _relations.Add(new DRelation { RelationName = relationName, ParentProperty = parentProperty, ChildList = childList, ChildProperty = childProperty, ChildListBackup = childList.Cast<object>().ToList(), RelationIndex = _relations.Count});
            childList.ListChanged += ChildRelationFullList_ListChanged;
        }

        public void RemoveRelation(string relationName)
        {
            var relation =
                _relations.FirstOrDefault(
                    x => x.RelationName.Equals(relationName, StringComparison.InvariantCultureIgnoreCase));
            if (relation == null)
            {
                return;
            }
            relation.ChildList.ListChanged -= ChildRelationFullList_ListChanged;
            _relations.Remove(relation);
        }

        private void ChildRelationFullList_ListChanged(object sender, ListChangedEventArgs e)
        {

            var relation = _relations.FirstOrDefault(x => x.ChildList == sender);
            if (relation == null)
            {
                return;
            }

            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                var newChildItem = relation.ChildList[e.NewIndex];
                relation.ChildListBackup.Add(newChildItem);
                var newChildItemKey = relation.ChildProperty.GetValue(newChildItem, null);
                var itemSpecificChildList = _parentToChilDataLists?.FirstOrDefault(x => x.Key.Equals(newChildItemKey) && x.Relation == relation );
                if (itemSpecificChildList == null)
                {
                    return;
                }

                itemSpecificChildList.ChildRelationList.ListChanged -= Ret_ListChanged;
                itemSpecificChildList.ChildRelationList.Add(newChildItem);
                itemSpecificChildList.ChildRelationListBackup.Add(newChildItem);
                itemSpecificChildList.ChildRelationList.ListChanged += Ret_ListChanged;
            }

            if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                var oldChildItem = relation.ChildListBackup[e.NewIndex];
                relation.ChildListBackup.Remove(oldChildItem);
                var oldChildItemKey = relation.ChildProperty.GetValue(oldChildItem, null);


                var itemSpecificChildList = _parentToChilDataLists?.FirstOrDefault(x => x.Key.Equals(oldChildItemKey) && x.Relation == relation);
                if (itemSpecificChildList == null)
                {
                    return;
                }

                itemSpecificChildList.ChildRelationList.ListChanged -= Ret_ListChanged;
                itemSpecificChildList.ChildRelationList.Remove(oldChildItem);
                itemSpecificChildList.ChildRelationListBackup.Remove(oldChildItem);
                itemSpecificChildList.ChildRelationList.ListChanged += Ret_ListChanged;
            }

        }

        List<object> IGen3DataList.Where(Func<object, bool> condition)
        {
            return this.Where(condition).ToList();
        }

        public bool HasRelation(string relationName)
        {
            return _relations.Any(x => x.RelationName == relationName);
        }

        public IGen3DataList GetEmptyCopy()
        {
            return Activator.CreateInstance(GetType()) as IGen3DataList;
        }

        

        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<DataListChangedItem> Diff(IGen3DataList other, List<string> ignoreColumnsNames = null)
        {
            if (this.MemberType != other.MemberType)
            {
                throw new Exception(this.MemberType.Name + "Two lists has 2 different data type");
            }
            
            var retValue = new List<DataListChangedItem>();

            var properties = MemberType.GetProperties().Where(x => x.CanWrite && x.CanRead).ToList();
            if (ignoreColumnsNames != null && ignoreColumnsNames.Count> 0)
            {
                properties = properties.Where(x=> !ignoreColumnsNames.Contains(x.Name)).ToList();
            }

            var minCount = Math.Min(Count, other.Count);
            for (var i = 0; i < minCount; ++i)
            {
                var curRow = this[i];
                var preRow = other[i];
                foreach (var property in properties)
                {
                    var curValue = property.GetValue(curRow, null);
                    var previousValue = property.GetValue(preRow, null);
                    var same = true;
                    if (property.PropertyType == typeof (string))
                    {
                        curValue = (curValue as string) ?? string.Empty;
                        previousValue = (previousValue as string) ?? string.Empty;
                        same = curValue.Equals(previousValue);
                    }
                    else if (curValue == null && previousValue == null)
                    {
                        //do nothing
                    }
                    else if(curValue == null)
                    {
                        same = false;
                    }
                    else if (previousValue == null)
                    {
                        same = false;
                    }
                    else
                    {
                        same = curValue.Equals(previousValue);
                    }

                    if (!same)
                    {
                        retValue.Add(new DataListItemPropertyChanged { Index = i, NewDataList = this, OldDataList = other, NewItem = curRow, OldItem = preRow, Property = property, NewPropertyValue = curValue, OldPropertyValue = previousValue});
                    }
                }
            }

            for (var i = minCount; i < Count; ++i)
            {
                retValue.Add(new DataListItemAdded { Index = i, NewDataList = this, OldDataList = other, NewItem = this[i] });
            }

            for (var i = minCount; i < other.Count; ++i)
            {
                retValue.Add(new DataListItemDeleted { Index = i, NewDataList = this, OldDataList = other, OldItem = other[i] });
            }

           

            return retValue;
        }

        public DRelation[] Relations => _relations?.ToArray()??new DRelation[] {};
        
        public ItemAndItsListPair[] FindAllOffsprings(object item)
        {
            var retValue = new List<ItemAndItsListPair>();
            
            var qu = new Queue<ItemAndItsListPair>();
            qu.Enqueue(new ItemAndItsListPair { Item = item, DataList = this});
            var visited = new HashSet<object>();
            while (qu.Count > 0)
            {
                var cur = qu.Dequeue();
                retValue.Add(cur);
                visited.Add(cur.Item);

                foreach (var childRelation in cur.DataList.Relations)
                {
                    var key = childRelation.ParentProperty.GetValue(cur.Item, null);
                    foreach (var childItem in childRelation.ChildList.Where(x => 
                                    key.Equals(childRelation.ChildProperty.GetValue(x, null)) &&
                                    !visited.Contains(x))
                        .Select(x => new ItemAndItsListPair{ Item = x, DataList = childRelation.ChildList }))
                    {
                        qu.Enqueue(childItem);
                    }
                    
                }
            }
            retValue.RemoveAt(0);
            return retValue.ToArray();
        }
        
    }
}
