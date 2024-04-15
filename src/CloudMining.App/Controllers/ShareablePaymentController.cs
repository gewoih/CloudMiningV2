using CloudMining.Application.DTO.Payments;
using CloudMining.Application.Services.Payments;
using CloudMining.Application.Services.Users;
using CloudMining.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.App.Controllers
{
	[Route("payments")]
	public class ShareablePaymentController : Controller
	{
		private readonly IUserService _userService;
		private readonly IShareablePaymentService _shareablePaymentService;
		
		public ShareablePaymentController(IShareablePaymentService shareablePaymentService, IUserService userService)
		{
			_shareablePaymentService = shareablePaymentService;
			_userService = userService;
		}

		public async Task<IActionResult> Index(PaymentType paymentType = PaymentType.Electricity)
		{
			var currentUserId = _userService.GetCurrentUserId(); 
			var payments = await _shareablePaymentService.GetAsync(paymentType, currentUserId);

			return View(payments);
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateShareablePaymentDto paymentDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var payment = await _shareablePaymentService.CreateAsync(paymentDto);
			return View(payment);
		}
	}
}
