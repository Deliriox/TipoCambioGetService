using Microsoft.Extensions.Configuration;

namespace TipoCambioGetService
{
    public class App
    {
        private readonly IConfigurationRoot _config;

        public App(IConfigurationRoot config)
        {
            _config = config;
        }
    }
}
