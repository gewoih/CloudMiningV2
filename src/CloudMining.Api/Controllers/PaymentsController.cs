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
		public async Task<PaymentListDto> Get(
			[FromQuery] PaymentType paymentType, 
			[FromQuery] int skip = 0, 
			[FromQuery] int take = 10)
		{
			var payments = await _shareablePaymentService.GetAsync(skip, take, paymentType);
			var paymentDtos = payments.Item1.Select(payment => _paymentMapper.ToDto(payment));
			var paymentListDto = new PaymentListDto()
			{
				Payments = paymentDtos,
				TotalRecords = payments.Item2
			};
			return paymentListDto;
		}

		[HttpGet("shares")]
		public async Task<IEnumerable<PaymentShareDto>> GetShares([FromQuery] Guid paymentId)
		{
			var paymentShares = await _shareablePaymentService.GetPaymentShares(paymentId);
			var paymentSharesDto = paymentShares.Select(paymentShare => _paymentShareMapper.ToDto(paymentShare));
			return paymentSharesDto;
		}

		[HttpPost]
		public async Task<PaymentDto> Create([FromBody] CreatePaymentDto createPaymentDto)
		{
			var payment = await _shareablePaymentService.CreateAsync(createPaymentDto);
			var paymentDto = _paymentMapper.ToDto(payment);
			return paymentDto;
		}
	}
}
