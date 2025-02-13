using Elastic.Serilog.Sinks;
using Microsoft.Extensions.Configuration;
using Serilog;


namespace CommonPractices.SerilogSett
{
    public static class SerilogSettings
    {
        public static void configureLogging(string url)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", true).Build();

            Serilog.Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new[] { new Uri(url) },
                      options =>
                      {
                          options.TextFormatting = new Elastic.CommonSchema.Serilog.EcsTextFormatterConfiguration<Elastic.CommonSchema.EcsDocument>();
                          options.BootstrapMethod = new Elastic.Ingest.Elasticsearch.BootstrapMethod();
                          options.ConfigureChannel = channelOptions =>
                          {
                              channelOptions.BufferOptions = new Elastic.Channels.BufferOptions();
                          };
                      })
                .Enrich.WithProperty("Environment", environment)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

    }
}
