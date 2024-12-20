﻿using CloudMining.Domain.Models.Purchases;
using CloudMining.Interfaces.DTO.Purchases;

namespace CloudMining.Interfaces.Interfaces;

public interface IPurchaseService
{
	Task<List<PurchaseDto>> GetPurchasesAsync();
	Task<Purchase> CreatePurchaseAsync(CreatePurchaseDto purchaseDto);
}