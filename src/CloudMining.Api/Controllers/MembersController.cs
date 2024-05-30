using CloudMining.Application.Mappings;
using CloudMining.Domain.Models.Identity;
using CloudMining.Domain.Models.Payments;
using CloudMining.Interfaces.DTO.Members;
using CloudMining.Interfaces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;
    private readonly IDepositService _depositService;
    private readonly IMapper<User, MemberDto> _memberMapper;
    private readonly IMapper<Deposit, MemberDepositDto> _memberDepositMapper;

    public MembersController(IUserManagementService userManagementService,
        IDepositService depositService,
        IMapper<User, MemberDto> memberMapper,
        IMapper<Deposit, MemberDepositDto> memberDepositMapper)
    {
        _userManagementService = userManagementService;
        _depositService = depositService;
        _memberMapper = memberMapper;
        _memberDepositMapper = memberDepositMapper;
    }
    
    [HttpGet]
    public async Task<IEnumerable<MemberDto>> Get()
    {
        var members = await _userManagementService.GetMembersAsync();
        var membersDto = members.Select(member => _memberMapper.ToDto(member));
        return membersDto;
    }
    
    [HttpGet("deposits")]
    public async Task<IEnumerable<MemberDepositDto>> GetMemberDeposits([FromQuery] Guid userId)
    {
        var memberDeposits = await _depositService.GetUserDeposits(userId);
        var memberDepositsDto = memberDeposits.Select(deposit => _memberDepositMapper.ToDto(deposit));
        return memberDepositsDto;
    }
}