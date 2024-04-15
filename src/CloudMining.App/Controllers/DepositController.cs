using CloudMining.Application.DTO.Payments.Deposits;
using CloudMining.Application.Services.Deposits;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.App.Controllers
{
	[Route("[controller]")]
	public class DepositController : Controller
	{
		private readonly IDepositService _depositService;

		public DepositController(IDepositService depositService)
		{
			_depositService = depositService;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateDepositDto depositDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var newDeposit = await _depositService.AddDepositAndRecalculateShares(depositDto);
			return View(newDeposit);
		}
	}
}
