using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMining.Application.DTO.Payments.Purchase
{
    public sealed class CreatePurchaseDto
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Purpose { get; set; }
    }
}
