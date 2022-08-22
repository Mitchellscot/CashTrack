using CashTrack.Models.ImportRuleModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ImportRulesService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace CashTrack.Pages.Import
{
    public class RulesModel : PageModelBase
    {
        private readonly IImportRulesService _service;

        public RulesModel(IImportRulesService service)
        {
            _service = service;
        }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public  ImportRuleResponse RuleRespose { get; set; }
        public async Task<IActionResult> OnGet()
        {
            RuleRespose = await _service.GetImportRulesAsync(new ImportRuleRequest());
            return Page();
        }
    }
}
