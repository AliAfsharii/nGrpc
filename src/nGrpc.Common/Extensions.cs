using Newtonsoft.Json;
using MessagePack;

namespace nGrpc.Common
{
    public static class Extensions
    {
        public static string ToJson(this object obj)
        {
            if (obj == null)
                return null;

            return JsonConvert.SerializeObject(obj);
        }

        public static T ToObject<T>(this string json)
        {
            if (json == null)
                return default;

            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T CloneByMessagePack<T>(this T obj)
        {
            byte[] b = MessagePackSerializer.Typeless.Serialize(obj);
            T o = (T)MessagePackSerializer.Typeless.Deserialize(b);
            return o;
        }

        public static byte[] ToBytes<T>(this T obj)
        {
            byte[] b = MessagePackSerializer.Typeless.Serialize(obj);
            return b;
        }

        public static T ToObject<T>(this byte[] bytes)
        {
            if (bytes == null)
                return default;

            T o = (T)MessagePackSerializer.Typeless.Deserialize(bytes);
            return o;
        }
    }
}
