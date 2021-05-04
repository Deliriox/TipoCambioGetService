using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using TipoCambioDiaService;
using TipoCambioGetService.Services.Interfaces;
using TipoCambioGetService.Services.Model;

namespace TipoCambioGetService.Services
{
    public class TipoCambioDiaApi : ITipoCambioDiaApi
    {
        public readonly string serviceUrl;
        public readonly EndpointAddress endpointAddress;
        public readonly BasicHttpBinding basicHttpBinding;
        public TipoCambioDiaApi(string connection)
        {
            serviceUrl = connection;
            endpointAddress = new (serviceUrl);

            basicHttpBinding =
                new BasicHttpBinding(endpointAddress.Uri.Scheme.ToLower() == "http" ?
                            BasicHttpSecurityMode.None : BasicHttpSecurityMode.Transport);

            //Please set the time accordingly, this is only for demo
            basicHttpBinding.OpenTimeout = TimeSpan.MaxValue;
            basicHttpBinding.CloseTimeout = TimeSpan.MaxValue;
            basicHttpBinding.ReceiveTimeout = TimeSpan.MaxValue;
            basicHttpBinding.SendTimeout = TimeSpan.MaxValue;
        }

        public async Task<TipoCambioSoapClient> GetInstanceAsync()
        {
            return await Task.Run(() => new TipoCambioSoapClient(basicHttpBinding, endpointAddress));
        }

        public async Task<IList<ExchangeRateModel>> GetTipoCambioDia()
        {
            var client = await GetInstanceAsync();
            var response = await client.TipoCambioDiaAsync();
            if(response.Body.TipoCambioDiaResult.CambioDolar != null)
            {
                IList<ExchangeRateModel> exchangeRateModelList = new List<ExchangeRateModel>();
                foreach(var item in response.Body.TipoCambioDiaResult.CambioDolar)
                {
                    ExchangeRateModel temporalModel = new ExchangeRateModel();
                    temporalModel.Id = 0;
                    temporalModel.Date = Convert.ToDateTime(item.fecha);
                    temporalModel.Rate = item.referencia;
                    temporalModel.WhenObtained = DateTime.Now;
                    exchangeRateModelList.Add(temporalModel);
                }
                return exchangeRateModelList;
            }
            return null;
        }
    }
}
