using CloudMining.Contracts.DTO.Payments.User;
using CloudMining.Contracts.Interfaces;
using CloudMining.Domain.Models;
using CloudMining.Domain.Models.Payments.Shareable;

namespace CloudMining.Application.Mappings;

public class UserPaymentMapper : IMapper<ShareablePayment, UserPaymentDto>
{
    private readonly IUserService _userService;

    public UserPaymentMapper(IUserService userService)
    {
        _userService = userService;
    }

    public UserPaymentDto ToDto(ShareablePayment model)
    {
        var currentUserId = _userService.GetCurrentUserId();
        var currentUserPaymentShare = model.PaymentShares.Find(paymentShare => paymentShare.UserId == currentUserId);

        var userShare = currentUserPaymentShare?.Share ?? 0;
        var userSharedAmount = currentUserPaymentShare?.Amount ?? 0;

        return new UserPaymentDto
        {
            Id = model.Id,
            Amount = model.Amount,
            Date = model.Date,
            IsCompleted = model.IsCompleted,
            Share = userShare,
            SharedAmount = userSharedAmount
        };
    }

    public ShareablePayment ToDomain(UserPaymentDto dto)
    {
        throw new NotImplementedException();
    }
}