using CloudMining.Application.DTO.Currencies;
using CloudMining.Application.Services.Currencies;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.App.Controllers
{
    [Route("[controller]")]
    public class CurrencyController : Controller
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCurrency([FromBody] CurrencyDto currency)
        {
            await _currencyService.CreateAsync(currency);
            return Ok();
        }
    }
}
