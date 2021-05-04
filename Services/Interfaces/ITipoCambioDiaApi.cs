
using System.Collections.Generic;
using System.Threading.Tasks;
using TipoCambioDiaService;
using TipoCambioGetService.Services.Model;

namespace TipoCambioGetService.Services.Interfaces
{
    public interface ITipoCambioDiaApi
    {
        Task<TipoCambioSoapClient> GetInstanceAsync();
        Task<IList<ExchangeRateModel>> GetTipoCambioDia();
    }
}
