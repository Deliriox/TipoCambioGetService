using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;
using TipoCambioGetService.Services;
using TipoCambioGetService.Services.Interfaces;
using System.Threading;

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
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Get connections from appsettings.json
            string banguatConnection = configuration.GetConnectionString("BanguatConnection");
            string localServiceConnection = configuration.GetConnectionString("LocalServiceConnection");     

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
                    var key = Console.KeyAvailable;
                    if (key)
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