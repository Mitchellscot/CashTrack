using CashTrack.Common;
using CashTrack.Data.Entities;
using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Models.SummaryModels;
using CashTrack.Models.UserModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ExpenseReviewService;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.IncomeCategoryService;
using CashTrack.Services.IncomeReviewService;
using CashTrack.Services.MainCategoriesService;
using CashTrack.Services.SubCategoryService;
using CashTrack.Services.SummaryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CashTrack.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModelBase
    {
        private readonly ISummaryService _summaryService;
        private readonly IExpenseService _expenseService;
        private readonly IExpenseReviewService _expenseReviewService;
        private readonly IIncomeReviewService _incomeReviewService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IMainCategoriesService _mainCategoryService;
        private readonly IIncomeCategoryService _incomeCategoryService;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly ILogger<IndexModel> _logger;

        public int ReviewAmount { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Year { get; set; } = DateTime.Now.Year;
        [BindProperty(SupportsGet = true)]
        public int Month { get; set; } = DateTime.Now.Month;
        public SelectList YearSelectList { get; set; }
        public SelectList MonthList { get; set; } = new SelectList(new Dictionary<string, int> {
            { "January", 1 },
            { "February", 2 },
            { "March", 3 },
            { "April", 4 },
            { "May", 5 },
            { "June", 6 },
            { "July", 7 },
            { "August", 8 },
            { "September", 9 },
            { "October", 10 },
            { "November", 11 },
            { "December", 12 }
        }, "Value", "Key", DateTime.Now.Month);
        public MonthlySummaryResponse SummaryResponse { get; set; }
        public SubCategoryDropdownSelection[] SubCategoryList { get; set; }
        public MainCategoryDropdownSelection[] MainCategoryList { get; set; }
        public IncomeCategoryDropdownSelection[] IncomeCategoryList { get; set; }
        public IndexModel(IExpenseService expenseService, IExpenseReviewService expenseReviewService, IIncomeReviewService incomeReviewService, ISummaryService summaryService, ISubCategoryService subCategoryService, IMainCategoriesService mainCategoryService, IIncomeCategoryService incomeCategoryService, SignInManager<UserEntity> signInManager, ILogger<IndexModel> logger)
        {
            _summaryService = summaryService;
            _expenseService = expenseService;
            _expenseReviewService = expenseReviewService;
            _incomeReviewService = incomeReviewService;
            _subCategoryService = subCategoryService;
            _mainCategoryService = mainCategoryService;
            _incomeCategoryService = incomeCategoryService;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            if (!UserIsAuthenticated && _env.Equals(CashTrackEnv.Production, StringComparison.CurrentCultureIgnoreCase))
            {
                var result = await _signInManager.PasswordSignInAsync("demo", "demo", true, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"A Demo User has logged in at {DateTime.Now} CST Time from {HttpContext.Connection.RemoteIpAddress}");
                }
                else
                {
                    InfoMessage = "Please use 'demo' as a login name and password to view the app.";
                    return LocalRedirect("Account/Login");
                }
            }

            var incomeReviews = await _incomeReviewService.GetCountOfIncomeReviews();
            var expenseReviews = await _expenseReviewService.GetCountOfExpenseReviews();
            if (expenseReviews > 0 || incomeReviews > 0)
            {
                ReviewAmount = incomeReviews + expenseReviews;
            }

            return await PrepareAndRenderPage();
        }
        private async Task<IActionResult> PrepareAndRenderPage()
        {
            IncomeCategoryList = await _incomeCategoryService.GetIncomeCategoryDropdownListAsync();
            SubCategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            MainCategoryList = await _mainCategoryService.GetMainCategoriesForDropdownListAsync();
            SummaryResponse = await _summaryService.GetMonthlySummaryAsync(new MonthlySummaryRequest() { Year = this.Year, Month = this.Month, UserId = int.Parse(this.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value) });
            YearSelectList = new SelectList(await _expenseService.GetAnnualSummaryYearsAsync());
            return Page();
        }
    }
}