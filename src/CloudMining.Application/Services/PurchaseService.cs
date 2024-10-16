using CloudMining.Domain.Models.Purchases;
using CloudMining.Infrastructure.Database;
using CloudMining.Interfaces.DTO.Purchases;
using CloudMining.Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services;

public class PurchaseService : IPurchaseService
{
	private readonly CloudMiningContext _context;

	public PurchaseService(CloudMiningContext context)
	{
		_context = context;
	}

	public async Task<List<PurchaseDto>> GetPurchasesAsync()
	{
		var purchasesDto = await _context.Purchases
			.Select(purchase => new PurchaseDto(purchase.Id, purchase.Caption, purchase.Amount,purchase.Date))
			.ToListAsync()
			.ConfigureAwait(false);
		
		return purchasesDto;
	}

	public async Task<bool> CreatePurchaseAsync(CreatePurchaseDto purchaseDto)
	{
		var newPurchase = new Purchase
		{
			Caption = purchaseDto.Caption,
			Amount = purchaseDto.Amount,
			Date = DateOnly.FromDateTime(purchaseDto.Date)
		};

		await _context.Purchases.AddAsync(newPurchase);
		var result = await _context.SaveChangesAsync();

		return result > 0;
	}
}