using Metalogic.DataUtil;
using System;
using System.Runtime.Serialization;

namespace Metalogic.UI.Editors
{
    [Serializable]
    public class LengthUnits : PicklistItem
    {
        public LengthUnits()
        {

        }

        public LengthUnits(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public string Mask { get; set; }

        public double ConvertTo(double srcValue, LengthUnits target)
        {
            if (target == null)
            {
                throw new Exception("Length unit is null");
            }
            return srcValue * GetConversionFactor(target);
        }

        public double GetConversionFactor(LengthUnits target)
        {
            if (this == M)
            {
                if (target == CM)
                {
                    return 100d;
                }
                if (target == MM)
                {
                    return 1000d;
                }
                if (target == Inch)
                {
                    return 39.3701d;
                }
                if (target == KM)
                {
                    return 0.001d;
                }
                if (target == Mi)
                {
                    return 0.000621371d;
                }
                if (target == Feet)
                {
                    return 3.28084d;
                }
            }
            if (this == CM)
            {
                if (target == M)
                {
                    return 0.01d;
                }
                if (target == MM)
                {
                    return 10d;
                }
                if (target == Inch)
                {
                    return 0.393701d;
                }
                if (target == KM)
                {
                    return 0.00001d;
                }
                if (target == Mi)
                {
                    return 6.21371e-6d;
                }
                if (target == Feet)
                {
                    return 0.0328084d;
                }
            }

            if (this == MM)
            {
                if (target == M)
                {
                    return 0.001d;
                }
                if (target == CM)
                {
                    return 0.1d;
                }
                if (target == Inch)
                {
                    return 0.0393701d;
                }
                if (target == KM)
                {
                    return 1e-6d;
                }
                if (target == Mi)
                {
                    return 6.21371e-7d;
                }
                if (target == Feet)
                {
                    return 0.00328084d;
                }
            }

            if (this == Inch)
            {
                if (target == M)
                {
                    return 0.0254d;
                }
                if (target == CM)
                {
                    return 2.54d;
                }
                if (target == MM)
                {
                    return 25.4d;
                }
                if (target == KM)
                {
                    return 2.54e-5d;
                }
                if (target == Mi)
                {
                    return 1.57828e-5d;
                }
                if (target == Feet)
                {
                    return 0.0833333d;
                }
            }
            if (this == KM)
            {
                if (target == M)
                {
                    return 1000d;
                }
                if (target == CM)
                {
                    return 100000d;
                }
                if (target == Inch)
                {
                    return 39370.1d;
                }
                if (target == MM)
                {
                    return 1000000d;
                }
                if (target == Mi)
                {
                    return 0.621371d;
                }
                if (target == Feet)
                {
                    return 3280.84d;
                }
            }
            if (this == Mi)
            {
                if (target == M)
                {
                    return 1609.34d;
                }
                if (target == CM)
                {
                    return 160934d;
                }
                if (target == Inch)
                {
                    return 63360d;
                }
                if (target == MM)
                {
                    return 1.609e+6;
                }
                if (target == KM)
                {
                    return 1.60934d;
                }
                if (target == Feet)
                {
                    return 5280d;
                }
            }
            if (this == Feet)
            {
                if (target == M)
                {
                    return 0.3048d;
                }
                if (target == CM)
                {
                    return 30.48d;
                }
                if (target == Inch)
                {
                    return 12d;
                }
                if (target == MM)
                {
                    return 304.8d;
                }
                if (target == Mi)
                {
                    return 0.000189394d;
                }
            }
            return 1d;
        }

        public string ToStringMask { get; set; }

        public string UnitDisplayShortSymbol { get; set; }

        public static LengthUnits M = new LengthUnits { Mask = "f3", ToStringMask = "0.###", UnitDisplayShortSymbol = "m" };
        public static LengthUnits CM = new LengthUnits { Mask = "f2", ToStringMask = "0.##", UnitDisplayShortSymbol = "cm" };
        public static LengthUnits Inch = new LengthUnits { Mask = "f3", ToStringMask = "0.###", BackupCode1 = "Inche", UnitDisplayShortSymbol = "\"" };
        public static LengthUnits MM = new LengthUnits { Mask = "f2", ToStringMask = "0.##", UnitDisplayShortSymbol = "mm" };
        public static LengthUnits KM = new LengthUnits { Mask = "f2", ToStringMask = "0.##", UnitDisplayShortSymbol = "km" };
        public static LengthUnits Mi = new LengthUnits { Mask = "f2", ToStringMask = "0.##", UnitDisplayShortSymbol = "mi" };
        public static LengthUnits Feet = new LengthUnits { Mask = "f2", ToStringMask = "0.##", UnitDisplayShortSymbol = "ft" };
    }
}