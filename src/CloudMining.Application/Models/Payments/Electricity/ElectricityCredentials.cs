using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMining.Application.Models.Payments.Electricity
{
    public sealed class ElectricityCredentials
    {
        public DateTime CreatedDate { get; set; }
        public decimal Amount { get; set; }
    }
}
