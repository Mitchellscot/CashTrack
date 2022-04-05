using CashTrack.Models.MerchantModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.MerchantService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CashTrack.Pages.Merchants
{

    public class DetailModel : PageModelBase
    {
        private readonly IMerchantService _merchantService;
        public DetailModel(IMerchantService merchantService) => _merchantService = merchantService;
        [BindProperty(SupportsGet = true)]
        public int id { get; set; }
        public MerchantDetail Merchant { get; set; }
        public async Task<IActionResult> OnGet()
        {
            Merchant = await _merchantService.GetMerchantDetailAsync(id);

            return Page();
        }
    }
}
