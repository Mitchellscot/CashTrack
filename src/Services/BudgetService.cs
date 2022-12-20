using AutoMapper;
using CashTrack.Common;
using CashTrack.Common.Exceptions;
using CashTrack.Data.Entities;
using CashTrack.Models.BudgetModels;
using CashTrack.Repositories.BudgetRepository;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Services.BudgetService
{
    public interface IBudgetService
    {
        Task<AnnualBudgetPageResponse> GetAnnualBudgetPageAsync(AnnualBudgetPageRequest request);
        Task<MonthlyBudgetPageResponse> GetMonthlyBudgetPageAsync(MonthlyBudgetPageRequest request);
        Task<BudgetListResponse> GetBudgetListAsync(BudgetListRequest request);
        Task<CategoryAveragesAndTotals> GetCategoryAveragesAndTotalsAsync(int subCategoryId);
        Task<int[]> GetAnnualBudgetYearsAsync();
        Task<int> CreateBudgetItemAsync(AddEditBudgetAllocation request);
        Task<int> UpdateBudgetAsync(AddEditBudgetAllocation request);
        Task<bool> DeleteBudgetAsync(int id);
    }
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepo;
        private readonly IExpenseRepository _expenseRepo;
        private readonly IMapper _mapper;

        public BudgetService(IBudgetRepository budgetRepo, IExpenseRepository expenseRepo, IMapper mapper) => (_budgetRepo, _expenseRepo, _mapper) = (budgetRepo, expenseRepo, mapper);

        public async Task<AnnualBudgetPageResponse> GetAnnualBudgetPageAsync(AnnualBudgetPageRequest request)
        {
            var budgets = await _budgetRepo.FindWithMainCategories(x => x.Year == request.Year);

            var monthlyIncome = GetAnnualData(budgets, BudgetType.Income).ToList();
            var monthlyNeeds = GetAnnualData(budgets, BudgetType.Need).ToList();
            var monthlyWants = GetAnnualData(budgets, BudgetType.Want).ToList();
            var monthlySavings = GetAnnualSavingsData(budgets).ToList();
            var monthlyUnallocated = GetAnnualUnallocatedData(budgets).ToList();

            var totalAnnualIncome = monthlyIncome.Sum();

            var savingsForTypeChat = monthlySavings.Sum().ToPercentage(totalAnnualIncome) > 0 ? monthlySavings.Sum().ToPercentage(totalAnnualIncome) : 0;
            return new AnnualBudgetPageResponse()
            {
                AnnualBudgetChartData = new AnnualBudgetChartData()
                {
                    IncomeData = monthlyIncome,
                    NeedsData = monthlyNeeds,
                    WantsData = monthlyWants,
                    SavingsData = monthlySavings,
                    Unallocated = monthlyUnallocated
                },
                AnnualSummary = new BudgetSummary()
                {
                    IncomeAmount = totalAnnualIncome,
                    ExpensesAmount = monthlyNeeds.Sum() + monthlyWants.Sum(),
                    NeedsAmount = monthlyNeeds.Sum(),
                    WantsAmount = monthlyWants.Sum(),
                    SavingsAmount = monthlySavings.Sum(),
                    UnallocatedAmount = monthlyUnallocated.Sum()
                },
                TypePercentages = new Dictionary<string, int>()
                {
                    {"Needs", monthlyNeeds.Sum().ToPercentage(totalAnnualIncome) },
                    {"Wants", monthlyWants.Sum().ToPercentage(totalAnnualIncome) },
                    {"Savings", savingsForTypeChat },
                    {"Unallocated", monthlyUnallocated.Sum().ToPercentage(totalAnnualIncome) }
                },
                SubCategoryPercentages = GetSubCategoryPercentages(budgets, totalAnnualIncome),
                MainCategoryPercentages = GetMainCategoryPercentages(budgets, totalAnnualIncome),
                BudgetBreakdown = GetBudgetBreakdown(budgets, totalAnnualIncome)
            };
        }
        public async Task<MonthlyBudgetPageResponse> GetMonthlyBudgetPageAsync(MonthlyBudgetPageRequest request)
        {
            var budgets = await _budgetRepo.FindWithMainCategories(x => x.Year == request.Year && x.Month == request.Month);

            var mainCategoryLabels = budgets.Where(x => x.SubCategoryId != null && x.Amount > 0).Select(x => x.SubCategory.MainCategory.Name).OrderBy(x => x).Distinct().ToArray();

            var savingsAllocations = budgets.Where(x => x.BudgetType == BudgetType.Savings).Sum(x => x.Amount);
            var incomeAmount = budgets.Where(x => x.BudgetType == BudgetType.Income).Sum(x => x.Amount);
            var needsAmount = budgets.Where(x => x.BudgetType == BudgetType.Need).Sum(x => x.Amount);
            var wantsAmount = budgets.Where(x => x.BudgetType == BudgetType.Want).Sum(x => x.Amount);
            var expenseAmounts = needsAmount + wantsAmount;

            var expensesAndSavingsAmount = savingsAllocations + expenseAmounts;

            var adjustedSavings = expensesAndSavingsAmount > incomeAmount ?
                savingsAllocations + (incomeAmount - expensesAndSavingsAmount) : savingsAllocations;

            var unallocatedAmount = (incomeAmount - expensesAndSavingsAmount) > 0 ?
                incomeAmount - expensesAndSavingsAmount : 0;

            //taking income off since it makes the graph look wonky
            var incomeExists = false; //incomeAmount > 0;
            var savingsExists = adjustedSavings != 0;
            var unallocatedExists = unallocatedAmount > 0;

            var savingsForTypeChart = adjustedSavings.ToPercentage(incomeAmount) > 0 ? adjustedSavings.ToPercentage(incomeAmount) : 0;
            return new MonthlyBudgetPageResponse()
            {
                MonthlyBudgetChartData = new MonthlyBudgetChartData()
                {
                    Labels = ChartUtilities.GenerateMonthlyChartLabels(incomeExists, mainCategoryLabels, savingsExists, unallocatedExists),
                    //IncomeData = ChartUtilities.GetMonthlyIncomeData(incomeAmount, mainCategoryLabels.Length, savingsExists, unallocatedExists),
                    ExpenseData = ChartUtilities.GetMonthlyBudgetExpenseData(budgets, incomeExists, savingsExists, mainCategoryLabels, unallocatedExists),
                    SavingsData = ChartUtilities.GetMonthlySavingsData(incomeExists, mainCategoryLabels.Length, adjustedSavings, unallocatedExists),
                    Unallocated = ChartUtilities.GetMonthlyUnallocatedData(incomeExists, mainCategoryLabels.Length, savingsExists, unallocatedAmount)
                },
                MonthlySummary = new BudgetSummary()
                {
                    IncomeAmount = incomeAmount,
                    ExpensesAmount = expenseAmounts,
                    NeedsAmount = needsAmount,
                    WantsAmount = wantsAmount,
                    SavingsAmount = adjustedSavings,
                    UnallocatedAmount = unallocatedAmount
                },
                TypePercentages = new Dictionary<string, int>()
                {
                    {"Needs", needsAmount.ToPercentage(incomeAmount) },
                    {"Wants", wantsAmount.ToPercentage(incomeAmount) },
                    {"Savings", savingsForTypeChart },
                    {"Unallocated", unallocatedAmount.ToPercentage(incomeAmount) }
                },
                SubCategoryPercentages = GetSubCategoryPercentages(budgets, incomeAmount),
                MainCategoryPercentages = GetMainCategoryPercentages(budgets, incomeAmount),
                BudgetBreakdown = GetBudgetBreakdown(budgets, incomeAmount)
            };
        }

        public async Task<BudgetListResponse> GetBudgetListAsync(BudgetListRequest request)
        {
            var budgetListItems = await ParseBudgetListQuery(request);
            var count = await _budgetRepo.GetCount(x => true);
            return new BudgetListResponse(request.PageNumber, request.PageSize, count, budgetListItems);
        }
        public async Task<CategoryAveragesAndTotals> GetCategoryAveragesAndTotalsAsync(int subCategoryId)
        {
            var expenses = await _expenseRepo.Find(x => x.CategoryId == subCategoryId && x.Date.Year > DateTime.Now.AddYears(-3).Year);
            var sixMonthTotal = decimal.Round(expenses.Where(x => x.Date > DateTime.Now.AddMonths(-6)).Sum(x => x.Amount));
            var thisYearTotal = decimal.Round(expenses.Where(x => x.Date.Year == DateTime.Now.Year).Sum(x => x.Amount));
            var lastYearTotal = decimal.Round(expenses.Where(x => x.Date.Year == (DateTime.Now.Year - 1)).Sum(x => x.Amount));
            var twoYearsAgoTotal = decimal.Round(expenses.Where(x => x.Date.Year == (DateTime.Now.Year - 2)).Sum(x => x.Amount));

            return new CategoryAveragesAndTotals()
            {
                SixMonthAverages = decimal.Round(sixMonthTotal / 6),
                ThisYearAverages = decimal.Round(thisYearTotal / DateTime.Now.Month),
                LastYearAverages = decimal.Round(lastYearTotal / 12),
                TwoYearsAgoAverages = decimal.Round(twoYearsAgoTotal / 12),
                SixMonthTotals = sixMonthTotal,
                ThisYearTotals = thisYearTotal,
                LastYearTotals = lastYearTotal,
                TwoYearsAgoTotals = twoYearsAgoTotal
            };
        }
        public async Task<int[]> GetAnnualBudgetYearsAsync()
        {
            return (await _budgetRepo.Find(x => true)).GroupBy(x => x.Year).Select(x => x.Key).ToArray();
        }

        public async Task<int> CreateBudgetItemAsync(AddEditBudgetAllocation request)
        {
            var isIncome = request.IsIncome || request.Type == BudgetType.Income;
            var annualBudget = request.Month == 0 || request.TimeSpan == AllocationTimeSpan.Year;
            var isWeekly = request.TimeSpan == AllocationTimeSpan.Week;
            var isYearly = request.TimeSpan == AllocationTimeSpan.Year && request.Month == 0;

            if (request.Amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(request.Amount));

            if (!annualBudget && request.Month < 0 || !annualBudget && request.Month > 12)
                throw new ArgumentOutOfRangeException(nameof(request.Month));

            if (!isIncome && request.Type != BudgetType.Savings && request.SubCategoryId == 0)
                throw new ArgumentException("Expense Budgets must have a Category");

            if (annualBudget)
            {
                var budgetEntities = new List<BudgetEntity>();
                int amount;
                if (isWeekly)
                    amount = (request.Amount * 52) / 12;
                else if (isYearly)
                    amount = request.Amount / 12;
                else
                    amount = request.Amount;

                for (int i = 1; i <= 12; i++)
                {
                    var onceAMonthBudget = new BudgetEntity();
                    onceAMonthBudget.Amount = amount;
                    onceAMonthBudget.Month = i;
                    onceAMonthBudget.Year = request.Year;
                    onceAMonthBudget.BudgetType = isIncome ? BudgetType.Income : request.Type;
                    onceAMonthBudget.SubCategoryId = isIncome || request.Type == BudgetType.Savings ? null : request.SubCategoryId;
                    budgetEntities.Add(onceAMonthBudget);
                }
                return await _budgetRepo.CreateMany(budgetEntities);
            }

            var budgetEntity = new BudgetEntity();
            budgetEntity.Amount = isWeekly ? (request.Amount * 52) / 12 : request.Amount;
            budgetEntity.Month = request.Month;
            budgetEntity.Year = request.Year;
            budgetEntity.BudgetType = isIncome ? BudgetType.Income : request.Type;
            budgetEntity.SubCategoryId = isIncome ? null : request.SubCategoryId;

            return await _budgetRepo.Create(budgetEntity);
        }
        public async Task<int> UpdateBudgetAsync(AddEditBudgetAllocation request)
        {
            if (request.Id == null)
                throw new ArgumentException("Need an Id to update a budget");

            var isIncome = request.IsIncome || request.Type == BudgetType.Income;

            if (request.Amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(request.Amount));

            if (request.Month < 0 || request.Month > 12)
                throw new ArgumentOutOfRangeException(nameof(request.Month));

            if (!isIncome && request.Type != BudgetType.Savings && request.SubCategoryId == 0)
                throw new ArgumentException("Expense Budgets must have a Category");

            var currentBudget = await _budgetRepo.FindById(request.Id.Value);

            currentBudget.BudgetType = isIncome ? BudgetType.Income : request.Type;
            currentBudget.Amount = request.Amount;
            currentBudget.SubCategoryId = isIncome || request.Type == BudgetType.Savings ? null : request.SubCategoryId;
            currentBudget.Month = request.Month;
            currentBudget.Year = request.Year;
            return await _budgetRepo.Update(currentBudget);
        }
        public async Task<bool> DeleteBudgetAsync(int id)
        {
            var budget = await _budgetRepo.FindById(id);
            if (budget == null)
                throw new BudgetNotFoundException("Invalid Budget Id");

            return await _budgetRepo.Delete(budget);
        }
        internal IEnumerable<int> GetAnnualData(BudgetEntity[] budgets, BudgetType type)
        {
            var monthlyData = budgets.Where(x => x.BudgetType == type).GroupBy(x => x.Month).Select(x =>
            {
                return (Month: x.Key, Amount: x.Sum(x => x.Amount));
            }).OrderBy(x => x.Month).ToDictionary(k => k.Month, v => v.Amount);

            for (int i = 1; i <= 12; i++)
            {
                if (monthlyData.Any(x => x.Key == i))
                    yield return monthlyData[i];
                else
                    yield return 0;
            }
        }
        internal IEnumerable<int> GetAnnualSavingsData(BudgetEntity[] budgets)
        {
            var monthlyIncome = budgets.Where(x => x.BudgetType == BudgetType.Income).GroupBy(x => x.Month).Select(x =>
            {
                return (Month: x.Key, Amount: x.Sum(x => x.Amount));
            }).OrderBy(x => x.Month).ToDictionary(k => k.Month, v => v.Amount);

            var monthlyExpenses = budgets.Where(x => x.BudgetType == BudgetType.Need || x.BudgetType == BudgetType.Want).GroupBy(x => x.Month).Select(x =>
            {
                return (Month: x.Key, Amount: x.Sum(x => x.Amount));
            }).OrderBy(x => x.Month).ToDictionary(k => k.Month, v => v.Amount);

            var monthlySavings = budgets.Where(x => x.BudgetType == BudgetType.Savings).GroupBy(x => x.Month).Select(x =>
            {
                return (Month: x.Key, Amount: x.Sum(x => x.Amount));
            }).OrderBy(x => x.Month).ToDictionary(k => k.Month, v => v.Amount);

            for (int i = 1; i <= 12; i++)
            {
                if (monthlyIncome.Any(x => x.Key == i) && monthlyExpenses.Any(x => x.Key == i) && monthlySavings.Any(x => x.Key == i))
                {
                    if (monthlyIncome[i] > (monthlyExpenses[i] + monthlySavings[i]))
                        yield return monthlySavings[i];
                    else if (monthlyIncome[i] < (monthlyExpenses[i] + monthlySavings[i]))
                        yield return (monthlyIncome[i] - (monthlyExpenses[i] + monthlySavings[i])) + monthlySavings[i];
                    else yield return monthlySavings[i];
                }
                else if (monthlyIncome.Any(x => x.Key == i) && !monthlyExpenses.Any(x => x.Key == i))
                    yield return monthlySavings[i];
                else if (!monthlyIncome.Any(x => x.Key == i) && monthlyExpenses.Any(x => x.Key == i))
                    yield return 0 - monthlyExpenses[i];
                else yield return 0;
            }
        }
        internal IEnumerable<int> GetAnnualUnallocatedData(BudgetEntity[] budgets)
        {
            var monthlyIncome = budgets.Where(x => x.BudgetType == BudgetType.Income).GroupBy(x => x.Month).Select(x =>
            {
                return (Month: x.Key, Amount: x.Sum(x => x.Amount));
            }).OrderBy(x => x.Month).ToDictionary(k => k.Month, v => v.Amount);

            var monthlyExpenses = budgets.Where(x => x.BudgetType != BudgetType.Income).GroupBy(x => x.Month).Select(x =>
            {
                return (Month: x.Key, Amount: x.Sum(x => x.Amount));
            }).OrderBy(x => x.Month).ToDictionary(k => k.Month, v => v.Amount);

            for (int i = 1; i <= 12; i++)
            {
                if (monthlyIncome.Any(x => x.Key == i) && monthlyExpenses.Any(x => x.Key == i))
                {
                    if (monthlyIncome[i] > monthlyExpenses[i])
                        yield return monthlyIncome[i] - monthlyExpenses[i];
                    else yield return 0;
                }
                else if (monthlyIncome.Any(x => x.Key == i) && !monthlyExpenses.Any(x => x.Key == i))
                    yield return monthlyIncome[i];
                else
                    yield return 0;
            }
        }
        internal Dictionary<string, int> GetSubCategoryPercentages(BudgetEntity[] budgets, int totalAnnualIncome)
        {
            if (budgets.Length == 0)
                return new Dictionary<string, int>();

            var expenseTotal = budgets.Where(x => x.SubCategoryId != null).Sum(x => x.Amount);
            var amountToPercentBy = expenseTotal > totalAnnualIncome ? expenseTotal : totalAnnualIncome;

            var expensePercentagesOfIncome = budgets.Where(x => x.SubCategoryId != null).GroupBy(x => x.SubCategory.Name).Select(x =>
            {
                return (Name: x.Key, Amount: x.Sum(x => x.Amount));
            }).Select(x =>
            {
                return (Name: x.Name, Percentage: x.Amount.ToPercentage(amountToPercentBy));
            }).Where(x => x.Percentage > 0).ToDictionary(k => k.Name, v => v.Percentage);

            var savingsAllocated = GetAnnualSavingsData(budgets).Sum();

            if (savingsAllocated > 0)
                expensePercentagesOfIncome.Add("Savings", savingsAllocated.ToPercentage(amountToPercentBy));

            var unAllocated = GetAnnualUnallocatedData(budgets).Sum();

            if (unAllocated > 0)
                expensePercentagesOfIncome.Add("Unallocated", unAllocated.ToPercentage(amountToPercentBy));

            return expensePercentagesOfIncome;
        }
        internal Dictionary<string, int> GetMainCategoryPercentages(BudgetEntity[] budgets, int totalAnnualIncome)
        {
            if (budgets.Length == 0)
                return new Dictionary<string, int>();

            var expenseTotal = budgets.Where(x => x.SubCategoryId != null).Sum(x => x.Amount);
            var amountToPercentBy = expenseTotal > totalAnnualIncome ? expenseTotal : totalAnnualIncome;

            var expensePercentages = budgets.Where(x => x.SubCategoryId != null).GroupBy(x => x.SubCategory.MainCategory.Name).Select(x =>
            {
                return (Name: x.Key, Amount: x.Sum(x => x.Amount));
            }).Select(x =>
            {
                return (Name: x.Name, Percentage: x.Amount.ToPercentage(amountToPercentBy));
            }).Where(x => x.Percentage > 0).ToDictionary(k => k.Name, v => v.Percentage);

            var savingsAllocated = GetAnnualSavingsData(budgets).Sum();

            if (savingsAllocated > 0)
                expensePercentages.Add("Savings", savingsAllocated.ToPercentage(amountToPercentBy));

            var unAllocated = GetAnnualUnallocatedData(budgets).Sum();

            if (unAllocated > 0)
                expensePercentages.Add("Unallocated", unAllocated.ToPercentage(amountToPercentBy));

            return expensePercentages;
        }
        internal List<BudgetBreakdown> GetBudgetBreakdown(BudgetEntity[] budgets, int totalIncome)
        {
            var monthlyBudgets = new List<BudgetBreakdown>();
            var subCategories = budgets.Where(x => x.SubCategoryId != null).GroupBy(x => x.SubCategoryId).Select(x =>
            {
                return new BudgetBreakdown()
                {
                    MainCategoryId = x.Select(x => x.SubCategory.MainCategoryId).FirstOrDefault(),
                    SubCategoryId = x.Key.Value,
                    Name = x.Select(x => x.SubCategory.Name).FirstOrDefault(),
                    Amount = x.Sum(x => x.Amount),
                    Percentage = x.Sum(x => x.Amount).ToPercentage(totalIncome)
                };
            }).ToList();
            monthlyBudgets.AddRange(subCategories);
            var mainCategories = budgets.Where(x => x.SubCategoryId != null).GroupBy(x => x.SubCategory.MainCategory.Id).Select(x =>
            {
                return new BudgetBreakdown()
                {
                    MainCategoryId = x.Key,
                    SubCategoryId = 0,
                    Name = x.Select(x => x.SubCategory.MainCategory.Name).FirstOrDefault(),
                    Amount = x.Sum(x => x.Amount),
                    Percentage = x.Sum(x => x.Amount).ToPercentage(totalIncome)
                };
            }).ToList();
            monthlyBudgets.AddRange(mainCategories);
            var savingsAmount = budgets.Where(x => x.BudgetType == BudgetType.Savings).Sum(x => x.Amount);

            var adjustedSavings = (savingsAmount + mainCategories.Sum(x => x.Amount)) > totalIncome ?
                 (totalIncome - mainCategories.Sum(x => x.Amount)) : savingsAmount;

            var savings = new BudgetBreakdown()
            {
                MainCategoryId = int.MaxValue - 1,
                SubCategoryId = 0,
                Name = "Savings",
                Amount = adjustedSavings,
                Percentage = adjustedSavings > 0 ? savingsAmount.ToPercentage(totalIncome) : 0
            };

            if (savings.Amount != 0)
                monthlyBudgets.Add(savings);

            var unallocatedAmount = subCategories.Sum(x => x.Amount) + savings.Amount >= totalIncome ? 0 : totalIncome - (subCategories.Sum(x => x.Amount) + savings.Amount);
            var unAllocated = new BudgetBreakdown()
            {
                Name = "Unallocated",
                MainCategoryId = int.MaxValue,
                SubCategoryId = 0,
                Amount = unallocatedAmount,
                Percentage = unallocatedAmount > 0 ? unallocatedAmount.ToPercentage(totalIncome) : 0
            };
            if (unAllocated.Amount > 0)
                monthlyBudgets.Add(unAllocated);

            return monthlyBudgets.OrderBy(x => x.MainCategoryId).ThenBy(x => x.SubCategoryId).ToList();
        }
        private async Task<List<BudgetListItem>> ParseBudgetListQuery(BudgetListRequest request)
        {
            var budgetListItems = new List<BudgetEntity>();
            switch (request.Order)
            {
                case BudgetOrderBy.Year:
                    var allBudgetsByYear = await _budgetRepo.Find(x => true);

                    budgetListItems = request.Reversed ?
                        allBudgetsByYear.OrderByDescending(x => x.Year).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                        allBudgetsByYear.OrderBy(x => x.Year).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                    break;
                case BudgetOrderBy.Month:
                    var allBudgetsByMonth = await _budgetRepo.Find(x => true);
                    var budgetsByMonth = _mapper.Map<List<BudgetListItem>>(allBudgetsByMonth);
                    budgetsByMonth = request.Reversed ?
                        budgetsByMonth.OrderByDescending(x => x.Month).ThenByDescending(x => x.SubCategory).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                        budgetsByMonth.OrderBy(x => x.Month).ThenBy(x => x.SubCategory).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                    return budgetsByMonth;
                case BudgetOrderBy.Amount:
                    var allBudgetsByAmount = await _budgetRepo.Find(x => true);

                    budgetListItems = request.Reversed ?
                        allBudgetsByAmount.OrderByDescending(x => x.Amount).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                        allBudgetsByAmount.OrderBy(x => x.Amount).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                    break;
                case BudgetOrderBy.SubCategory:
                    var allBudgetsBySubCategory = (await _budgetRepo.Find(x => true)).ToList();
                    var budgetsBySub = _mapper.Map<List<BudgetListItem>>(allBudgetsBySubCategory);
                    budgetsBySub = request.Reversed ?
                        budgetsBySub.OrderByDescending(x => x.SubCategory).ThenByDescending(x => x.MainCategory).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                        budgetsBySub.OrderBy(x => x.SubCategory).ThenBy(x => x.MainCategory).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                    return budgetsBySub;
                case BudgetOrderBy.MainCategory:
                    var allBudgetsByMainCategory = (await _budgetRepo.Find(x => true)).ToList();
                    var budgetsByMain = _mapper.Map<List<BudgetListItem>>(allBudgetsByMainCategory);
                    budgetsByMain = request.Reversed ?
                        budgetsByMain.OrderByDescending(x => x.MainCategory).ThenByDescending(x => x.SubCategory).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                        budgetsByMain.OrderBy(x => x.MainCategory).ThenBy(x => x.SubCategory).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                    return budgetsByMain;
                case BudgetOrderBy.Type:
                    var allBudgetsByType = await _budgetRepo.Find(x => true);

                    budgetListItems = request.Reversed ?
                        allBudgetsByType.OrderByDescending(x => x.BudgetType).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                        allBudgetsByType.OrderBy(x => x.BudgetType).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                    break;
            }
            return _mapper.Map<List<BudgetListItem>>(budgetListItems);
        }

        public class BudgetMapperProfile : Profile
        {
            public BudgetMapperProfile()
            {
                CreateMap<BudgetEntity, BudgetListItem>()
                    .ForMember(b => b.Id, o => o.MapFrom(src => src.Id))
                    .ForMember(b => b.Year, o => o.MapFrom(src => src.Year))
                    .ForMember(b => b.Month, o => o.MapFrom(src => src.Month))
                    .ForMember(b => b.SubCategory, o => o.MapFrom(src => src.SubCategoryId != null ? src.SubCategory.Name : string.Empty))
                    .ForMember(b => b.SubCategoryId, o => o.MapFrom(src => src.SubCategoryId != null ? src.SubCategory.Id : 0))
                    .ForMember(b => b.MainCategory, o => o.MapFrom(src => src.SubCategoryId != null ? src.SubCategory.MainCategory.Name : src.BudgetType.ToString()))
                    .ForMember(b => b.Type, o => o.MapFrom(src => src.BudgetType.ToString()))
                    .ForMember(b => b.Amount, o => o.MapFrom(src => src.Amount));

            }
        }
    }
}
