using CloudMining.Application.DTO.Payments;
using CloudMining.Application.Services.Payments;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class PaymentsController : ControllerBase
	{
		private readonly IShareablePaymentService _shareablePaymentService;

		public PaymentsController(IShareablePaymentService shareablePaymentService)
		{
			_shareablePaymentService = shareablePaymentService;
		}

		[HttpGet]
		public async Task<List<PaymentDto>> Get(PaymentType paymentType)
		{
			var payments = await _shareablePaymentService.GetAsync(paymentType);
			var paymentsDto = new List<PaymentDto>(payments.Count);
			paymentsDto.AddRange(payments.Select(payment => 
				new PaymentDto
				{
					Id = payment.Id, 
					Caption = payment.Caption, 
					Date = payment.Date, 
					Amount = payment.Amount,
					//TODO: Обязательно должны быть доли внутри payment + подумать как это будет работать с ролями
					IsCompleted = payment.IsCompleted
				}));

			return paymentsDto;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreatePaymentDto paymentDto)
		{
			_ = await _shareablePaymentService.CreateAsync(paymentDto);
			return Ok();
		}
	}
}
