using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpgsqlTypes;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace nGrpc.Worker.StartupUtils
{
    public static class SerilogUtils
    {
        private class WriteUtcTime : TimestampColumnWriter
        {
            readonly string _propertyName;

            public WriteUtcTime(string propertyName)
            {
                _propertyName = propertyName;
            }

            public override object GetValue(LogEvent logEvent, IFormatProvider formatProvider = null)
            {
                var scalerValue = logEvent.Properties[_propertyName];
                return (scalerValue as ScalarValue).Value;
            }
        }

        private class UtcTimestampEnricher : ILogEventEnricher
        {
            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                logEvent.AddPropertyIfAbsent(
                    propertyFactory.CreateProperty(
                        "UtcTimestamp",
                        logEvent.Timestamp.UtcDateTime));
            }
        }

        private static LogEventLevel ParseLogEventLevel(string value)
        {
            if (!Enum.TryParse(value, out LogEventLevel parsedLevel))
                throw new InvalidOperationException($"The value {value} is not a valid Serilog level.");
            return parsedLevel;
        }

        public static void AddSerilog(string connectionString, LogLevelConfigs logLevelConfig)
        {
            IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
            {
                {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
                {"message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
                {"level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
                {"raise_date", new WriteUtcTime("UtcTimestamp") },
                {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
                {"properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
                {"props_test", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
                {"machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") }
            };

            LogEventLevel console = ParseLogEventLevel(logLevelConfig.Console);
            LogEventLevel microsoft = ParseLogEventLevel(logLevelConfig.Microsoft);
            LogEventLevel database = ParseLogEventLevel(logLevelConfig.Database);
            LoggingLevelSwitch levelSwitch = new LoggingLevelSwitch(console);

            Log.Logger = new LoggerConfiguration()
           .MinimumLevel.ControlledBy(levelSwitch)
           .MinimumLevel.Override("Microsoft", microsoft)
           .Enrich.With(new UtcTimestampEnricher())
           .WriteTo.Console()
           .WriteTo.PostgreSQL(connectionString, "serverLogs", columnOptions: columnWriters, restrictedToMinimumLevel: database)
           .CreateLogger();
        }
    }
}
