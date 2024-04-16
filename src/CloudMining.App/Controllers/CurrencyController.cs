using CloudMining.Application.DTO.Currencies;
using CloudMining.Application.Services.Currencies;
using CloudMining.Infrastructure.Emcd.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.App.Controllers
{
    [Route("[controller]")]
    public class CurrencyController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly EmcdApiClient _emcdApiClient;

        public CurrencyController(ICurrencyService currencyService, EmcdApiClient emcdApiClient)
        {
	        _currencyService = currencyService;
	        _emcdApiClient = emcdApiClient;
        }

        [HttpPost]
        public async Task<IActionResult> AddCurrency([FromBody] CurrencyDto currency)
        {
            await _currencyService.CreateAsync(currency);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetPayouts()
        {
	        var result = await _emcdApiClient.GetPayouts();
	        return Json(result);
        }
    }
}
