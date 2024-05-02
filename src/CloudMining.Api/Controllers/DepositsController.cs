﻿using CloudMining.Application.DTO.Payments.Deposits;
using CloudMining.Application.Services.Deposits;
using CloudMining.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class DepositsController : ControllerBase
	{
		private readonly IDepositService _depositService;

		public DepositsController(IDepositService depositService)
		{
			_depositService = depositService;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateDepositDto depositDto)
		{
			_ = await _depositService.AddDepositAndRecalculateShares(depositDto);
			return Ok();
		}
	}
}
