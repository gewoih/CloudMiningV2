using CloudMining.Application.DTO.Currencies;
using CloudMining.Application.Services.Currencies;
using CloudMining.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class CurrenciesController : ControllerBase
	{
		private readonly ICurrencyService _currencyService;

		public CurrenciesController(ICurrencyService currencyService)
		{
			_currencyService = currencyService;
		}

		[HttpPost]
		public async Task<Currency> Create([FromBody] CurrencyDto currency)
		{
			var createdCurrency = await _currencyService.CreateAsync(currency);
			return createdCurrency;
		}
	}
}
