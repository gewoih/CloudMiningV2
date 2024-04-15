using CloudMining.Application.DTO.Payments;
using CloudMining.Application.Services.Payments;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.App.Controllers
{
	[Route("payment")]
	public class ShareablePaymentController : Controller
	{
		private readonly IShareablePaymentService _shareablePaymentService;

		public ShareablePaymentController(IShareablePaymentService shareablePaymentService)
		{
			_shareablePaymentService = shareablePaymentService;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateShareablePaymentDto paymentDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var payment = await _shareablePaymentService.CreateAsync(paymentDto);
			return View(payment);
		}
	}
}
