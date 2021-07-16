using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Metalogic.DataUtil
{
    public class ReflectionUtilFunctions
    {
        private static readonly Hashtable SGetTypeBuffer = new Hashtable();


        [MethodImpl(MethodImplOptions.Synchronized)]
        private static Type GetTypeFromCache(string name)
        {
            if (SGetTypeBuffer.ContainsKey(name))
            {
                return (Type)SGetTypeBuffer[name];
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void SetTypeToCache(string name, Type typ)
        {
            SGetTypeBuffer[name] = typ;
        }

        public static Type GetType(string name)
        {
            var typ = GetTypeFromCache(name);
            if (typ != null)
            {
                return typ;
            }

            typ = _reflector != null ? _reflector.FindType(name) : FindType(name);
            SetTypeToCache(name, typ);
            return typ;
        }

        private static IReflector _reflector;

        public static void SetReflector(IReflector reflector)
        {
            _reflector = reflector;
        }

        private static Type FindType(string name)
        {
            var retType = Type.GetType(name);
            // tryu the lazy way first
            if (retType != null)
            {
                return retType;
            }

            foreach (var a in AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.GetName().Name.Equals("System.Data" ) && x.GetName().Name.EndsWith(".Data")))
            {
                try
                {
                    retType = a.GetType(name) ??
                              a.GetExportedTypes()
                               .FirstOrDefault(x => x.GetCustomAttributes(typeof(LegacyClassName), true)
                                                 .OfType<LegacyClassName>()
                                                 .Any(y => name.Equals(y.Name)));
                }
                catch (Exception)
                {
                    retType = null;
                }

                if (retType != null)
                    return retType;
            }

            foreach (var a in AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.GetName().Name.EndsWith(".Data")))
            {
                try
                {
                    retType = a.GetType(name) ??
                              a.GetExportedTypes()
                               .FirstOrDefault(x => x.GetCustomAttributes(typeof(LegacyClassName), true)
                                                 .OfType<LegacyClassName>()
                                                 .Any(y => name.Equals(y.Name)));
                }
                catch (Exception)
                {
                    retType = null;
                }

                if (retType != null)
                    return retType;
            }



            var rootDic = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            var q = new Queue<DirectoryInfo>();
            q.Enqueue(rootDic);
            while (q.Count > 0)
            {
                var curDic = q.Dequeue();
                foreach (var f in curDic.GetFiles().Where(x => x.Name.ToUpper().EndsWith(".DLL")))
                {
                    try
                    {
                        retType = Assembly.LoadFile(f.FullName).GetType(name);
                    }
                    catch (Exception e)
                    {
                        retType = null;
                    }

                    if (retType != null)
                    {
                        q.Clear();
                        return retType;
                    }
                }

                foreach (var curChildDic in curDic.GetDirectories())
                {
                    q.Enqueue(curChildDic);
                }
            }


          

            return null;
        }

  }
}
