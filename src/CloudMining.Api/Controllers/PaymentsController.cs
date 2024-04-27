using AutoMapper;
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
		private readonly IMapper _mapper;

		public PaymentsController(IShareablePaymentService shareablePaymentService, IMapper mapper)
		{
			_shareablePaymentService = shareablePaymentService;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<List<PaymentDto>> Get(PaymentType paymentType)
		{
			var payments = await _shareablePaymentService.GetAsync(paymentType);
			var paymentDtos = _mapper.Map<List<ShareablePayment>, List<PaymentDto>>(payments);
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
