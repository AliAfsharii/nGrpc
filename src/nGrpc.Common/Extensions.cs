using Newtonsoft.Json;

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
    }
}
