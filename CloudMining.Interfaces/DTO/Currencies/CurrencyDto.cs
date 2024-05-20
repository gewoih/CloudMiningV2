﻿using CloudMining.Domain.Enums;

namespace CloudMining.Interfaces.DTO.Currencies
{
    public sealed class CurrencyDto
    {
        public string Name { get; set; }
        public CurrencyCode Code { get; set; }
        public int Precision { get; set; }
    }
}
