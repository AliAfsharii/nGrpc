using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace nGrpc.ServerCommon
{
    public static class Extensions
    {
        public static void AddConfig<TImplementation>(this IServiceCollection services, IConfiguration configuration, string section)
        where TImplementation : class, new()
        {
            var configPoco = new TImplementation();
            configuration.Bind(section, configPoco);
            services.AddSingleton(configPoco);
        }

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

        public static T CloneByMessagePack<T>(this T obj) where T : class
        {
            byte[] b = MessagePackSerializer.Typeless.Serialize(obj);
            T o = (T)MessagePackSerializer.Typeless.Deserialize(b);
            return o;
        }
    }
}
