using CloudMining.Application.DTO.Payments.Electricity;
using CloudMining.Application.Services.Payments.Electricity;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.App.Controllers
{
	[Route("electricity")]
	public class ElectricityPaymentController : Controller
	{
		private readonly IElectricityPaymentService _electricityPaymentService;

		public ElectricityPaymentController(IElectricityPaymentService electricityPaymentService)
		{
			_electricityPaymentService = electricityPaymentService;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateElectricityPaymentDto electricityPaymentDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var payment = await _electricityPaymentService.CreateAsync(electricityPaymentDto);
			return View(payment);
		}
	}
}
