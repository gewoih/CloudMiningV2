using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Payments.User;
using CloudMining.Interfaces.Interfaces;

namespace CloudMining.Application.Mappings;

public class UserPaymentMapper : IMapper<ShareablePayment, UserPaymentDto>
{
    private readonly ICurrentUserService _currentUserService;

    public UserPaymentMapper(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public UserPaymentDto ToDto(ShareablePayment model)
    {
        var currentUserId = _currentUserService.GetCurrentUserId();
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