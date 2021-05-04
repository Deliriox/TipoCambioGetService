using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TipoCambioGetService.Services.Interfaces;
using TipoCambioGetService.Services.Model;

namespace TipoCambioGetService.Services
{
    public class LocalServiceApi : ILocalServiceApi
    {
        public readonly string serviceUrl;
        public LocalServiceApi(string connection)
        {
            serviceUrl = connection;
        }

        public async Task<bool> CreateExchangeRate(IList<ExchangeRateModel> exchangeRateModelList)
        {
            var exchangeRate = JsonConvert.SerializeObject(exchangeRateModelList);
            exchangeRate = exchangeRate.ToString();
            
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(serviceUrl);
            var response = await client.PostAsync("api/ExchangeRate", new StringContent(exchangeRate, Encoding.UTF8, "application/json"));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("The latest 'exchange rate' had been inserted into the data base.");
                return true;
            }
            return false;
        }
    }
}
