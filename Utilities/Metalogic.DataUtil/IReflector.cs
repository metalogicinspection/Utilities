using System;
namespace Metalogic.DataUtil
{
    public interface IReflector
    {
        Type FindType(string className);
        string FindClassName(Type type);
    }
}
