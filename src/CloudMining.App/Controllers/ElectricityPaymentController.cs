using CloudMining.Application.Services.Payments.Electricity;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.App.Controllers
{
	public class ElectricityPaymentController : Controller
	{
		private readonly IElectricityPaymentService _electricityPaymentService;

		public ElectricityPaymentController(IElectricityPaymentService electricityPaymentService)
		{
			_electricityPaymentService = electricityPaymentService;
		}

		[HttpPost]
		public async Task<IActionResult> Create()
		{

		}
	}
}
