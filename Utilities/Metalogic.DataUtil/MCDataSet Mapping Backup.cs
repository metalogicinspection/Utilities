using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
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
            else
            {
                table.Rows.Clear();
                table.AddNewRow(value);
            }
        }

        private static string DataSetSchemaNodeName = "DataSetSchema";

        private static string TABLE_HEADER_PREFIX = "Table_";
        private static string TABLE_HEADER_SUFFIX = "_Type";
        private static int[] COMPRESSED_MARKER = {93, 0, 0, 128, 0};



        /// <summary>
        /// 0 => A, 1 => B....25=> Z, 26=> BA， 27=>BB
        /// </summary>
        /// <param name="int"></param>
        /// <returns></returns>
        private static string IndexToAlphabic(int num)
        {
            if (num == 0)
            {
                return "A";
            }
            var stringValue = string.Empty;
            decimal columnNumber = num;
            while (columnNumber > 0)
            {
                var currentLetterNumber = (columnNumber) % 26;
                var currentLetter = (char)(currentLetterNumber + 65);
                stringValue = currentLetter + stringValue;
                columnNumber = (columnNumber - (currentLetterNumber)) / 26;
            }
            return stringValue;
        }
        
        public void SaveToXMLFile(XmlElement node)
        {
            node.SetAttribute("V", "3");
            var tables = Tables.OfType<TableBase>().ToList();
            for (var i = 0; i < tables.Count; ++i)
            {
                var curTable = tables[i];
                var defaultObj = Activator.CreateInstance(curTable.MemberType);
                var columns = curTable.MemberType.GetProperties()
                    .Where(x => x.CanWrite)
                    .Join(curTable.Columns.OfType<DataColumn>(), x => x.Name, y => y.ColumnName,
                        (x, y) =>
                            new
                            {
                                Column = y,
                                Property = x,
                                DefaultValue = x.GetValue(defaultObj, null) ?? (x.PropertyType == typeof(string) ? string.Empty : null),
                            })
                    .OrderBy(x => x.Column.ColumnName)
                    .ToList();

                var isFirstRow = true;
                foreach (DataRow row in curTable.Rows)
                {
                    var rowNode = node.OwnerDocument.CreateElement(IndexToAlphabic(i));
                    if (isFirstRow)
                    {
                        rowNode.SetAttribute("TYP", curTable.MemberType.FullName);
                        rowNode.SetAttribute("TBL", curTable.TableName);
                    }
                    for (var x = 0; x < columns.Count; ++x)
                    {
                        var column = columns[x];
                        var cellNode = node.OwnerDocument.CreateElement(IndexToAlphabic(x));
                        var value = row[column.Column];
                        if (isFirstRow)
                        {
                            cellNode.SetAttribute("N", column.Column.ColumnName);
                        }

                        if (column.Column.DataType == typeof(Bitmap))
                        {
                            if (value != DBNull.Value)
                            {
                                try
                                {
                                    var array = ImageToByte((Bitmap)value);

                                    cellNode.InnerText = Convert.ToBase64String(array);
                                }
                                finally
                                {
                                }
                            }
                            else if (!isFirstRow)
                            {
                                continue;
                            }
                        }
                        else if (column.Column.DataType == typeof(bool))
                        {
                            var castedValue = value != DBNull.Value && (bool)value;
                            if (castedValue != default(bool))
                            {
                                cellNode.SetAttribute("V", "T");
                            }
                            else if (!isFirstRow)
                            {
                                continue;
                            }
                        }
                        else if (column.Column.DataType == typeof(int))
                        {
                            var castedValue = value == DBNull.Value ? default(int) : (int)value;
                            if (castedValue != default(int))
                            {
                                cellNode.SetAttribute("V", castedValue.ToString());
                            }
                            else if (!isFirstRow)
                            {
                                continue;
                            }
                        }
                        else if (column.Column.DataType == typeof(short))
                        {
                            var castedValue = value == DBNull.Value ? default(short) : (short)value;
                            if (castedValue != default(short))
                            {
                                cellNode.SetAttribute("V", castedValue.ToString());
                            }
                            else if (!isFirstRow)
                            {
                                continue;
                            }
                        }
                        else if (column.Column.DataType == typeof(long))
                        {
                            var castedValue = value == DBNull.Value ? default(long) : (long)value;
                            if (castedValue != default(long))
                            {
                                cellNode.SetAttribute("V", castedValue.ToString());
                            }
                            else if (!isFirstRow)
                            {
                                continue;
                            }
                        }
                        else if (column.Column.DataType == typeof(Color))
                        {
                            var color = value == DBNull.Value ? Color.Transparent : (Color)value;
                            if (color != Color.Transparent)
                            {
                                cellNode.SetAttribute("V", color.ToArgb().ToString());
                                cellNode.SetAttribute("ColorName", color.Name);
                            }
                            else if (!isFirstRow)
                            {
                                continue;
                            }
                        }
                        else if (column.Column.DataType == typeof(byte[]))
                        {
                            if (value != DBNull.Value)
                            {
                                cellNode.InnerText = Convert.ToBase64String((byte[])value);
                            }
                            else if (!isFirstRow)
                            {
                                continue;
                            }
                        }
                        else if (column.Column.DataType == typeof(double))
                        {
                            var castedValue = value == DBNull.Value ? default(double) : (double)value;
                            if (castedValue != default(double))
                            {
                                cellNode.SetAttribute("V", castedValue.ToString("R"));
                            }
                            else if (!isFirstRow)
                            {
                                continue;
                            }
                        }
                        else if (column.Column.DataType == typeof(float))
                        {
                            var castedValue = value == DBNull.Value ? default(float) : (float)value;
                            if (castedValue != default(float))
                            {
                                cellNode.SetAttribute("V", castedValue.ToString("R"));
                            }
                            else if (!isFirstRow)
                            {
                                continue;
                            }
                        }
                        else if (column.Column.DataType == typeof(Guid))
                        {
                            var guidValue = value == DBNull.Value ? default(Guid) : (Guid)value;
                            if (guidValue != default(Guid))
                            {
                                cellNode.SetAttribute("V", guidValue.ToString());
                            }
                            else if (!isFirstRow)
                            {
                                continue;
                            }
                        }
                        else if (column.Column.DataType == typeof(DateTime))
                        {
                            var dValue = value == DBNull.Value ? default(DateTime) : (DateTime)value;
                            if (dValue != default(DateTime))
                            {
                                cellNode.SetAttribute("V", ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz"));
                            }
                            else if (!isFirstRow)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            var defaultValue = (string)column.DefaultValue;
                            if (isFirstRow && !string.IsNullOrEmpty(defaultValue))
                            {
                                cellNode.SetAttribute("DV", defaultValue);
                            }

                            var strValue = value == null || value == DBNull.Value ? string.Empty : value.ToString();
                            if (!strValue.Equals(defaultValue))
                            {
                                cellNode.SetAttribute("V", strValue);
                            }
                            else if (!isFirstRow)
                            {
                                continue;
                            }
                        }

                        rowNode.AppendChild(cellNode);
                    }
                    node.AppendChild(rowNode);
                    isFirstRow = false;
                }
            }
        }

        private static byte[] ImageToByte(Bitmap img)
        {
            //ImageConverter converter = new ImageConverter();
            //return (byte[])converter.ConvertTo(img, typeof(byte[]));

            byte[] jArray;
            using (var stream = new MemoryStream())
            {
                var i2 = new Bitmap(img);
                i2.Save(stream, ImageFormat.Jpeg);
                jArray = stream.ToArray();
            }
            return jArray;
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

        private class PerpertyExt
        {
            public int Order { get; set; }
            public PropertyInfo Prpty { get; set; }
            public string PrptyName { get; set; }

            public bool IsCompilerGeneratedAttribute { get; set; }
            public bool HasDefaultValueFromXMLFile { get; set; }
            public string DefaultValueString { get; set; }


            public string XMLTagName { get; set; }
        }
        private static bool IsCompilerGeneratedAttribute(PropertyInfo property)
        {
            return property.GetSetMethod()?.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any() ??
                   false;
        }

        public static bool TryLoadFromXMLFile(XmlElement node, out MCDataSet set)
        {
            try
            {
                if (node.Attributes.Count < 1 || !node.Attributes[0].Name.Equals("V"))
                {

                    return TryLoadFromXMLFileV1_V2(node, out set);
                }

                set = new MCDataSet(node.Name);

                var rowNodes = node.ChildNodes.OfType<XmlNode>().ToList();

                set.EnforceConstraints = false;

                var curTableTag = string.Empty;
                TableBase curTable = null;
                List<PerpertyExt> curTablePrpties = null;

                foreach (var curRowNode in rowNodes)
                {
                    if (!curRowNode.Name.Equals(curTableTag))
                    {
                        //current row node is the first row of given table
                        curTableTag = curRowNode.Name;
                        var typeName = curRowNode.Attributes[0].Value;
                        var type = ReflectionUtilFunctions.GetType(typeName);
                        curTable = new TableBase(type) { TableName = curRowNode.Attributes[1].Value };
                        var properties = curTable.MemberType.GetProperties().Where(x => x.CanWrite).ToList();

                        curTablePrpties = properties
                          .Select(x => new { Order = 0, Prpty = x, PrptyName = x.Name, IsCompilerGeneratedAttribute = IsCompilerGeneratedAttribute(x) })
                          .Union(
                                  //some of the property has different name when the xml file were created
                                  //use LegacyPropertyName tag to match current property name and property name at the time
                                  properties.SelectMany(x => x.GetCustomAttributes(typeof(LegacyPropertyName), true)
                                    .OfType<LegacyPropertyName>()
                                    .Select(legacyName => new { Order = 1, Prpty = x, PrptyName = legacyName.Name, IsCompilerGeneratedAttribute = IsCompilerGeneratedAttribute(x) })))
                          .LeftOuterJoin(
                              //get the list of properites when XML were created
                              //this list is stored in the first row of this given table in XML
                              curRowNode.ChildNodes.OfType<XmlElement>(),
                              x => x.PrptyName,
                              y => y.Attributes[0].Value,
                              (x, y) => new PerpertyExt
                              {
                                  Prpty = x.Prpty,
                                  PrptyName = x.PrptyName,
                                  Order = x.Order,
                                  XMLTagName = y?.Name ?? string.Empty,
                                  IsCompilerGeneratedAttribute = x.IsCompilerGeneratedAttribute,
                                  HasDefaultValueFromXMLFile = y != null,
                                  DefaultValueString = y?.Attributes.OfType<XmlAttribute>().FirstOrDefault(attrbute => attrbute.Name.Equals("DV"))?.Value ?? string.Empty
                              })
                          .GroupBy(x => x.PrptyName)
                          //if the same property exist current propety name tag and legacy name tag in xml, use curent name instead of legacy name tag
                          //order by .Order will filter out
                          .Select(x => x.OrderBy(y => y.Order).First())
                          .ToList();

                        set.AddTable(curTable);
                    }


                    var obj = Activator.CreateInstance(curTable.MemberType);

                    var propertyNodePairs = curTablePrpties
                        .LeftOuterJoin(curRowNode.ChildNodes.OfType<XmlNode>(), x => x.XMLTagName, y => y.Name,
                            (x, y) => new {
                                x.Prpty,
                                x.PrptyName,
                                x.XMLTagName,
                                x.IsCompilerGeneratedAttribute,
                                HasValueFromXMLFile = x.HasDefaultValueFromXMLFile,
                                x.DefaultValueString,
                                CellDataNode = y
                            })
                        .ToList();


                    foreach (var pair in propertyNodePairs)
                    {
                        object value = null;
                        var strValue = pair.CellDataNode?.Attributes.OfType<XmlAttribute>()
                            .FirstOrDefault(x => "V".Equals(x.Name))?
                            .Value ?? pair.DefaultValueString;
                        if (!pair.HasValueFromXMLFile)
                        {
                            if (pair.Prpty.PropertyType == typeof(string)
                                && !pair.IsCompilerGeneratedAttribute
                                && pair.Prpty.GetValue(obj, null) == null)
                            {
                                value = string.Empty;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else if (pair.Prpty.PropertyType == typeof(string))
                        {
                            value = strValue;
                        }
                        else if (pair.Prpty.PropertyType == typeof(Guid))
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
                        else if (pair.Prpty.PropertyType == typeof(double))
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
                        else if (pair.Prpty.PropertyType == typeof(float))
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
                        else if (pair.Prpty.PropertyType == typeof(int))
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
                        else if (pair.Prpty.PropertyType == typeof(DateTime))
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
                        else if (pair.Prpty.PropertyType == typeof(long))
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
                        else if (pair.Prpty.PropertyType == typeof(short))
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
                        else if (pair.Prpty.PropertyType == typeof(bool))
                        {
                            value = strValue.Equals("T", StringComparison.CurrentCultureIgnoreCase);
                        }
                        else if (pair.Prpty.PropertyType == typeof(Bitmap))
                        {
                            if (pair.CellDataNode == null)
                            {
                                continue;
                            }
                            var array2 = Convert.FromBase64String(pair.CellDataNode.InnerText);
                            using (var stream2 = new MemoryStream())
                            {
                                try
                                {
                                    stream2.Write(array2, 0, array2.Length);
                                    value = (Bitmap)Image.FromStream(stream2);
                                }
                                catch (Exception)
                                {
                                    value = null;
                                }
                            }
                        }
                        else if (pair.Prpty.PropertyType == typeof(byte[]))
                        {
                            if (pair.CellDataNode == null)
                            {
                                continue;
                            }
                            value = Convert.FromBase64String(pair.CellDataNode.InnerText);
                        }
                        else if (pair.Prpty.PropertyType == typeof(Color))
                        {
                            int argb;
                            XmlAttribute colorNameAttribute;
                            Color colorFromColorName;
                            if (string.IsNullOrWhiteSpace(strValue) || !int.TryParse(strValue, out argb))
                            {
                                value = Color.Transparent;
                            }
                            else if ((colorNameAttribute = pair.CellDataNode?.Attributes?.OfType<XmlAttribute>()
                                                .FirstOrDefault(x => x.Name.Equals("ColorName"))) != null
                                    && (colorFromColorName = Color.FromName(colorNameAttribute.Value)).ToArgb() == argb)
                            {
                                value = colorFromColorName;
                            }
                            else
                            {
                                value = Color.FromArgb(argb);
                            }
                        }

                        if (value != null)
                        {
                            pair.Prpty.SetValue(obj, value, null);
                        }
                    }
                    curTable.AddNewRow(obj);
                }
                set.EnforceConstraints = true;
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        private static bool TryLoadFromXMLFileV1_V2(XmlElement node, out MCDataSet set)
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
                            if (pair.Prpty.PropertyType == typeof (string) 
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
                        else if (pair.Prpty.PropertyType == typeof (string))
                        {
                            value = strValue;
                        }
                        else if (pair.Prpty.PropertyType == typeof (Guid))
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
                        else if (pair.Prpty.PropertyType == typeof (double))
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
                        else if (pair.Prpty.PropertyType == typeof (float))
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
                        else if (pair.Prpty.PropertyType == typeof (int))
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
                        else if (pair.Prpty.PropertyType == typeof (DateTime))
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
                        else if (pair.Prpty.PropertyType == typeof (long))
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
                        else if (pair.Prpty.PropertyType == typeof (short))
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
                        else if (pair.Prpty.PropertyType == typeof (bool))
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
                        else if (pair.Prpty.PropertyType == typeof (Bitmap))
                        {
                            if (strValue == string.Empty)
                            {
                                continue;
                            }
                            var array2 = Convert.FromBase64String(strValue);
                            using (var stream2 = new MemoryStream())
                            {
                                try
                                {
                                    stream2.Write(array2, 0, array2.Length);
                                    value = (Bitmap) Image.FromStream(stream2);
                                }
                                catch (Exception)
                                {
                                    value = null;
                                }
                            }
                        }
                        else if (pair.Prpty.PropertyType == typeof (byte[]))
                        {
                            if (strValue == string.Empty)
                            {
                                continue;
                            }
                            value = Convert.FromBase64String(strValue);
                        }
                        else if (pair.Prpty.PropertyType == typeof (Color))
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
        
        public void LoadDataFromOtherConditional(XmlElement node)
        {
            if (node.Attributes.Count < 1 || !node.Attributes[0].Name.Equals("V"))
            {
                LoadDataFromOtherConditionalV1_V2(node);
                return;
            }

            var rowNodes = node.ChildNodes.OfType<XmlNode>().ToList();

            EnforceConstraints = false;

            var mcTables = Tables.OfType<TableBase>().ToList();
            var mcTablesWithLegacyName =mcTables.SelectMany(x => 
                                x.MemberType.GetCustomAttributes(typeof (LegacyClassName), true)
                                .OfType<LegacyClassName>()
                                .Select(legacyName => new { Table = x, TypeName = legacyName.Name}))
                .ToList();

            var curTableTag = string.Empty;
            TableBase curTable = null;
            List<PerpertyExt> curTablePrpties = null;
            foreach (var curRowNode in rowNodes)
            {
                if (!curRowNode.Name.Equals(curTableTag))
                {
                    //current row node is the first row of given table
                    curTableTag = curRowNode.Name;
                    var typeName = curRowNode.Attributes[0].Value;
                    var tableName = curRowNode.Attributes[1].Value;

                    curTable = mcTables.FirstOrDefault(
                                    x => x.TableName.Equals(tableName) && x.MemberType.FullName.Equals(typeName))
                               ?? mcTablesWithLegacyName.FirstOrDefault(
                                    x => x.Table.TableName.Equals(tableName) && x.TypeName.Equals(typeName))?.Table
                               ?? mcTables.FirstOrDefault(
                                    x => x.MemberType.FullName.Equals(typeName))
                               ?? mcTablesWithLegacyName.FirstOrDefault(
                                    x => x.Table.TableName.Equals(tableName) )?.Table;

                    curTable?.Rows.Clear();
                    
                    var properties = curTable?.MemberType.GetProperties().Where(x => x.CanWrite).ToList()??new List<PropertyInfo>();

                    curTablePrpties = properties
                      .Select(x => new { Order = 0, Prpty = x, PrptyName = x.Name, IsCompilerGeneratedAttribute = IsCompilerGeneratedAttribute(x) })
                      .Union(
                              //some of the property has different name when the xml file were created
                              //use LegacyPropertyName tag to match current property name and property name at the time
                              properties.SelectMany(x => x.GetCustomAttributes(typeof(LegacyPropertyName), true)
                                .OfType<LegacyPropertyName>()
                                .Select(legacyName => new { Order = 1, Prpty = x, PrptyName = legacyName.Name, IsCompilerGeneratedAttribute = IsCompilerGeneratedAttribute(x) })))
                      .LeftOuterJoin(
                          //get the list of properites when XML were created
                          //this list is stored in the first row of this given table in XML
                          curRowNode.ChildNodes.OfType<XmlElement>(),
                          x => x.PrptyName,
                          y => y.Attributes[0].Value,
                          (x, y) => new PerpertyExt
                          {
                              Prpty = x.Prpty,
                              PrptyName = x.PrptyName,
                              Order = x.Order,
                              XMLTagName = y?.Name ?? string.Empty,
                              IsCompilerGeneratedAttribute = x.IsCompilerGeneratedAttribute,
                              HasDefaultValueFromXMLFile = y != null,
                              DefaultValueString = y?.Attributes.OfType<XmlAttribute>().FirstOrDefault(attrbute => attrbute.Name.Equals("DV"))?.Value ?? string.Empty
                          })
                      .GroupBy(x => x.PrptyName)
                      //if the same property exist current propety name tag and legacy name tag in xml, use curent name instead of legacy name tag
                      //order by .Order will filter out
                      .Select(x => x.OrderBy(y => y.Order).First())
                      .ToList();

                }

                if (curTable == null)
                {
                    continue;
                }
                var obj = Activator.CreateInstance(curTable.MemberType);

                var propertyNodePairs = curTablePrpties
                    .LeftOuterJoin(curRowNode.ChildNodes.OfType<XmlNode>(), x => x.XMLTagName, y => y.Name,
                        (x, y) => new {
                            x.Prpty,
                            x.PrptyName,
                            x.XMLTagName,
                            x.IsCompilerGeneratedAttribute,
                            HasValueFromXMLFile = x.HasDefaultValueFromXMLFile,
                            x.DefaultValueString,
                            CellDataNode = y
                        })
                    .ToList();


                foreach (var pair in propertyNodePairs)
                {
                    object value = null;
                    var strValue = pair.CellDataNode?.Attributes.OfType<XmlAttribute>()
                        .FirstOrDefault(x => "V".Equals(x.Name))?
                        .Value ?? pair.DefaultValueString;
                    if (!pair.HasValueFromXMLFile)
                    {
                        if (pair.Prpty.PropertyType == typeof(string)
                            && !pair.IsCompilerGeneratedAttribute
                            && pair.Prpty.GetValue(obj, null) == null)
                        {
                            value = string.Empty;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (pair.Prpty.PropertyType == typeof(string))
                    {
                        value = strValue;
                    }
                    else if (pair.Prpty.PropertyType == typeof(Guid))
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
                    else if (pair.Prpty.PropertyType == typeof(double))
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
                    else if (pair.Prpty.PropertyType == typeof(float))
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
                    else if (pair.Prpty.PropertyType == typeof(int))
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
                    else if (pair.Prpty.PropertyType == typeof(DateTime))
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
                    else if (pair.Prpty.PropertyType == typeof(long))
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
                    else if (pair.Prpty.PropertyType == typeof(short))
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
                    else if (pair.Prpty.PropertyType == typeof(bool))
                    {
                        value = strValue.Equals("T", StringComparison.CurrentCultureIgnoreCase);
                    }
                    else if (pair.Prpty.PropertyType == typeof(Bitmap))
                    {
                        if (pair.CellDataNode == null)
                        {
                            continue;
                        }
                        var array2 = Convert.FromBase64String(pair.CellDataNode.InnerText);
                        using (var stream2 = new MemoryStream())
                        {
                            try
                            {
                                stream2.Write(array2, 0, array2.Length);
                                value = (Bitmap)Image.FromStream(stream2);
                            }
                            catch (Exception)
                            {
                                value = null;
                            }
                        }
                    }
                    else if (pair.Prpty.PropertyType == typeof(byte[]))
                    {
                        if (pair.CellDataNode == null)
                        {
                            continue;
                        }
                        value = Convert.FromBase64String(pair.CellDataNode.InnerText);
                    }
                    else if (pair.Prpty.PropertyType == typeof(Color))
                    {
                        int argb;
                        XmlAttribute colorNameAttribute;
                        Color colorFromColorName;
                        if (string.IsNullOrWhiteSpace(strValue) || !int.TryParse(strValue, out argb))
                        {
                            value = Color.Transparent;
                        }
                        else if ((colorNameAttribute = pair.CellDataNode?.Attributes?.OfType<XmlAttribute>()
                                            .FirstOrDefault(x => x.Name.Equals("ColorName"))) != null
                                && (colorFromColorName = Color.FromName(colorNameAttribute.Value)).ToArgb() == argb)
                        {
                            value = colorFromColorName;
                        }
                        else
                        {
                            value = Color.FromArgb(argb);
                        }
                    }

                    if (value != null)
                    {
                        pair.Prpty.SetValue(obj, value, null);
                    }
                }
                curTable.AddNewRow(obj);
            }
        }

        private void LoadDataFromOtherConditionalV1_V2(XmlElement node)
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
                            if (pair.Prpty.PropertyType == typeof(string)
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
                        else if (pair.Prpty.PropertyType == typeof(string))
                        {
                            value = strValue;
                        }
                        else if (pair.Prpty.PropertyType == typeof(Guid))
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
                        else if (pair.Prpty.PropertyType == typeof(double))
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
                        else if (pair.Prpty.PropertyType == typeof(float))
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
                        else if (pair.Prpty.PropertyType == typeof(int))
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
                        else if (pair.Prpty.PropertyType == typeof(DateTime))
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
                        else if (pair.Prpty.PropertyType == typeof(long))
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
                        else if (pair.Prpty.PropertyType == typeof(short))
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
                        else if (pair.Prpty.PropertyType == typeof(bool))
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
                        else if (pair.Prpty.PropertyType == typeof(Bitmap))
                        {
                            if (strValue == string.Empty)
                            {
                                continue;
                            }
                            var array2 = Convert.FromBase64String(strValue);
                            using (var stream2 = new MemoryStream())
                            {
                                try
                                {
                                    stream2.Write(array2, 0, array2.Length);
                                    value = (Bitmap)Image.FromStream(stream2);
                                }
                                catch (Exception)
                                {
                                    value = null;
                                }
                            }
                        }
                        else if (pair.Prpty.PropertyType == typeof(byte[]))
                        {
                            if (strValue == string.Empty)
                            {
                                continue;
                            }
                            value = Convert.FromBase64String(strValue);
                        }
                        else if (pair.Prpty.PropertyType == typeof(Color))
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

                        if (value != null)
                        {
                            pair.Prpty.SetValue(obj, value, null);
                        }
                    }
                    targetTbl.AddNewRow(obj);
                }
            }
            EnforceConstraints = true;
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
    }
    
}
