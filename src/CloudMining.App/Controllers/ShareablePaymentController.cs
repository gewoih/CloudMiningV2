using CloudMining.Application.DTO.Payments;
using CloudMining.Application.Services.Payments;
using CloudMining.Application.Services.Users;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;
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

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet("/api/payments")]
		public async Task<List<ShareablePayment>> Get(PaymentType paymentType)
		{
			var currentUserId = _userService.GetCurrentUserId();
			var payments = await _shareablePaymentService.GetAsync(paymentType, currentUserId);

			return payments;
		}

		[HttpPost("/api/payments")]
		public async Task<IActionResult> Create(CreateShareablePaymentDto paymentDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			_ = await _shareablePaymentService.CreateAsync(paymentDto);
			return RedirectToAction("Index");
		}
	}
}
