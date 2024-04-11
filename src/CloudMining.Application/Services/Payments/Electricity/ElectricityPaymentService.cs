using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudMining.Application.Models.Payments.Electricity;
using CloudMining.Application.Services.Users;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;
using CloudMining.Domain.Models.Base;

namespace CloudMining.Application.Services.Payments.Electricity
{
    public sealed class ElectricityPaymentService : IElectricityPaymentService
    {
        private readonly IUserService _userService;

        public ElectricityPaymentService(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<ShareablePayment> CreateAsync(ElectricityCredentials credentials)
        {
            var allUsersIds = await _userService.GetAllUsersIdsAsync();
            var paymentShares = new List<PaymentShare>();
            foreach (var userId in allUsersIds)
            {
                var paymentShare = new PaymentShare
                {
                    UserId = userId,
                    Amount = ,
                    Percent = ,
                    IsCompleted = false
                };
                paymentShares.Add(paymentShare);
            }
            var newPayment = new ShareablePayment
            {
                Amount = credentials.Amount,
                Currency = ,
                Type = PaymentType.Electricity,
                IsCompleted = false,
                PaymentShares = paymentShares
            };
            return newPayment;
        }
    }
}
