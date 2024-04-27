﻿using CloudMining.Application.DTO.Payments;
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
		public async Task<List<ShareablePayment>> Get(PaymentType paymentType)
		{
			return await _shareablePaymentService.GetAsync(paymentType);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreatePaymentDto paymentDto)
		{
			_ = await _shareablePaymentService.CreateAsync(paymentDto);
			return Ok();
		}
	}
}
