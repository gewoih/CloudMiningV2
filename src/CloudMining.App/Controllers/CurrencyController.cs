using CloudMining.Application.Models.Currencies;
using CloudMining.Application.Services.Currencies;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.App.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        [HttpPost]
        public async Task<IActionResult> AddCurrency([FromBody]CurrencyDTO currency)
        {
            await _currencyService.CreateAsync(currency);
            return Ok();
        }
    }
}
