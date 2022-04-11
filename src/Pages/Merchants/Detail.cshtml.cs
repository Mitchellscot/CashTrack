using CashTrack.Common.Exceptions;
using CashTrack.Models.MerchantModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.MerchantService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
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
        [BindProperty]
        public int MerchantId { get; set; }
        public string occuranceLabels { get; set; }
        public string occuranceTotals { get; set; }
        public SelectList MerchantSelectList { get; set; }
        [BindProperty(SupportsGet =true)]
        public string SearchTerm { get; set; }
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
                    return Page();
                }
            }
            var merchantNames = await _merchantService.GetAllMerchantNames();
            Merchant = await _merchantService.GetMerchantDetailAsync(id);
            MerchantSelectList = new SelectList(merchantNames, Merchant.Name);
            return Page();
        }
        public async Task<IActionResult> OnPostDelete()
        {
            try
            {
                var deleteSuccess = await _merchantService.DeleteMerchantAsync(MerchantId);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return LocalRedirect("~/Merchants/Index");
            }
            TempData["SuccessMessage"] = "Successfully Deleted a Merchant!";
            return LocalRedirect("~/Merchants/Index");
        }
    }
}
