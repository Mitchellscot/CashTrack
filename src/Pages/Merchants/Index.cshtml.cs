using CashTrack.Models.MerchantModels;
using CashTrack.Services.MerchantService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Merchants
{
    public class IndexModel : PageModel
    {

        private readonly IMerchantService _merchantService;
        public IndexModel(IMerchantService merchantService) => _merchantService = merchantService;
        [TempData]
        public string InfoMessage { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        public AddMerchantModal MerchantModal { get; set; }
        public async Task<IActionResult> OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAddMerchantModal()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Error adding Merchant. Please try again");
                return LocalRedirect(MerchantModal.Returnurl);
            }
            try
            {
                var merchantSuccess = await _merchantService.CreateMerchantAsync(new Merchant()
                {
                    Name = MerchantModal.Name,
                    City = MerchantModal.City,
                    State = MerchantModal.State,
                    IsOnline = MerchantModal.IsOnline,
                    SuggestOnLookup = MerchantModal.SuggestOnLookup,
                    Notes = MerchantModal.Notes
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return LocalRedirect(MerchantModal.Returnurl);
            }

            TempData["SuccessMessage"] = "Successfully added a new Merchant!";
            return LocalRedirect(MerchantModal.Returnurl);
        }
    }
}
