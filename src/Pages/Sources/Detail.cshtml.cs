using CashTrack.Common.Exceptions;
using CashTrack.Models.IncomeSourceModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.IncomeSourceService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Sources
{
    public class DetailModel : PageModelBase
    {
        private readonly IIncomeSourceService _sourceService;

        public DetailModel(IIncomeSourceService sourceService) => _sourceService = sourceService;
        [BindProperty(SupportsGet = true)]
        public int id { get; set; }
        public IncomeSourceDetail Source { get; set; }
        public SelectList SourceSelectList { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (SearchTerm != null)
            {
                try
                {
                    var sourceId = (await _sourceService.GetIncomeSourceByName(SearchTerm)).Id;
                    return RedirectToPage("./Detail", new { id = sourceId });
                }
                catch (IncomeSourceNotFoundException)
                {
                    InfoMessage = "No merchant found with the name " + SearchTerm;
                    return Page();
                }
            }
            var sourceNames = await _sourceService.GetAllIncomeSourceNames();
            Source = await _sourceService.GetIncomeSourceDetailAsync(id);
            SourceSelectList = new SelectList(sourceNames, Source.Name);
            return Page();
        }
        public async Task<IActionResult> OnPostDelete()
        {
            //TODO: When deleting, set all incomes sourceId to null
            try
            {
                var deleteSuccess = await _sourceService.DeleteIncomeSourceAsync(id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return LocalRedirect("~/Sources/Index");
            }
            TempData["SuccessMessage"] = "Successfully Deleted an Income Source!";
            return LocalRedirect("~/Sources/Index");
        }
    }
}
