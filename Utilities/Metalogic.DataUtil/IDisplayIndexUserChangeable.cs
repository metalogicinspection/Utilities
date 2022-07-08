using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metalogic.DataUtil
{
    /// <summary>
    /// data class like Weld, user change change its display order in PDF
    /// implement this interface, system will adjust it for you properly
    /// </summary>s
    public interface IDisplayIndexUserChangeable
    {
        int Index { get; set; }
    }
}
