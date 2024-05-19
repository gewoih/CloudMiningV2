using CloudMining.Application.DTO.Payments;
using CloudMining.Application.DTO.Payments.Admin;
using CloudMining.Application.DTO.Payments.User;
using CloudMining.Application.Mappings;
using CloudMining.Application.Services.Payments;
using CloudMining.Application.Services.Users;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;
using CloudMining.Domain.Models.Payments.Shareable;
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
		private readonly IUserService _userService;
		private readonly IMapper<ShareablePayment, AdminPaymentDto> _adminPaymentMapper;
		private readonly IMapper<ShareablePayment, UserPaymentDto> _userPaymentMapper;
		private readonly IMapper<PaymentShare, PaymentShareDto> _paymentShareMapper;

		public PaymentsController(
			IShareablePaymentService shareablePaymentService, 
			IUserService userService,
			IMapper<ShareablePayment, AdminPaymentDto> adminPaymentMapper, 
			IMapper<PaymentShare, PaymentShareDto> paymentShareMapper, 
			IMapper<ShareablePayment, UserPaymentDto> userPaymentMapper)
		{
			_shareablePaymentService = shareablePaymentService;
			_userService = userService;
			_adminPaymentMapper = adminPaymentMapper;
			_paymentShareMapper = paymentShareMapper;
			_userPaymentMapper = userPaymentMapper;
		}

		[HttpGet]
		public async Task<PaymentsPageDto> Get(
			[FromQuery] PaymentType paymentType, 
			[FromQuery] int skip = 0, 
			[FromQuery] int take = 10,
			[FromQuery] bool withShares = false)
		{
			var paginatedPayments = await _shareablePaymentService.GetAsync(skip, take, withShares, paymentType);
			var totalPaymentsCount = await _shareablePaymentService.GetUserPaymentsCount(paymentType);
			var isCurrentUserAdmin = _userService.IsCurrentUserAdmin();
			
			var paymentsPageDto = new PaymentsPageDto
			{
				TotalCount = totalPaymentsCount
			};

			if (isCurrentUserAdmin)
				paymentsPageDto.Items = paginatedPayments.Select(payment => (PaymentDto)_adminPaymentMapper.ToDto(payment)).ToList();
			else
				paymentsPageDto.Items = paginatedPayments.Select(payment => (PaymentDto)_userPaymentMapper.ToDto(payment)).ToList();
			
			return paymentsPageDto;
		}

		[HttpGet("shares")]
		public async Task<IEnumerable<PaymentShareDto>> GetShares([FromQuery] Guid paymentId)
		{
			var paymentShares = await _shareablePaymentService.GetPaymentShares(paymentId);
			var paymentSharesDto = paymentShares.Select(paymentShare => _paymentShareMapper.ToDto(paymentShare));
			return paymentSharesDto;
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<PaymentDto> Create([FromBody] CreatePaymentDto createPaymentDto)
		{
			var payment = await _shareablePaymentService.CreateAsync(createPaymentDto);
			var paymentDto = _adminPaymentMapper.ToDto(payment);
			return paymentDto;
		}
	}
}
