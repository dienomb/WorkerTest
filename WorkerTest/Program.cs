using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkerTest.Configuration;
using WorkerTest.Services;

namespace WorkerTest
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureLogging((hostContext, config) =>
                {
                    config.AddConsole();
                    config.AddDebug();
                })
                .ConfigureHostConfiguration(config =>
                {
                    config.AddEnvironmentVariables();
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                    config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    //services.AddSingleton<MonitorLoop>();

                    #region snippet1
                    services.AddHostedService<ChannelSample>();
                    //services.AddHostedService<TimedHostedService>();
                    #endregion

                    #region snippet2
                    //services.AddHostedService<ConsumeScopedServiceHostedService>();
                    //services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
                    #endregion

                    #region snippet3
                    //services.AddHostedService<QueuedHostedService>();
                    //services.AddSingleton<TestService>();
                    //services.AddSingleton<TestService2>();
                    //services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
                    //services.AddSingleton<Func<string, ITestService>>(serviceProvider => key =>
                    //{
                    //    switch (key)
                    //    {
                    //        case "Servis1":
                    //            return serviceProvider.GetRequiredService<TestService>();
                    //        default:
                    //            return serviceProvider.GetRequiredService<TestService2>();
                    //    }
                    //});
                    #endregion
                })
                .UseConsoleLifetime()
                .Build();

            using (host)
            {
                // Start the host
                await host.StartAsync();

                //// Monitor for new background queue work items
                //var monitorLoop = host.Services.GetRequiredService<MonitorLoop>();
                //monitorLoop.StartMonitorLoop();

                // Wait for the host to shutdown
                await host.WaitForShutdownAsync();
            }
        }
    }
}
