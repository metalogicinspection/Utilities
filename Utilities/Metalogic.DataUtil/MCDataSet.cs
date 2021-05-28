using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using Metalogic.DataUtil.SevenZip;

namespace Metalogic.DataUtil
{
    [ToolboxItem(false)]
    public class MCDataSet : DataSet
    {
        public string DataComponentName { get; set; }

        public MCDataSet(string dataComponentName)
        {
            DataComponentName = dataComponentName;
        }

        private MCDataSet()
        {
        }

        public T GetSingleton<T>() where T : class
        {
            var t = typeof (T);
            var table = Tables.OfType<TableBase>().FirstOrDefault(tbl => tbl.MemberType == t);
            if (table == null)
            {
                return null;
            }
            return table.GetFirstRowAsObject() as T;
        }

        public void SetSingleton(object value)
        {
            if (value == null)
            {
                throw new Exception("value can not be null");
            }
            var t = value.GetType();
            var table = Tables.OfType<TableBase>().FirstOrDefault(tbl => tbl.MemberType == t);
            if (table == null)
            {
                Tables.Add(TableBase.CreateTableByItem(value));
            }
            else if (table.Rows.Count == 1)
            {
                table.Rows[0].UpdateFields(value);
            }
            else 
            {
                table.Rows.Clear();
                table.AddNewRow(value);
            }
        }

        public void ReplaceList(IList list)
        {
            if (list == null)
            {
                throw new Exception("list can not be null");
            }
            var t = list.GetType().GetGenericArguments()[0];
            var table = Tables.OfType<TableBase>().FirstOrDefault(tbl => tbl.MemberType == t);
            if (table == null)
            {
                table = new TableBase(t);
                Tables.Add(table);
            }
            if (table.Rows.Count >0)
            {
                table.Rows.Clear();
            }
            foreach (var item in list)
            {
                table.AddNewRow(item);
            }
        }

        public List<T> GetListOfObjects<T>() where T : class
        {
            var t = typeof(T);
            var table = Tables.OfType<TableBase>().FirstOrDefault(tbl => tbl.MemberType == t);
            if (table == null)
            {
                throw new Exception("Table of type " + t.Name + " not found.");
            }


            return table.AsEnumerable().Select(x => x.ToObject<T>()).ToList();
        }



        private static string DataSetSchemaNodeName = "DataSetSchema";

        private static string TABLE_HEADER_PREFIX = "Table_";
        private static string TABLE_HEADER_SUFFIX = "_Type";
        private static int[] COMPRESSED_MARKER = {93, 0, 0, 128, 0};

        public void SaveToXMLFile(XmlElement node)
        {
            var tables = Tables.OfType<TableBase>().ToList();
            foreach (var curTable in tables)
            {
                var columns = curTable.MemberType.GetProperties()
                    .Where(x => x.CanWrite)
                    .Join(curTable.Columns.OfType<DataColumn>(), x => x.Name, y => y.ColumnName,
                        (x, y) =>
                            new
                            {
                                Column = y,
                                Property = x,
                                HasAlternateName = x.GetCustomAttributes(typeof (LegacyPropertyName), true).Any(),
                                IsCompilerGeneratedAttribute = x.GetSetMethod()?.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any()??false,
                            })
                    .ToList();

                var addedTypeLabel = false;
                var defaultObj = Activator.CreateInstance(curTable.MemberType);
                foreach (DataRow row in curTable.Rows)
                {
                    var rowNode = node.OwnerDocument.CreateElement(curTable.TableName);
                    if (!addedTypeLabel)
                    {
                        rowNode.SetAttribute("Type", curTable.MemberType.FullName);
                        addedTypeLabel = true;
                    }
                    foreach (var column in columns)
                    {
                        var cellNode = node.OwnerDocument.CreateElement(column.Column.ColumnName);
                        var value = row[column.Column];
                        if (column.Column.DataType == TYPE_BOOL)
                        {
                            var castedValue = value != DBNull.Value && (bool) value;
                            if (castedValue != default(bool))
                            {
                                cellNode.SetAttribute("V", castedValue.ToString());
                            }
                        }
                        else if (column.Column.DataType == TYPE_INT)
                        {
                            var castedValue = value == DBNull.Value ? default(int) : (int) value;
                            if (castedValue != default(int))
                            {
                                cellNode.SetAttribute("V", castedValue.ToString());
                            }
                        }
                        else if (column.Column.DataType == TYPE_SHORT)
                        {
                            var castedValue = value == DBNull.Value ? default(short) : (short) value;
                            if (castedValue != default(short))
                            {
                                cellNode.SetAttribute("V", castedValue.ToString());
                            }
                        }
                        else if (column.Column.DataType == TYPE_LONG)
                        {
                            var castedValue = value == DBNull.Value ? default(long) : (long) value;
                            if (castedValue != default(long))
                            {
                                cellNode.SetAttribute("V", castedValue.ToString());
                            }
                        }
                        else if (column.Column.DataType == TYPE_COLOR)
                        {
                            if (value != DBNull.Value)
                            {
                                var color = (Color)value;
                                cellNode.SetAttribute("V", color.ToArgb().ToString());
                                cellNode.SetAttribute("ColorName", color.Name);
                            }
                        }
                        else if (column.Column.DataType == TYPE_BYTEARRAY)
                        {
                            if (value != DBNull.Value)
                            {
                                cellNode.InnerText = Convert.ToBase64String((byte[])value);
                            }
                            else if (!column.HasAlternateName)
                            {
                                continue;
                            }
                        }
                        else if (column.Column.DataType == TYPE_DOUBLE)
                        {
                            var castedValue = value == DBNull.Value ? default(double) : (double) value;
                            if (castedValue != default(double))
                            {
                                cellNode.SetAttribute("V", castedValue.ToString("R"));
                            }
                        }
                        else if (column.Column.DataType == TYPE_FLOAT)
                        {
                            var castedValue = value == DBNull.Value ? default(float) : (float) value;
                            if (castedValue != default(float))
                            {
                                cellNode.SetAttribute("V", castedValue.ToString("R"));
                            }
                        }
                        else if (column.Column.DataType == TYPE_GUID)
                        {
                            var guidValue = value == DBNull.Value ? default(Guid) : (Guid) value;
                            if (guidValue != default(Guid))
                            {
                                cellNode.SetAttribute("V", guidValue.ToString());
                            }
                        }
                        else if (column.Column.DataType == TYPE_DATETIME)
                        {
                            var dValue = value == DBNull.Value ? default(DateTime) : (DateTime) value;
                            if (dValue != default(DateTime))
                            {
                                cellNode.SetAttribute("V", dValue.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz"));
                            }
                        }
                        else if (column.Column.DataType == TYPE_PICKLIST_ITEM_BASE)
                        {
                            if (value != DBNull.Value)
                            {
                                cellNode.SetAttribute("V", string.Concat(value.GetType().FullName, ">", value.ToString()));
                            }
                        }
                        else if (column.Column.DataType.IsSubclassOf(TYPE_PICKLIST_ITEM_BASE))
                        {
                            if (value != DBNull.Value)
                            {
                                cellNode.SetAttribute("V", value.ToString());
                            }
                        }
                        else if (column.Column.DataType.IsSubclassOf(TYPE_TIMESPAN))
                        {
                            if (value != DBNull.Value)
                            {
                                cellNode.SetAttribute("V", value.ToString());
                            }
                        }
                        else
                        {
                            var strValue = value == DBNull.Value ? string.Empty : value.ToString();

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
                                && string.IsNullOrEmpty(column.Property.GetValue(defaultObj,null)?.ToString()))
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
        
        public void SaveToXMLFile(FileInfo file, bool compressed = false)
        {
            if (file.Exists)
            {
                file.Delete();
            }
            var doc = new XmlDocument();
            SaveToXMLFile(doc);

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


        public void SaveToXMLFile(XmlDocument doc)
        {
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            var node = doc.CreateElement(DataComponentName);
            SaveToXMLFile(node);
            doc.AppendChild(node);
        }

        public byte[] ToByteArray(bool compressed = false)
        {
            var doc = new XmlDocument();
            SaveToXMLFile(doc);
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                doc.Save(stream);
                bytes = stream.GetBuffer();
            }


            return compressed ? CompressHelper.Compress(bytes) : bytes;
        }


        
        public static bool TryLoadFromXMLFile(string filePath, out MCDataSet retValue)
        {
            return TryLoadFromXMLFile(new FileInfo(filePath), out retValue);
        }

        public static bool TryLoadFromXMLFile(FileInfo fInfo, out MCDataSet retValue)
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
        public static bool TryLoadFromXMLFile(byte[] serilizedDataSet, out MCDataSet set)
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


        private static bool TryLoadFromXMLFile(XmlElement node, out MCDataSet set)
        {
            set = new MCDataSet(node.Name);

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



            set.EnforceConstraints = false;
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

                var targetTbl = new TableBase(type) {TableName = groupedRows.Key};
                var trgProperties = targetTbl.MemberType.GetProperties().Where(x => x.CanWrite)
                    .Select(x => new {
                        Prpty = x,
                        IsCompilerGeneratedAttribute = x.GetSetMethod()?.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any() ?? false
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
                            value = Convert.FromBase64String(strValue);
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
                                (privit = strValue.IndexOf(">", StringComparison.Ordinal)) > 0 &&
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
                    targetTbl.AddNewRow(obj);
                }
                set.AddTable(targetTbl);
            }
            set.EnforceConstraints = true;
            return true;
        }


        private void LoadDataFromOther(MCDataSet source)
        {
            EnforceConstraints = false;

            foreach (DataTable trgTable in Tables)
            {
                var srcTable = source.Tables[trgTable.TableName];
                if (srcTable == null)
                {
                    //throw new Exception("Tables not match. Data Table " + trgTable.TableName + " is missing from source");
                    continue;
                }

                trgTable.Import(srcTable);
            }
            EnforceConstraints = true;

            AcceptChanges();
        }

        public void LoadDataFromOtherConditional(MCDataSet source)
        {
            EnforceConstraints = false;
            var trgTbls = Tables.OfType<TableBase>().ToList();
            foreach (var srcTbl in source.Tables.OfType<TableBase>().Where(x => x.Rows.Count > 0).ToList())
            {
                var trgTbl =
                    trgTbls.FirstOrDefault(
                        x => x.TableName.Equals(srcTbl.TableName) && x.MemberType == srcTbl.MemberType) ??
                    trgTbls.FirstOrDefault(x => x.MemberType == srcTbl.MemberType);
                if (trgTbl == null)
                {
                    continue;
                }

                trgTbl.Rows.Clear();

                foreach (var itm in srcTbl.AsEnumerable().Select(x => x.ToObject()))
                {
                    trgTbl.AddNewRow(itm);
                }
            }
            EnforceConstraints = true;
        }
        
        public void LoadDataFromOtherConditional(FileInfo fileInfo)
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

        public void LoadDataFromOtherConditional(XmlElement node)
        {
            DataComponentName = node.Name;

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



            var targetTables = Tables.OfType<TableBase>().ToList();
            EnforceConstraints = false;
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

                var targetTbl = targetTables.FirstOrDefault(x => x.TableName.Equals(groupedRows.Key) && x.MemberType == type) ??
                                targetTables.FirstOrDefault(x => x.MemberType == type);
                if (targetTbl == null)
                {
                    continue;
                }

                var trgProperties = targetTbl.MemberType.GetProperties().Where(x => x.CanWrite)
                    .Select(x=> new {
                        Prpty = x,
                        IsCompilerGeneratedAttribute = x.GetSetMethod()?.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any() ?? false })
                        .ToList();
                targetTbl.Rows.Clear();
                foreach (var dataRowNode in groupedRows)
                {
                    var obj = Activator.CreateInstance(targetTbl.MemberType);

                    var propertyNodePairs = trgProperties
                        .LeftOuterJoin(dataRowNode.ChildNodes.OfType<XmlNode>(), x => x.Prpty.Name, y => y.Name,
                            (x, y) => new { x.Prpty, Node = y, x.IsCompilerGeneratedAttribute })
                        .ToList();


                    foreach (var pair in propertyNodePairs)
                    {
                        var dataCellNode = pair.Node ??pair.Prpty.GetCustomAttributes(typeof(LegacyPropertyName), true)
                                            .OfType<LegacyPropertyName>()
                                            .Join(dataRowNode.ChildNodes.OfType<XmlNode>(), x => x.Name, y => y.Name, (x, y) => y)
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
                            value = Convert.FromBase64String(strValue);
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
                                (privit = strValue.IndexOf(">", StringComparison.Ordinal))> 0 &&
                                (pickListType = ReflectionUtilFunctions.GetType(strValue.Substring(0, privit)))!= null &&
                                pickListType.IsSubclassOf(TYPE_PICKLIST_ITEM_BASE) &&
                                (item = PicklistItem.GetPickListItem(pickListType, strValue.Substring(privit + 1)))!= null)
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
                    targetTbl.AddNewRow(obj);
                }
            }
        }

        public override DataSet Clone()
        {
            var retValue = base.Clone() as MCDataSet;
            retValue.DataComponentName = DataComponentName;
            foreach (TableBase table in Tables)
            {
                (retValue.Tables[table.TableName] as TableBase).MemberType = table.MemberType;
            }
            return retValue;
        }

        /// <summary>
        /// Add table to this dataset, and return table for other function
        /// </summary>
        /// <param name="table">the table that want to add to the dataset</param>
        /// <returns>return input parameter table</returns>
        public TableBase AddTable(TableBase table)
        {
            Tables.Add(table);
            return table;
        }


        public new MCDataSet Copy()
        {
            var doc = new XmlDocument();

            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            var node = doc.CreateElement(DataComponentName);
            SaveToXMLFile(node);

            var retValue = Clone() as MCDataSet;

            retValue.LoadDataFromOtherConditional(node);

            return retValue;
        }
        public class IngoreTableColumnOnDiff
        {
            public string TableName { get; set; }

            public string ColumnName { get; set; }
        }

        public bool HasChanges(MCDataSet other, List<IngoreTableColumnOnDiff> ignoreColumnsNames = null)
        {
            return Diff(other, ignoreColumnsNames).Any();
        }


        public List<DataTableChangedItem> Diff(MCDataSet other, List<IngoreTableColumnOnDiff> ignoreColumnsNames = null)
        {
            var retValue = new List<DataTableChangedItem>();
            foreach (var curSetTable in Tables.OfType<TableBase>())
            {
                var otrTable = other.Tables[curSetTable.TableName] as TableBase;
                if (otrTable == null || curSetTable.MemberType != otrTable.MemberType)
                {
                    throw new Exception(string.Concat("Datable ", curSetTable.TableName, "　not found on other"));
                }
                var curTableIgnoreColumns = ignoreColumnsNames?.Where(x => curSetTable.TableName.Equals(x.TableName)).Select(x => x.ColumnName).ToList();
                retValue.AddRange(curSetTable.Diff(otrTable, curTableIgnoreColumns));
            }
            return retValue;
        }

        public MCDataSet GetCopyOfTypes(Type[] carryOverTypes)
        {
            var retValue = Copy();
            retValue.Relations.Clear();
            foreach (var tbl in retValue.Tables.OfType<TableBase>())
            {
                tbl.Constraints.Clear();
            }
            retValue.Tables.OfType<TableBase>()
                .Where(x => !carryOverTypes.Contains(x.MemberType))
                .ToList()
                .ForEach(x => retValue.Tables.Remove(x));
            return retValue;
        }

        public DateTime GetSingletonDateProperty<T>(string property) where T : class
        {
            return GetSingletonProperty<T, DateTime>(property);
        }

        public bool GetSingletonBooleanProperty<T>(string property) where T : class
        {
            return GetSingletonProperty<T, bool>(property);
        }

        public double GetSingletonDouletProperty<T>(string property) where T : class
        {
            return GetSingletonProperty<T, double>(property);
        }

        public float GetSingletonFloatProperty<T>(string property) where T : class
        {
            return GetSingletonProperty<T, float>(property);
        }

        public int GetSingletonIntProperty<T>(string property) where T : class
        {
            return GetSingletonProperty<T, int>(property);
        }

        public long GetSingletonLongProperty<T>(string property) where T : class
        {
            return GetSingletonProperty<T, long>(property);
        }

        public string GetSingletonStringProperty<T>(string property) where T : class
        {
            var t = typeof(T);
            var table = Tables.OfType<TableBase>().FirstOrDefault(tbl => tbl.MemberType == t);
            if (table == null)
            {
                throw new Exception(string.Concat("Can not find table of type: ", t.FullName));
            }
            if (table.Rows.Count < 1)
            {
                throw new Exception(string.Concat("table of type: ", t.FullName, " has no data rows"));
            }

            var c = table.Columns[property];
            if (c == null)
            {
                throw new Exception(string.Concat("can not find property ", property));
            }

            if (c.DataType != typeof(string))
            {
                throw new Exception(string.Concat(property, " is not a ", typeof(string), "!"));
            }

            return table.Rows[0][c] == DBNull.Value ? null : (string)table.Rows[0][c];
        }

        public TDatatype GetSingletonProperty<T, TDatatype>(string property) where T : class where TDatatype : struct
        {
            var t = typeof(T);
            var table = Tables.OfType<TableBase>().FirstOrDefault(tbl => tbl.MemberType == t);
            if (table == null)
            {
                throw new Exception(string.Concat("Can not find table of type: ", t.FullName));
            }
            if (table.Rows.Count < 1)
            {
                throw new Exception(string.Concat("table of type: ", t.FullName, " has no data rows"));
            }

            var c = table.Columns[property];
            if (c == null)
            {
                throw new Exception(string.Concat("can not find property ", property));
            }

            if (c.DataType != typeof(TDatatype))
            {
                throw new Exception(string.Concat(property, " is not a ", typeof(TDatatype), "!"));
            }

            return table.Rows[0][c] == DBNull.Value ? default(TDatatype) : (TDatatype)table.Rows[0][c];
        }


        public PK GetSingletonPicklistProperty<T, PK>(string property) where T : class where PK : PicklistItem
        {
            var t = typeof(T);
            var table = Tables.OfType<TableBase>().FirstOrDefault(tbl => tbl.MemberType == t);
            if (table == null)
            {
                throw new Exception(string.Concat("Can not find table of type: ", t.FullName));
            }
            if (table.Rows.Count < 1)
            {
                throw new Exception(string.Concat("table of type: ", t.FullName, " has no data rows"));
            }

            var c = table.Columns[property];
            if (c == null)
            {
                throw new Exception(string.Concat("can not find property ", property));
            }
            var value = table.Rows[0][c];
            value = value != DBNull.Value ? value : null;
            return c.DataType.IsSubclassOf(typeof(PicklistItem))? value as PK:
                    value?.ToString().ToPickListItem<PK>();
        }

        public void SetSingletonProperty<T>(string property, object value) where T : class
        {
            var t = typeof(T);
            var table = Tables.OfType<TableBase>().FirstOrDefault(tbl => tbl.MemberType == t);
            if (table == null)
            {
                throw new Exception(string.Concat("Can not find table of type: ", t.FullName));
            }
            if (table.Rows.Count < 1)
            {
                throw new Exception(string.Concat("table of type: ", t.FullName, " has no data rows"));
            }

            var c = table.Columns[property];
            if (c == null)
            {
                throw new Exception(string.Concat("can not find property ", property, " in type ", t.Name));
            }

            table.Rows[0][c] = value;
        }




    }
    
}
