using CloudMining.Interfaces.DTO.Payments.Deposits;
using CloudMining.Interfaces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.Api.Controllers;

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
	
	[Authorize(Roles = "Admin")]
	[HttpPost]
	public async Task<IActionResult> Create([FromBody] DepositDto depositDto)
	{
		_ = await _depositService.AddDepositAndRecalculateShares(depositDto);
		return Ok();
	}
}