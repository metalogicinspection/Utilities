using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Metalogic.DataUtil
{
    [Serializable]
    public abstract class PicklistItem : IConvertible, ISerializable, IEquatable<PicklistItem>, IComparable<PicklistItem>, IComparable
    {
        protected bool Equals(PicklistItem other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PicklistItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (_code != null ? _code.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_description != null ? _description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_shrtDsc != null ? _shrtDsc.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsEnabled.GetHashCode();
                return hashCode;
            }
        }

        private bool _codeIsSet;

        private string _code;
        public string Code
        {
            get
            {
                var t = GetType();
                if (!Items.ContainsKey(t))
                {
                    GetPickListByType(t);
                }
                return _code;
            }
            protected set
            {
                _code = value;
                _codeIsSet = true;
            }
        }

        /// <summary>
        /// Backup code for ToPickListItem(), this happened when a code is renamed
        /// </summary>
        public string BackupCode1 { get; protected set; }


        /// <summary>
        /// Backup code for ToPickListItem(), this happened when a code is renamed
        /// </summary>
        public string BackupCode2 { get; protected set; }


        /// <summary>
        /// Backup code for ToPickListItem(), this happened when a code is renamed
        /// </summary>
        public string BackupCode3 { get; protected set; }


        private string _description;
        public string Description
        {
            get { return string.IsNullOrEmpty(_description) ? ShortDescription : _description; }
            protected set { _description = string.IsNullOrEmpty(value) ? _description : value; }
        }

        private string _shrtDsc;
        public string ShortDescription
        {
            get { return string.IsNullOrEmpty(_shrtDsc) ? Code : _shrtDsc; }
            protected set { _shrtDsc = string.IsNullOrEmpty(value) ? _shrtDsc : value; }
        }

        public bool IsEnabled { get; protected set; }

        public override string ToString()
        {
            return Code;
        }

        protected PicklistItem()
        {
            _code = string.Empty;
            _codeIsSet = false;
            _description = string.Empty;
            ShortDescription = string.Empty;
            IsEnabled = true;
        }

        private static readonly Dictionary<Type, PicklistItem[]> Items = new Dictionary<Type, PicklistItem[]>();

        public static IEnumerable<T> GetPickList<T>() where T : PicklistItem
        {
            var t = typeof(T);

            var itms = GetPickListByType(t);

            return itms.Cast<T>().ToArray();
        }

        public static T GetPickListItem<T>(string code) where T : PicklistItem
        {
            return GetPickList<T>()
                .FirstOrDefault(x => x.Code == code) as T;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static PicklistItem GetPickListItem(Type t, string code)
        {
            var items = GetPickListByType(t);
            return items.FirstOrDefault(x => code.Equals(x.Code))
                   ?? items.FirstOrDefault(x => code.Equals(x.Code, StringComparison.OrdinalIgnoreCase))
                   ?? items.FirstOrDefault(x => code.Equals(x.BackupCode1, StringComparison.OrdinalIgnoreCase))
                   ?? items.FirstOrDefault(x => code.Equals(x.BackupCode2, StringComparison.OrdinalIgnoreCase))
                   ?? items.FirstOrDefault(x => code.Equals(x.BackupCode3, StringComparison.OrdinalIgnoreCase));}

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEnumerable<PicklistItem> GetPickListByType(Type t)
        {
            if (t == null || !typeof(PicklistItem).IsAssignableFrom(t))
            {
                throw new Exception("Given Type is not a PickListItem");
            }

            if (Items.ContainsKey(t))
            {
                return Items[t];
            }

            var fieldInfos = t.GetFields().Where(x => x.IsStatic && x.FieldType == t);
            var lst = new List<PicklistItem>();
            foreach (var field in fieldInfos)
            {
                var itm = field.GetValue(null) as PicklistItem;
                if (!itm._codeIsSet)
                {
                    itm.Code = field.Name;
                }
                lst.Add(itm);
            }


            var retValue = lst.ToArray();
            Items[t] = retValue;
            return retValue;
        }
        public static TableBase GetPickListTable<T>() where T : PicklistItem
        {
            var itms = GetPickList<T>();
            var t = new TableBase();
            foreach (var item in itms)
            {
                t.AddNewRow(item);
            }

            return t;
        }

        public static bool operator ==(PicklistItem a, PicklistItem b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Code == b.Code;
        }

        public static bool operator !=(PicklistItem a, PicklistItem b)
        {
            return !(a == b);
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.String;}

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            return Code;
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return this;}

        protected PicklistItem(SerializationInfo info, StreamingContext context)
        {
            string code = string.Empty;
            try
            {
                code = info.GetString("0");
            }
            catch (Exception)
            {
                code = info.GetString("PicklistItem+_code");
            }
            var ptskItem = PicklistItem.GetPickListItem(GetType(), code);
            if (ptskItem == null)
            {
                throw new Exception(string.Concat("PicklistItem ", code, " of type ", GetType().FullName, " not exist!"));
            }

            this.LoadFieldsFromOther(ptskItem);
            Code = ptskItem.Code;
            BackupCode1 = ptskItem.BackupCode1;
            BackupCode2 = ptskItem.BackupCode2;
            BackupCode3 = ptskItem.BackupCode3;
            ShortDescription = ptskItem.ShortDescription;
            Description = ptskItem.Description;
            IsEnabled = ptskItem.IsEnabled;
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("0", Code);
        }

        bool IEquatable<PicklistItem>.Equals(PicklistItem other)
        {
            return this == other;
        }

        public int CompareTo(PicklistItem other)
        {
            return Code.CompareTo(other.Code);
        }

        public int CompareTo(object obj)
        {
            return Code.CompareTo(obj?.ToString() ?? string.Empty);
        }
    }

}
