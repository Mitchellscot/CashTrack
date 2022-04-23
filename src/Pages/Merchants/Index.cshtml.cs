using CashTrack.Common.Exceptions;
using CashTrack.Models.Common;
using CashTrack.Models.MerchantModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.MerchantService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Merchants
{
    public class IndexModel : PageModelBase
    {
        private readonly IMerchantService _merchantService;
        public IndexModel(IMerchantService merchantService) => _merchantService = merchantService;
        [BindProperty]
        public AddEditMerchantModal MerchantModal { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public MerchantResponse MerchantResponse { get; set; }
        [BindProperty(SupportsGet = true)]
        public MerchantOrderBy Query { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool Q2 { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (SearchTerm != null)
            {
                try
                {
                    var merchantId = (await _merchantService.GetMerchantByNameAsync(SearchTerm)).Id;
                    return RedirectToPage("./Detail", new { id = merchantId });
                }
                catch (MerchantNotFoundException)
                {
                    InfoMessage = "No merchant found with the name " + SearchTerm;
                    MerchantResponse = await _merchantService.GetMerchantsAsync(new MerchantRequest() { Reversed = Q2, Order = Query, PageNumber = this.PageNumber });
                    return Page();
                }
            }

            MerchantResponse = await _merchantService.GetMerchantsAsync(new MerchantRequest() { Reversed = Q2, Order = Query, PageNumber = this.PageNumber });
            return Page();
        }
        public async Task<IActionResult> OnPostAddEditMerchantModal()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Error adding Merchant. Please try again");
                MerchantResponse = await _merchantService.GetMerchantsAsync(new MerchantRequest() { Reversed = Q2, Order = Query, PageNumber = this.PageNumber });
                return Page();
            }
            try
            {
                var newMerchant = new Merchant()
                {
                    Name = MerchantModal.Name,
                    City = MerchantModal.City,
                    State = MerchantModal.State,
                    IsOnline = MerchantModal.IsOnline,
                    SuggestOnLookup = MerchantModal.SuggestOnLookup,
                    Notes = MerchantModal.Notes
                };
                if (MerchantModal.IsEdit)
                {
                    newMerchant.Id = MerchantModal.Id;
                }

                var merchantSuccess = MerchantModal.IsEdit ? await _merchantService.UpdateMerchantAsync(newMerchant) : await _merchantService.CreateMerchantAsync(newMerchant);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                MerchantResponse = await _merchantService.GetMerchantsAsync(new MerchantRequest() { Reversed = Q2, Order = Query, PageNumber = this.PageNumber });
                return Page();
            }

            TempData["SuccessMessage"] = MerchantModal.IsEdit ? "Successfully edited a Merchant!" : "Successfully added a new Merchant!";
            return LocalRedirect(MerchantModal.Returnurl);
        }

    }
}
