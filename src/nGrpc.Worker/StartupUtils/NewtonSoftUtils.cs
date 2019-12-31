using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace nGrpc.Worker.StartupUtils
{
    public static class NewtonSoftUtils
    {
        public static void ConfigNewtonToSerializeEnumToString()
        {
            JsonConvert.DefaultSettings = (() =>
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });
                return settings;
            });
        }
    }
}
