using System;

namespace Metalogic.UI.EditorsInGrid
{

    [Flags]
    public enum PicklistDisplayValueModes
    {
        //None = 0x000, // 000000000000
        Code = 0x001, // 000000000001
        ShortDescription = 0x002, // 000000000010
        Description = 0x004, // 000000000100
        CustomFields = 0x008, // 000000001000
        All = 0xFFF, // 111111111111
    }

}