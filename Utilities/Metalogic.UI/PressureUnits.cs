using Metalogic.DataUtil;
using System;
using System.Runtime.Serialization;

namespace Metalogic.UI.Editors
{
    [Serializable]
    public class PressureUnits : PicklistItem
    {
        public PressureUnits()
        {

        }

        public PressureUnits(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public string Mask { get; set; }

        public double ConvertTo(double srcValue, PressureUnits target)
        {
            if (target == null)
            {
                throw new Exception("Pressure unit is null");
            }
            return srcValue * GetConversionFactor(target);
        }

        public double GetConversionFactor(PressureUnits target)
        {
            if (this == MPa)
            {
                if (target == kPa)
                {
                    return 1000d;
                }
                if (target == psi)
                {
                    return 145.038d;
                }
            }
            if (this == kPa)
            {
                if (target == MPa)
                {
                    return 0.001d;
                }
                if (target == psi)
                {
                    return 0.145038d;
                }
            }
            if (this == psi)
            {
                if (target == MPa)
                {
                    return 0.00689476d;
                }
                if (target == kPa)
                {
                    return 6.89476d;
                }
            }
            return 1d;
        }

        public string ToStringMask { get; set; }

        public string UnitDisplayShortSymbol { get; set; }

        public static PressureUnits MPa = new PressureUnits { Mask = "f0", ToStringMask = "#.#", UnitDisplayShortSymbol = "MPa" };
        public static PressureUnits kPa = new PressureUnits { Mask = "f0", ToStringMask = "#.#", UnitDisplayShortSymbol = "kPa" };
        public static PressureUnits psi = new PressureUnits { Mask = "f0", ToStringMask = "#.#", UnitDisplayShortSymbol = "psi" };
    }
}