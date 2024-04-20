using CloudMining.Application.DTO.Payments;
using CloudMining.Application.Services.Payments;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentsController : ControllerBase
	{
		private readonly IShareablePaymentService _shareablePaymentService;

		public PaymentsController(IShareablePaymentService shareablePaymentService)
		{
			_shareablePaymentService = shareablePaymentService;
		}
		
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateShareablePaymentDto paymentDto)
		{
			_ = await _shareablePaymentService.CreateAsync(paymentDto);
			return Ok();
		}
	}
}
