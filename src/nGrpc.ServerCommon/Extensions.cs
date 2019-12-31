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
    }
}
