using CloudMining.Domain.Models;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Payments.User;
using CloudMining.Interfaces.Interfaces;

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
            Status = currentUserPaymentShare?.Status ?? 0,
            Share = userShare,
            SharedAmount = userSharedAmount
        };
    }

    public ShareablePayment ToDomain(UserPaymentDto dto)
    {
        throw new NotImplementedException();
    }
}