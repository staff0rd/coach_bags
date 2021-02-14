using System;
using System.Threading.Tasks;
using coach_bags_selenium.Data;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.Chrome;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Logging;
using Amazon.S3;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Amazon;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using System.Collections.Generic;
using Npgsql;
using Serilog.Sinks.PostgreSQL;
using NpgsqlTypes;

[assembly: UserSecretsIdAttribute("35c1247a-0256-4d98-b811-eb58b6162fd7")]
namespace coach_bags_selenium
{

    [Subcommand(
        typeof(GetMetadataCommand),
        typeof(BackfillImagesCommand),
        typeof(ExportProductsCommand),
        typeof(ScrapeAndTweetCommand),
        typeof(TestAllCommand),
        typeof(TestMetadataCommand),
        typeof(TweetRandomProductCommand),
        typeof(GetTwitterImagesCommand),
        typeof(ScrapeCommand))]
    class Program
    {
        static Task Main(string[] args) => CreateHostBuilder().RunCommandLineApplicationAsync<Program>(args);
                
        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddTransient<ChromeDriver>(s => {
                            var headless = s.GetRequiredService<IConfiguration>().GetValue<bool>("Headless", true);
                            return ConfigureDriver(headless);
                        })
                        .AddMediatR(typeof(Program).Assembly)
                        .Configure<S3Options>(hostContext.Configuration.GetSection("S3"))
                        .Configure<TwitterOptions>(hostContext.Configuration.GetSection("Twitter"))
                        .AddTransient<IAmazonS3>(p => {
                            var options = p.GetService<IOptions<S3Options>>().Value;
                            var config = new AmazonS3Config
                            {
                                RegionEndpoint = RegionEndpoint.USWest2,
                            };
                            if (!string.IsNullOrWhiteSpace(options.Url)) // localstack
                            {
                                config.ForcePathStyle = true;
                                config.ServiceURL = options.Url;
                            }
                            return new AmazonS3Client(options.AccessKey, options.Secret, config);
                        })
                        .AddTransient<DataFactory>();
                        
                        var config = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .Enrich.FromLogContext()
                            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}");

                        WriteToPostgres(config, hostContext.Configuration);

                        Log.Logger = config.CreateLogger();
                })
                .ConfigureLogging((_, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddSerilog();
                });

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

        private Microsoft.Extensions.Configuration.IConfiguration _config;
        private ILogger<Program> _logger;
        private readonly IMediator _mediator;
        private readonly DataFactory _data;

        public Program(
            Microsoft.Extensions.Configuration.IConfiguration config,
            ILogger<Program> logger,
            IMediator mediator,
            DataFactory data)
        {
            _config = config;
            _logger = logger;
            _mediator = mediator;
            _data = data;
        }

        public async Task OnExecute()
        {
            var category = Enumeration.FromDisplayName<ProductCategory>(_config.GetValue<string>("Category"));
            var count = _config.GetValue<int>("Count");
            
            _data.GetDatabaseContext().Database.Migrate();

            await _mediator.Send(new ScrapeAndTweetCommand { Category = category, Count = count });
        }

        private static ChromeDriver ConfigureDriver(bool headless)
        {
            var options = new ChromeOptions();
            if (headless)
                options.AddArgument("headless");
            options.AddArgument("no-sandbox"); // needed to run inside container

            var driver = new ChromeDriver(options);
            driver.ExecuteChromeCommand("Network.setUserAgentOverride", new System.Collections.Generic.Dictionary<string, object> {
                { "userAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.53 Safari/537.36" }
            });
            driver.ExecuteChromeCommand("Page.addScriptToEvaluateOnNewDocument",
                new System.Collections.Generic.Dictionary<string, object> {
                    { "source", @"
                        Object.defineProperty(navigator, 'webdriver', {
                            get: () => undefined
                        })"
                    }
                 }
            );
            return driver;
        }
    }
}
