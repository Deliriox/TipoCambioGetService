using System;

namespace TipoCambioGetService.Services.Model
{
    public class ExchangeRateModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Rate { get; set; }
        public DateTime WhenObtained { get; set; }
    }
}
