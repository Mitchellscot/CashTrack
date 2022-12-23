using CashTrack.Common.Exceptions;
using CashTrack.Models.IncomeSourceModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.IncomeSourceService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Sources
{
    public class IndexModel : PageModelBase
    {
        private readonly IIncomeSourceService _sourceService;
        public IndexModel(IIncomeSourceService sourceService) => _sourceService = sourceService;
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public IncomeSourceResponse SourceResponse { get; set; }
        [BindProperty]
        public AddEditIncomeSourceModal SourceModal { get; set; }

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
                    InfoMessage = "No income source found with the name " + SearchTerm;
                    SourceResponse = await _sourceService.GetIncomeSourcesAsync(new IncomeSourceRequest() { PageNumber = this.PageNumber });
                    return Page();
                }
            }

            SourceResponse = await _sourceService.GetIncomeSourcesAsync(new IncomeSourceRequest() { PageNumber = this.PageNumber });
            return Page();
        }
        public async Task<IActionResult> OnPostAddEditIncomeSourceModal()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Error adding Income Source. Please try again");
                SourceResponse = await _sourceService.GetIncomeSourcesAsync(new IncomeSourceRequest() { PageNumber = this.PageNumber });
                return Page();
            }
            try
            {
                var newSource = new IncomeSource()
                {
                    Name = SourceModal.Name,
                    Notes = SourceModal.Notes,
                    SuggestOnLookup = SourceModal.SuggestOnLookup,
                    City = SourceModal.City,
                    State = SourceModal.State,
                    IsOnline = SourceModal.IsOnline,
                };
                if (SourceModal.IsEdit)
                {
                    newSource.Id = SourceModal.Id;
                }

                var sourceId = SourceModal.IsEdit ? await _sourceService.UpdateIncomeSourceAsync(newSource) : await _sourceService.CreateIncomeSourceAsync(newSource);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                SourceResponse = await _sourceService.GetIncomeSourcesAsync(new IncomeSourceRequest() { PageNumber = this.PageNumber });
                return Page();
            }

            TempData["SuccessMessage"] = SourceModal.IsEdit ? "Successfully edited an Income Source!" : "Successfully added a new Income Source!";
            return RedirectToPage(SourceModal.ReturnUrl);
        }

    }
}
