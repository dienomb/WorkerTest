using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkerTest.Configuration;

namespace WorkerTest
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureLogging(loggerFactory => loggerFactory.AddDebug())
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<BackgroundTaskSettings>(GetConfiguration().Build());
                    services.AddHostedService<Worker>();
                }
                )
                .Build();
            await host.RunAsync();
        }

        private static IConfigurationBuilder GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder;
        }
    }
}
