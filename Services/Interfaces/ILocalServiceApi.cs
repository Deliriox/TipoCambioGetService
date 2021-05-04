using System.Collections.Generic;
using System.Threading.Tasks;
using TipoCambioGetService.Services.Model;

namespace TipoCambioGetService.Services.Interfaces
{
    public interface ILocalServiceApi
    {
        Task<bool> CreateExchangeRate(IList<ExchangeRateModel> exchangeRateModelList);
    }
}
