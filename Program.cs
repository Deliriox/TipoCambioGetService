using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Timers;
using System.Threading.Tasks;
using System.Xml;
using TipoCambioGetService.Services;
using TipoCambioGetService.Services.Interfaces;
using System.Threading;
using System.Windows.Input;

namespace TipoCambioGetService
{
    class Program
    {
        public static IConfigurationRoot configuration;

        static int Main(string[] args)
        {

            try
            {
                // Start!
                MainAsync(args).Wait();
                return 0;
            }
            catch
            {
                return 1;
            }
        }

        static async Task MainAsync(string[] args)
        {
            // Create service collection
            Log.Information("Creating service collection");
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Create service provider
            Log.Information("Building service provider");
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            // Get connections from appsettings.json
            string banguatConnection = configuration.GetConnectionString("BanguatConnection");
            string localServiceConnection = configuration.GetConnectionString("LocalServiceConnection");

            TimeSpan startTime = TimeSpan.Zero;
            TimeSpan periodTime = TimeSpan.FromSeconds(30);            

            try
            {
                Console.WriteLine("The services have been started");
                Console.WriteLine("Press any keyboard to stop the service");
                ITipoCambioDiaApi tipoCambioDiaApi = new TipoCambioDiaApi(banguatConnection);
                ILocalServiceApi localServiceApi = new LocalServiceApi(localServiceConnection);
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                CancellationToken cancellationToken = cancellationTokenSource.Token;
                while (!cancellationToken.IsCancellationRequested)
                {
                    var test2 = Console.KeyAvailable;
                    if (test2)
                    {
                        cancellationTokenSource.Cancel();
                        break;
                    }
                    var ouput = await tipoCambioDiaApi.GetTipoCambioDia();
                    var test = await localServiceApi.CreateExchangeRate(ouput);
                    await Task.Delay(30000, cancellationToken);                    
                }

                

                Console.WriteLine("End of the service");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error running service: " + ex.ToString());
                throw ex;
            }
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging();

            // Build configuration
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            // Add access to generic IConfigurationRoot
            serviceCollection.AddSingleton<IConfigurationRoot>(configuration);

            // Add app
            serviceCollection.AddTransient<App>();
        }
    }
}