using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Serilog;
using Serilog.Events;
using System.Collections.Generic;
using Serilog.Sinks.PostgreSQL;
using NpgsqlTypes;

namespace coach_bags_selenium
{
    public class LogHelper 
    {
        public static void Configure(IConfiguration appConfig, string categoryName = null)
        {
            var config = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Category", categoryName ?? appConfig.GetValue<string>("Category"))
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}");

            WriteToPostgres(config, appConfig);

            Log.Logger = config.CreateLogger();
        }

        private static void WriteToPostgres(LoggerConfiguration logConfig, IConfiguration appConfig)
        {
            var connectionString = appConfig.GetConnectionString("Postgres");
            
            string tableName = "logs";
            IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
            {
                { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
                { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
                { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
                { "created", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
                { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
                { "properties", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
            };

            logConfig.WriteTo
                .PostgreSQL(connectionString, tableName, columnWriters, failureCallback: (e) =>
                {
                    Console.WriteLine("Could not log to postgres" + e.Message);
                }, needAutoCreateTable: true, needAutoCreateSchema: true);
        }
    }
}
