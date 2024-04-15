using CloudMining.Application.DTO.Payments.Purchase;
using CloudMining.Application.Services.Payments.Purchase;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.App.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseDto purchaseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var payment = await _purchaseService.CreateAsync(purchaseDto);
            return View(payment);
        }
    }
}
