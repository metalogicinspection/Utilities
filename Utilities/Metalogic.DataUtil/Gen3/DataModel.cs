using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using Metalogic.DataUtil;
using Metalogic.DataUtil.SevenZip;

namespace Gen3.Data
{
    public class DataModel
    {
        public DataModel()
        {
            DataComponentName = "DataSet";
        }

        public string DataComponentName { get; set; }

        private readonly List<IGen3DataList> _lists = new List<IGen3DataList>();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<IGen3DataList> ToList()
        {
            return _lists.ToList();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IGen3DataList AddList(IGen3DataList list)
        {
            if (list == null)
            {
                throw new Exception("List is null");
            }
            _lists.Add(list);
            return list;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Gen3DataList<TItemType> CreateAddListByType<TItemType>(IEnumerable<TItemType> srcItems) where TItemType : class
        {
            var list = new Gen3DataList<TItemType>(srcItems);
            AddList(list);
            return list;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Gen3DataList<TItemType> CreateAddListByType<TItemType>() where TItemType : class
        {
            var list = new Gen3DataList<TItemType>();
            AddList(list);
            return list;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IGen3DataList CreateAddListByType(Type itemType)
        {
            var listType = typeof(Gen3DataList<>).MakeGenericType(itemType);
            var listTrg = Activator.CreateInstance(listType) as IGen3DataList;
            AddList(listTrg);
            return listTrg;
        }

        /// <summary>
        /// return list of TItemType, if not exist, create an empty list 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Gen3DataList<TItemType> GetOrCreateAddListByType<TItemType>() where TItemType : class
        {
            return GetList<TItemType>()?? CreateAddListByType<TItemType>();
        }

        /// <summary>
        /// return list of itemType, if not exist, create an empty list 
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IGen3DataList GetOrCreateAddListByType(Type itemType)
        {
            return GetListByType(itemType) ?? CreateAddListByType(itemType);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IGen3DataList CreateAddListByItem(object item)
        {
            var listType = typeof(Gen3DataList<>).MakeGenericType(item.GetType());
            var listTrg = Activator.CreateInstance(listType) as IGen3DataList;
            listTrg.Add(item);
            AddList(listTrg);
            return listTrg;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public T GetSingleton<T>() where T: class 
        {
            var list = _lists.FirstOrDefault(x => x.MemberType == typeof (T));
            if (list == null || list.Count < 1)
            {
                return null;
            }
            return list[0] as T;
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetSingleton<T>(T value) where T : class
        {
            var list = _lists.FirstOrDefault(x => x.MemberType == typeof(T));
            if (list == null)
            {
                return;
            }
            if (list.Count < 1)
            {
                list.Add(value);
                return;
            }
            list[0] = value;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LoadDataFromOtherConditional(MCDataSet source, Type[] types )
        {
            var lists = types == null 
                ? _lists 
                : _lists.Join(types, x => x.MemberType, y => y, (x, y) => x).ToList();

            foreach (var listTrg in lists)
            {
                var tblSrc = source.GetTableByTemplateType(listTrg.MemberType);
                if (tblSrc == null || tblSrc.Rows.Count < 1)
                {
                    continue;
                }

                listTrg.Clear();
                var prpties = listTrg.MemberType.GetProperties().Where(x => x.CanWrite).ToList();
                foreach (DataRow itemRowSrc in tblSrc.Rows)
                {
                    var itemTrg = Activator.CreateInstance(listTrg.MemberType);
                    foreach (var propertyInfo in prpties)
                    {
                        var value = itemRowSrc[propertyInfo.Name];
                        value = value == DBNull.Value ? null : value;
                        propertyInfo.SetValue(itemTrg, value, null);
                    }
                    listTrg.Add(itemTrg);
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LoadDataFromOtherConditional(MCDataSet source)
        {
            LoadDataFromOtherConditional(source, null);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LoadDataFromOtherConditional(DataModel source)
        {
            LoadDataFromOtherConditional(source, null);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LoadDataFromOtherConditional(DataModel source, Type[] types )
        {
            var lists = types == null
                  ? _lists
                  : _lists.Join(types, x => x.MemberType, y => y, (x, y) => x).ToList();
            
            foreach (var listTrg in lists)
            {
                var listSrc = source.GetListByType(listTrg.MemberType)?.ToIList();
                if (listSrc == null || listSrc.Count < 1)
                {
                    continue;
                }

                listTrg.Clear();
                var prpties = listTrg.MemberType.GetProperties().Where(x => x.CanWrite).ToList();
                foreach (var itemSrc in listSrc)
                {
                    var itemTrg = Activator.CreateInstance(listTrg.MemberType);
                    foreach (var propertyInfo in prpties)
                    {
                        propertyInfo.SetValue(itemTrg, propertyInfo.GetValue(itemSrc, null), null);
                    }
                    listTrg.Add(itemTrg);
                }
            }
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataModel Copy()
        {
            var retValue = new DataModel() {DataComponentName = DataComponentName};
            foreach (var listSrc in _lists)
            {
                var listType = typeof (Gen3DataList<>).MakeGenericType(listSrc.MemberType);
                var listTrg = Activator.CreateInstance(listType) as IGen3DataList;
                listTrg.Name = listSrc.Name;

                var prpties = listSrc.MemberType.GetProperties(BindingFlags.Instance| BindingFlags.Public) .Where(x => x.CanWrite ).ToList();
                foreach (var itemSrc in listSrc.ToIList())
                {
                    var itemTrg = Activator.CreateInstance(listSrc.MemberType);
                    foreach (var propertyInfo in prpties)
                    {
                        propertyInfo.SetValue(itemTrg, propertyInfo.GetValue(itemSrc, null), null);
                    }
                    listTrg.Add(itemTrg);
                }
                retValue.AddList(listTrg);
            }

            return retValue;
        }

        public DataModel(MCDataSet set)
        {
            DataComponentName = set.DataComponentName;
            foreach (TableBase table in set.Tables)
            {
                var listType = typeof (Gen3DataList<>).MakeGenericType(table.MemberType);
                var list = Activator.CreateInstance(listType) as IGen3DataList;
                list.Name = table.TableName;
                foreach (DataRow row in table.Rows)
                {
                    list.Add(row.ToObject(table.MemberType));
                }
                AddList(list);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddRelation(string relationName, IGen3DataList parentList, string parentPropertyName, IGen3DataList childList,
            string childPropertyName)
        {
            relationName = relationName?.Trim();
            if (string.IsNullOrWhiteSpace(relationName))
            {
                throw new Exception(relationName + " is empty");
            }

            if (parentList == null)
            {
                throw new Exception("parentList is null");
            }
            if (!_lists.Contains(parentList))
            {
                throw new Exception("parentList does not belongs to this data container");
            }

            if (!_lists.Contains(childList))
            {
                throw new Exception("childList does not belongs to this data container");
            }
            
            (parentList as IHasChildableList).AddRelation(relationName, parentPropertyName, childList, childPropertyName);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public Gen3DataList<T> GetList<T>() where T : class
        {
            return _lists.FirstOrDefault(x => x.MemberType == typeof (T)) as Gen3DataList<T>;

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IGen3DataList GetListByType(Type memberType)
        {
            return _lists.FirstOrDefault(x => x.MemberType == memberType);

        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public IGen3DataList GetListByName(string name)
        {
            return _lists.FirstOrDefault(x => x.Name.Equals(name));

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public MCDataSet ToDataSet()
        {
            var set = new MCDataSet(DataComponentName);
            foreach (IGen3DataList list in _lists)
            {
                var table = new TableBase(list.MemberType) {TableName = list.Name};
                foreach (var item in list)
                {
                    table.AddNewRow(item);
                }
                set.Tables.Add(table);
            }
            return set;
        }


        [MethodImpl(MethodImplOptions.Synchronized)]public void LoadDataFromOtherConditional(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                return;
            }
            try
            {
                var bytes = File.ReadAllBytes(fileInfo.FullName);
                LoadDataFromOtherConditional(bytes);
            }
            catch (Exception)
            {
                return;
            }

        }


        private static string DataSetSchemaNodeName = "DataSetSchema";

        private static string TABLE_HEADER_PREFIX = "Table_";
        private static string TABLE_HEADER_SUFFIX = "_Type";
        private static int[] COMPRESSED_MARKER = {93, 0, 0, 128, 0};

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serilizedDataSet">contains serilized data of data set</param>
        public void LoadDataFromOtherConditional(byte[] serilizedDataSet)
        {
            var doc = new XmlDocument();
            MemoryStream fs = null;

            try
            {

                var matched = true;
                for (var i = 0; i < COMPRESSED_MARKER.Length; ++i)
                {
                    if (serilizedDataSet[i] != COMPRESSED_MARKER[i])
                    {
                        matched = false;
                    }
                }

                if (!matched)
                {
                    fs = new MemoryStream(serilizedDataSet);
                }
                else
                {
                    var deCompressed = CompressHelper.Decompress(serilizedDataSet);
                    fs = new MemoryStream(deCompressed);
                }
                doc.Load(fs);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to load xml" + "/n" + e.Message);
            }
            finally
            {
                try
                {
                    fs?.Close();
                }
                catch (Exception)
                {

                }
            }

            var node = doc.DocumentElement;
            if (node == null)
            {
                throw new Exception("Failed to load xml");
            }

            LoadDataFromOtherConditional(node);
        }



        public void LoadDataFromOtherConditional(XmlElement node)
        {
            DataModel tmp;
            if (!TryLoadFromXMLFile(node, out tmp))
            {
                return;
            }

            foreach (var curTrgTbl in _lists)
            {
                var srcTbl =
                    tmp._lists.FirstOrDefault(x => x.MemberType == curTrgTbl.MemberType && x.Name == curTrgTbl.Name) ??
                    tmp._lists.FirstOrDefault(x => x.MemberType == curTrgTbl.MemberType);
                if (srcTbl == null || srcTbl.Count < 1)
                {
                    continue;
                }

                curTrgTbl.Clear();
                foreach (var item in srcTbl)
                {
                    curTrgTbl.Add(item);
                }
            }
        }


        public static bool TryLoadFromXMLFile(string filePath, out DataModel retValue)
        {
            return TryLoadFromXMLFile(new FileInfo(filePath), out retValue);
        }

        public static bool TryLoadFromXMLFile(FileInfo fInfo, out DataModel retValue)
        {
            retValue = null;
            if (!fInfo.Exists)
            {
                return false;
            }
            bool succeed;
            try
            {
                var bytes = File.ReadAllBytes(fInfo.FullName);
                succeed = TryLoadFromXMLFile(bytes, out retValue);
            }
            catch (Exception)
            {
                retValue = null;
                return false;
            }
            return succeed;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="serilizedDataSet">contains serilized data of data set</param>
        public static bool TryLoadFromXMLFile(byte[] serilizedDataSet, out DataModel set)
        {
            var doc = new XmlDocument();
            MemoryStream fs = null;
            var seceed = false;
            try
            {

                var matched = true;
                for (var i = 0; i < COMPRESSED_MARKER.Length; ++i)
                {
                    if (serilizedDataSet[i] != COMPRESSED_MARKER[i])
                    {
                        matched = false;
                    }
                }

                if (!matched)
                {
                    fs = new MemoryStream(serilizedDataSet);
                }
                else
                {
                    var deCompressed = CompressHelper.Decompress(serilizedDataSet);
                    fs = new MemoryStream(deCompressed);
                }
                doc.Load(fs);

                var node = doc.DocumentElement;
                if (node == null)
                {
                    set = null;
                }
                else
                {
                    seceed = TryLoadFromXMLFile(node, out set);
                }
            }
            catch (Exception e)
            {
                set = null;
                seceed = false;
            }
            finally
            {
                try
                {
                    fs?.Close();
                }
                catch (Exception)
                {

                }
            }
            return seceed;
        }


        private static bool TryLoadFromXMLFile(XmlElement node, out DataModel set)
        {
            set = new DataModel();

            var legacyTblNameToType = new Dictionary<string, string>();

            var rowNodes = node.ChildNodes.OfType<XmlNode>().ToList();
            if (rowNodes.Count == 1 && "NewDataSet".Equals(rowNodes[0].Name))
            {
                rowNodes = rowNodes[0].ChildNodes.OfType<XmlNode>().ToList();
                foreach (XmlAttribute attr in node.Attributes)
                {
                    if (!attr.Name.StartsWith(TABLE_HEADER_PREFIX) && !attr.Name.EndsWith(TABLE_HEADER_SUFFIX))
                    {
                        continue;
                    }
                    var tableName = attr.Name.Substring(TABLE_HEADER_PREFIX.Length,
                        attr.Name.Length - TABLE_HEADER_PREFIX.Length - TABLE_HEADER_SUFFIX.Length);

                    legacyTblNameToType[tableName] = attr.Value;
                }
            }



            foreach (var groupedRows in rowNodes.GroupBy(x => x.Name))
            {
                var firstRowNode = groupedRows.First();
                var typeName = firstRowNode.Attributes.Count > 0 && firstRowNode.Attributes[0].Name == "Type"
                    ? firstRowNode.Attributes[0].Value
                    : legacyTblNameToType.ContainsKey(groupedRows.Key)
                        ? legacyTblNameToType[groupedRows.Key]
                        : groupedRows.Key;
                var type = ReflectionUtilFunctions.GetType(typeName);
                if (type == null)
                {
                    continue;
                }

                var listType = typeof (Gen3DataList<>).MakeGenericType(type);
                var targetTbl = Activator.CreateInstance(listType) as IGen3DataList;
                targetTbl.Name = groupedRows.Key;
                var trgProperties = targetTbl.MemberType.GetProperties().Where(x => x.CanWrite)
                    .Select(x => new
                    {
                        Prpty = x,
                        IsCompilerGeneratedAttribute =
                            x.GetSetMethod()?.GetCustomAttributes(typeof (CompilerGeneratedAttribute), true).Any() ??
                            false
                    })
                    .ToList();
                foreach (var dataRowNode in groupedRows)
                {
                    var obj = Activator.CreateInstance(targetTbl.MemberType);

                    var propertyNodePairs = trgProperties
                        .LeftOuterJoin(dataRowNode.ChildNodes.OfType<XmlNode>(), x => x.Prpty.Name, y => y.Name,
                            (x, y) => new {x.Prpty, Node = y, x.IsCompilerGeneratedAttribute})
                        .ToList();


                    foreach (var pair in propertyNodePairs)
                    {
                        var dataCellNode = pair.Node ??
                                           pair.Prpty.GetCustomAttributes(typeof (LegacyPropertyName), true)
                                               .OfType<LegacyPropertyName>()
                                               .Join(dataRowNode.ChildNodes.OfType<XmlNode>(), x => x.Name, y => y.Name,
                                                   (x, y) => y)
                                               .FirstOrDefault();

                        object value = null;
                        var strValue = dataCellNode?.Attributes.OfType<XmlAttribute>()
                            .FirstOrDefault(x => "V".Equals(x.Name))?
                            .Value ??
                                       dataCellNode?.InnerText;
                        if (dataCellNode == null)
                        {
                            if (pair.Prpty.PropertyType == TYPE_STRING
                                && pair.IsCompilerGeneratedAttribute
                                && pair.Prpty.GetValue(obj, null) == null)
                            {
                                value = string.Empty;
                            }
                            else if (pair.Prpty.PropertyType == TYPE_GUID
                               && pair.IsCompilerGeneratedAttribute
                               && "Hash".Equals(pair.Prpty.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                value = Guid.Empty;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else if (pair.Prpty.PropertyType == TYPE_STRING)
                        {
                            value = strValue;
                        }
                        else if (pair.Prpty.PropertyType == TYPE_GUID)
                        {
                            Guid dValue;
                            if (string.IsNullOrEmpty(strValue))
                            {
                                value = default(Guid);
                            }
                            else if (Guid.TryParse(strValue, out dValue))
                            {
                                value = dValue;
                            }
                        }
                        else if (pair.Prpty.PropertyType == TYPE_DOUBLE)
                        {
                            double dValue;
                            if (string.IsNullOrEmpty(strValue))
                            {
                                value = default(double);
                            }
                            else if (double.TryParse(strValue, out dValue))
                            {
                                value = dValue;
                            }
                        }
                        else if (pair.Prpty.PropertyType == TYPE_FLOAT)
                        {
                            float fValue;
                            if (string.IsNullOrEmpty(strValue))
                            {
                                value = default(float);
                            }
                            else if (float.TryParse(strValue, out fValue))
                            {
                                value = fValue;
                            }
                        }
                        else if (pair.Prpty.PropertyType == TYPE_INT)
                        {
                            int fValue;
                            if (string.IsNullOrEmpty(strValue))
                            {
                                value = default(int);
                            }
                            else if (int.TryParse(strValue, out fValue))
                            {
                                value = fValue;
                            }
                        }
                        else if (pair.Prpty.PropertyType == TYPE_DATETIME)
                        {
                            DateTime fValue;
                            if (string.IsNullOrEmpty(strValue))
                            {
                                value = default(DateTime);
                            }
                            else if (DateTime.TryParse(strValue, out fValue))
                            {
                                value = fValue;
                            }
                        }
                        else if (pair.Prpty.PropertyType == TYPE_LONG)
                        {
                            long fValue;
                            if (string.IsNullOrEmpty(strValue))
                            {
                                value = default(long);
                            }
                            else if (long.TryParse(strValue, out fValue))
                            {
                                value = fValue;
                            }
                        }
                        else if (pair.Prpty.PropertyType == TYPE_SHORT)
                        {
                            short fValue;
                            if (string.IsNullOrEmpty(strValue))
                            {
                                value = default(short);
                            }
                            else if (short.TryParse(strValue, out fValue))
                            {
                                value = fValue;
                            }
                        }
                        else if (pair.Prpty.PropertyType == TYPE_BOOL)
                        {
                            bool fValue;
                            if (string.IsNullOrEmpty(strValue))
                            {
                                value = default(bool);
                            }
                            else if (bool.TryParse(strValue, out fValue))
                            {
                                value = fValue;
                            }
                        }
                        else if (pair.Prpty.PropertyType == TYPE_BYTEARRAY)
                        {
                            if (strValue == string.Empty)
                            {
                                continue;
                            }
                            try
                            {
                                value = Convert.FromBase64String(strValue);
                            }
                            catch (Exception)
                            {
                                value = null;
                            }
                        }
                        else if (pair.Prpty.PropertyType == TYPE_COLOR)
                        {
                            int argb;
                            if (!int.TryParse(strValue, out argb))
                            {
                                continue;
                            }

                            try
                            {
                                var attribute =
                                    pair.Node.Attributes?.OfType<XmlAttribute>()
                                        .FirstOrDefault(x => x.Name.Equals("ColorName"));
                                Color c;
                                if (attribute != null && (c = Color.FromName(attribute.Value)).ToArgb() == argb)
                                {
                                    value = c;
                                }
                            }
                            finally
                            {
                                if (value == null)
                                {
                                    value = Color.FromArgb(argb);
                                }
                            }
                        }
                        else if (pair.Prpty.PropertyType == TYPE_PICKLIST_ITEM_BASE)
                        {
                            var privit = -1;
                            Type pickListType = null;
                            PicklistItem item = null;

                            if (!string.IsNullOrWhiteSpace(strValue) &&
                                (privit = strValue.LastIndexOf(".", StringComparison.Ordinal)) > 0 &&
                                (pickListType = ReflectionUtilFunctions.GetType(strValue.Substring(0, privit))) != null &&
                                pickListType.IsSubclassOf(TYPE_PICKLIST_ITEM_BASE) &&
                                (item = PicklistItem.GetPickListItem(pickListType, strValue.Substring(privit + 1))) != null)
                            {
                                value = item;
                            }
                            else
                            {
                                pair.Prpty.SetValue(obj, null, null);
                            }
                        }
                        else if (pair.Prpty.PropertyType.IsSubclassOf(TYPE_PICKLIST_ITEM_BASE))
                        {
                            value = PicklistItem.GetPickListItem(pair.Prpty.PropertyType, strValue);
                            if (value == null && pair.Node != null)
                            {
                                pair.Prpty.SetValue(obj, null, null);
                            }
                        }
                        else if (pair.Prpty.PropertyType == TYPE_TIMESPAN)
                        {
                            TimeSpan span;
                            if (!string.IsNullOrWhiteSpace(strValue) &&
                                TimeSpan.TryParse(strValue, out span))
                            {
                                value = span;
                            }
                        }

                        if (value != null)
                        {
                            pair.Prpty.SetValue(obj, value, null);
                        }
                    }
                    targetTbl.Add(obj);
                }
                set.AddList(targetTbl);
            }
            return true;
        }


        private static readonly Type TYPE_PICKLIST_ITEM_BASE = typeof(PicklistItem);
        private static readonly Type TYPE_STRING = typeof(string);
        private static readonly Type TYPE_INT = typeof(int);
        private static readonly Type TYPE_GUID = typeof(Guid);
        private static readonly Type TYPE_DOUBLE = typeof(double);
        private static readonly Type TYPE_FLOAT = typeof(float);
        private static readonly Type TYPE_BYTEARRAY = typeof(byte[]);
        private static readonly Type TYPE_DATETIME = typeof(DateTime);
        private static readonly Type TYPE_LONG = typeof(long);
        private static readonly Type TYPE_SHORT = typeof(short);
        private static readonly Type TYPE_BOOL = typeof(bool);
        private static readonly Type TYPE_COLOR = typeof(Color);
        private static readonly Type TYPE_TIMESPAN = typeof(TimeSpan);
        public void SaveToXMLFile(XmlElement node, List<IngoreOperationBase> ignoreColumnsNames = null)
        {
            var ignoreTypes = ignoreColumnsNames?
                               .OfType<IngoreOperationOnType>()
                               .Select(x => x.DataListMemberType).ToList();

            var ignoreColumns = ignoreColumnsNames?.OfType<IngoreOperationOnTypeProperty>().ToList();

            foreach (var curTable in _lists)
            {
                if (ignoreTypes?.Any(x =>  x == curTable.MemberType) ?? false)
                {
                    continue;
                }

                var ignoreColumnsForThisTYpe =
                    ignoreColumns?.Where(x => x.DataListMemberType == curTable.MemberType)
                        .Select(x => x.Property).ToList()?? new List<PropertyInfo>();

                var columns = curTable.MemberType.GetProperties()
                    .Where(x => x.CanWrite && !ignoreColumnsForThisTYpe.Contains(x))
                    .Select(x =>
                        new
                        {
                            Property = x,
                            HasAlternateName = x.GetCustomAttributes(typeof (LegacyPropertyName), true).Any(),
                            IsCompilerGeneratedAttribute =
                                x.GetSetMethod()?.GetCustomAttributes(typeof (CompilerGeneratedAttribute), true).Any() ??
                                false,
                        })
                    .ToList();

                var addedTypeLabel = false;
                var defaultObj = Activator.CreateInstance(curTable.MemberType);
                foreach (var row in curTable)
                {
                    var rowNode = node.OwnerDocument.CreateElement(curTable.Name);
                    if (!addedTypeLabel)
                    {
                        rowNode.SetAttribute("Type", curTable.MemberType.FullName);
                        addedTypeLabel = true;
                    }
                    foreach (var column in columns)
                    {
                        var cellNode = node.OwnerDocument.CreateElement(column.Property.Name);
                        var value = column.Property.GetValue(row, null);
                        if (column.Property.PropertyType == TYPE_BOOL)
                        {
                            cellNode.SetAttribute("V", value.ToString());
                        }
                        else if (column.Property.PropertyType == TYPE_INT)
                        {
                            cellNode.SetAttribute("V", value.ToString());
                        }
                        else if (column.Property.PropertyType == TYPE_SHORT)
                        {
                            cellNode.SetAttribute("V", value.ToString());
                        }
                        else if (column.Property.PropertyType == TYPE_LONG)
                        {
                            cellNode.SetAttribute("V", value.ToString());
                        }
                        else if (column.Property.PropertyType == TYPE_COLOR)
                        {
                            var color = (Color) value;
                            cellNode.SetAttribute("V", color.ToArgb().ToString());
                            cellNode.SetAttribute("ColorName", color.Name);
                        }
                        else if (column.Property.PropertyType == TYPE_BYTEARRAY)
                        {
                            if (value != null)
                            {
                                cellNode.InnerText = Convert.ToBase64String((byte[]) value);
                            }
                            else if (!column.HasAlternateName)
                            {
                                continue;
                            }
                        }
                        else if (column.Property.PropertyType == TYPE_DOUBLE)
                        {
                            var castedValue = (double) value;
                            cellNode.SetAttribute("V", castedValue.ToString("R"));
                        }
                        else if (column.Property.PropertyType == TYPE_FLOAT)
                        {
                            var castedValue = (float) value;
                            cellNode.SetAttribute("V", castedValue.ToString("R"));
                        }
                        else if (column.Property.PropertyType == TYPE_GUID)
                        {
                            var guidValue = (Guid) value;
                            cellNode.SetAttribute("V", guidValue.ToString());
                        }
                        else if (column.Property.PropertyType == TYPE_DATETIME)
                        {
                            cellNode.SetAttribute("V", ((DateTime) value).ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz"));
                        }
                        else if (column.Property.PropertyType == TYPE_PICKLIST_ITEM_BASE)
                        {
                            if (value == null)
                            {
                                continue;
                            }

                            cellNode.SetAttribute("V", string.Concat(value.GetType().FullName, ".", value.ToString()));
                        }
                        else if (column.Property.PropertyType.IsSubclassOf(TYPE_PICKLIST_ITEM_BASE))
                        {
                            if (value == null)
                            {
                                continue;
                            }

                            cellNode.SetAttribute("V", value.ToString());
                        }
                        else if (column.Property.PropertyType.IsSubclassOf(TYPE_TIMESPAN))
                        {
                            if (value == null)
                            {
                                continue;
                            }

                            cellNode.SetAttribute("V", value.ToString());
                        }
                        else
                        {
                            var strValue = value?.ToString() ?? string.Empty;

                            if (!strValue.Equals(string.Empty))
                            {
                                //if (string.IsNullOrWhiteSpace(strValue) && strValue.Length > 0)
                                //{
                                //    cellNode.SetAttribute("xml:space", "preserve");
                                //}

                                cellNode.SetAttribute("V", strValue);
                            }
                            else if (!column.HasAlternateName
                                     && column.IsCompilerGeneratedAttribute
                                     && string.IsNullOrEmpty(column.Property.GetValue(defaultObj, null)?.ToString()))
                            {
                                continue;
                            }
                        }

                        rowNode.AppendChild(cellNode);
                    }
                    node.AppendChild(rowNode);
                }
            }
        }
        
        public void SaveToXMLFile(FileInfo file,  bool compressed = false)
        {
            if (file.Exists)
            {
                file.Delete();
            }
            var doc = new XmlDocument();
            SaveToXMLFile(doc, null);

            if (!compressed)
            {
                doc.Save(file.FullName);
                return;
            }

            var stream = new MemoryStream();
            doc.Save(stream);
            byte[] bytes = stream.GetBuffer();
            var compressedBytes = CompressHelper.Compress(bytes);
            File.WriteAllBytes(file.FullName, compressedBytes);
        }


        public void SaveToXMLFile(XmlDocument doc, List<IngoreOperationBase> ignoreColumnsNames = null)
        {
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            var node = doc.CreateElement(DataComponentName);
            SaveToXMLFile(node, ignoreColumnsNames);
            doc.AppendChild(node);
        }

        public byte[] ToByteArray(List<IngoreOperationBase> ignoreColumnsNames = null, bool compressed = false)
        {
            var doc = new XmlDocument();
            SaveToXMLFile(doc, ignoreColumnsNames);
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                doc.Save(stream);
                bytes = stream.GetBuffer();
            }


            return compressed ? CompressHelper.Compress(bytes) : bytes;
        }

        public void ReplaceItemsOfList(IList itemsToReplace)
        {
            if (!IsList(itemsToReplace))
            {
                throw new Exception("content is not a List<>");
            }
            
            var type = itemsToReplace.GetType().GetGenericArguments()[0];
            var list = GetListByType(type);
            if (list == null)
            {
                throw new Exception("list of type " + type.Name + " does not exist in DataModel");
            }

            list.Clear();
            foreach (var item in itemsToReplace)
            {
                list.Add(item);
            }
        }

        private bool IsList(object o)
        {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public bool HasChanges(DataModel other, List<IngoreOperationBase> ignoreColumnsNames = null)
        {
            return Diff(other, ignoreColumnsNames).Any();
        }

        public List<DataListChangedItem> Diff(DataModel other, List<IngoreOperationBase> ignoreColumnsNames = null)
        {
            var retValue = new List<DataListChangedItem>();
            
            if (_lists.Count != other._lists.Count)
            {
                throw new Exception("Two datacontainers has different lists count.");
            }

            var ignoreTypes = ignoreColumnsNames?
                                .OfType<IngoreOperationOnType>()
                                .Select(x=> x.DataListMemberType).ToList();

            var ignoreColumns = ignoreColumnsNames?.OfType<IngoreOperationOnTypeProperty>().ToList();
            for (var i = 0; i < _lists.Count; ++i)
            {
                var curSetTable = _lists[i];
                var otrTable = other._lists[i];
                if (ignoreTypes?.Contains(curSetTable.MemberType)??false)
                {
                    continue;
                }

                var curTableIgnoreColumns = ignoreColumns?.Where(x => x.DataListMemberType == curSetTable.MemberType).Select(x => x.Property.Name).ToList();
                retValue.AddRange(curSetTable.Diff(otrTable, curTableIgnoreColumns));
            }
            return retValue;
        }
    }
}