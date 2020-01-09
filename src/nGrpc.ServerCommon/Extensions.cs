using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
