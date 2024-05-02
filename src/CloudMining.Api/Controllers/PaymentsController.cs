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
		private readonly IMapper<ShareablePayment, PaymentDto> _mapper;

		public PaymentsController(IShareablePaymentService shareablePaymentService, IMapper<ShareablePayment, PaymentDto> mapper)
		{
			_shareablePaymentService = shareablePaymentService;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IEnumerable<PaymentDto>> Get(
			[FromQuery] PaymentType paymentType, 
			[FromQuery] int skip = 0, 
			[FromQuery] int take = 10)
		{
			var payments = await _shareablePaymentService.GetAsync(skip, take, paymentType);
			var paymentDtos = payments.Select(payment => _mapper.ToDto(payment));
			return paymentDtos;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreatePaymentDto paymentDto)
		{
			_ = await _shareablePaymentService.CreateAsync(paymentDto);
			return Ok();
		}
	}
}
