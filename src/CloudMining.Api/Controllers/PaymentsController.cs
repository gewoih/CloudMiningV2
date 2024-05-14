using CloudMining.Application.DTO.Payments;
using CloudMining.Application.Mappings;
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
		private readonly IMapper<ShareablePayment, PaymentDto> _paymentMapper;
		private readonly IMapper<PaymentShare, PaymentShareDto> _paymentShareMapper;

		public PaymentsController(
			IShareablePaymentService shareablePaymentService, 
			IMapper<ShareablePayment, PaymentDto> paymentMapper, 
			IMapper<PaymentShare, PaymentShareDto> paymentShareMapper)
		{
			_shareablePaymentService = shareablePaymentService;
			_paymentMapper = paymentMapper;
			_paymentShareMapper = paymentShareMapper;
		}

		[HttpGet]
		public async Task<PaymentsPageDto> Get(
			[FromQuery] PaymentType paymentType, 
			[FromQuery] int skip = 0, 
			[FromQuery] int take = 10)
		{
			var paginatedPayments = await _shareablePaymentService.GetAsync(skip, take, paymentType);
			var totalPaymentsCount = await _shareablePaymentService.GetUserPaymentsCount(paymentType);
			
			var paymentsPageDto = new PaymentsPageDto
			{
				Items = paginatedPayments.Select(payment => _paymentMapper.ToDto(payment)).ToList(),
				TotalCount = totalPaymentsCount
			};
			return paymentsPageDto;
		}
		
	}
}
