using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Metalogic.DataUtil
{
    public class Serializer
    {
        public static byte[] Serialize(object data)
        {
            byte[] retValue;
            using (var stream = new MemoryStream()){
                var bFormatter = new BinaryFormatter();
                bFormatter.Serialize(stream, data);
                stream.Close();
                retValue = stream.ToArray();
                stream.Dispose();
            }

            return retValue;
        }

        public static T Deserialize<T>(byte[] data)
        {
            T retValue;
            using (var stream = new MemoryStream(data))
            {
                var bFormatter = new BinaryFormatter();
                retValue = (T)bFormatter.Deserialize(stream);
                stream.Close();
                stream.Dispose();
            }

            return retValue;
        }

        public static object Deserialize(byte[] data)
        {
            object retValue;
            using (var stream = new MemoryStream(data))
            {
                var bFormatter = new BinaryFormatter();
                retValue = bFormatter.Deserialize(stream);
                stream.Close();
                stream.Dispose();
            }

            return retValue;
        }
    }
}
