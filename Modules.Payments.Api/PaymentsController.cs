using CloudMining.Common.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Payments.Contracts.DTO;
using Modules.Payments.Contracts.DTO.Admin;
using Modules.Payments.Contracts.DTO.User;
using Modules.Payments.Contracts.Interfaces;
using Modules.Payments.Domain.Enums;
using Modules.Payments.Domain.Models;
using Modules.Users.Contracts.Interfaces;

namespace Modules.Payments.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PaymentsController : ControllerBase
{
	private readonly IMapper<ShareablePayment, AdminPaymentDto> _adminPaymentMapper;
	private readonly ICurrentUserService _currentUserService;
	private readonly IMapper<PaymentShare, PaymentShareDto> _paymentShareMapper;
	private readonly IShareablePaymentService _shareablePaymentService;
	private readonly IMapper<ShareablePayment, UserPaymentDto> _userPaymentMapper;

	public PaymentsController(
		IShareablePaymentService shareablePaymentService,
		ICurrentUserService currentUserService,
		IMapper<ShareablePayment, AdminPaymentDto> adminPaymentMapper,
		IMapper<PaymentShare, PaymentShareDto> paymentShareMapper,
		IMapper<ShareablePayment, UserPaymentDto> userPaymentMapper)
	{
		_shareablePaymentService = shareablePaymentService;
		_currentUserService = currentUserService;
		_adminPaymentMapper = adminPaymentMapper;
		_paymentShareMapper = paymentShareMapper;
		_userPaymentMapper = userPaymentMapper;
	}

	[HttpGet]
	public async Task<PaymentsPageDto> Get(
		[FromQuery] PaymentType paymentType,
		[FromQuery] int skip = 0,
		[FromQuery] int take = 10)
	{
		var paginatedPayments = await _shareablePaymentService.GetAsync(skip, take, paymentType);
		var totalPaymentsCount = await _shareablePaymentService.GetUserPaymentsCount(paymentType);
		var isCurrentUserAdmin = _currentUserService.IsCurrentUserAdmin();

		var payments = isCurrentUserAdmin
			? paginatedPayments.Select(payment => (PaymentDto)_adminPaymentMapper.ToDto(payment)).ToList()
			: paginatedPayments.Select(payment => (PaymentDto)_userPaymentMapper.ToDto(payment)).ToList();

		var paymentsPageDto = new PaymentsPageDto(payments, totalPaymentsCount);

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

	[Authorize]
	[HttpPatch("status")]
	public async Task<IActionResult> ChangeStatus([FromBody] Guid paymentShareId)
	{
		var succeeded = await _shareablePaymentService.CompletePaymentShareAsync(paymentShareId);
		if (!succeeded)
			return NotFound();

		return Ok();
	}
}