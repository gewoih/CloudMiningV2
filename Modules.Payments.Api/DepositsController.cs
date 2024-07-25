using CloudMining.Common.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Payments.Contracts.DTO.Deposits;
using Modules.Payments.Contracts.DTO.User;
using Modules.Payments.Contracts.Interfaces;
using Modules.Payments.Domain.Models;
using Modules.Users.Contracts.Interfaces;
using Modules.Users.Domain.Models;

namespace Modules.Payments.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DepositsController : ControllerBase
{
	private readonly IUserManagementService _userManagementService;
	private readonly IDepositService _depositService;
	private readonly IMapper<User, MemberDto> _memberMapper;
	private readonly IMapper<Deposit, DepositDto> _depositMapper;

	public DepositsController(IDepositService depositService, 
		IMapper<Deposit, DepositDto> depositMapper, 
		IUserManagementService userManagementService, IMapper<User, MemberDto> memberMapper)
	{
		_depositService = depositService;
		_depositMapper = depositMapper;
		_userManagementService = userManagementService;
		_memberMapper = memberMapper;
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] DepositDto depositDto)
	{
		_ = await _depositService.AddDepositAndRecalculateShares(depositDto);
		return Ok();
	}
	
	[HttpGet("deposits")]
	public async Task<IEnumerable<DepositDto>> GetMemberDeposits([FromQuery] Guid userId)
	{
		var memberDeposits = await _depositService.GetUserDeposits(userId);
		var memberDepositsDto = memberDeposits.Select(deposit => _depositMapper.ToDto(deposit));
		return memberDepositsDto;
	}
	
	[HttpGet("members")]
	public async Task<IEnumerable<MemberDto>> Get()
	{
		var members = await _userManagementService.GetUsersAsync(withDeposits: true, withShareChanges: true);
		var membersDto = members.Select(member => _memberMapper.ToDto(member));
		return membersDto;
	}
}