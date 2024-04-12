using CloudMining.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMining.Application.Models.Currencies
{
    public sealed class CurrencyDTO
    {
        public string Caption { get; set; }
        public CurrencyCode Code { get; set; }
        public int Precision { get; set; }
    }
}
