using CashTrack.Models.AuthenticationModels;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.ExpenseReviewModels;
using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Models.IncomeModels;
using CashTrack.Models.IncomeReviewModels;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Models.MerchantModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.IncomeSourceRepository;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using FluentValidation;
using System;
using System.Linq;

namespace CashTrack.Validators;

/* AUTHENTICATION */
public class AuthenticationValidator : AbstractValidator<AuthenticationModels.Request>
{
    public AuthenticationValidator()
    {
        RuleFor(a => a.Name).NotEmpty().WithMessage("What's your name again?").MaximumLength(25);
        RuleFor(a => a.Password).NotEmpty().WithMessage("Forget your password?").MaximumLength(50);
    }
}

/*  EXPENSES */
public class ExpenseValidators : AbstractValidator<Expense>
{
    public ExpenseValidators(ISubCategoryRepository _categoryRepo, IMerchantRepository _merchantRepo)
    {
        RuleFor(x => x.Amount).NotEmpty().GreaterThan(0);
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Date).Must(x => x < DateTime.Today.AddDays(1)).WithMessage("The Purchase Date cannot be in the future.");
        When(x => x.SubCategoryId > 0, () =>
        {
            RuleFor(x => x.SubCategoryId).MustAsync(async (model, value, _) =>
                {
                    return (await _categoryRepo.Find(x => x.Id == value)).Any();
                }).WithMessage("Invalid Category");
        });
        When(x => !string.IsNullOrEmpty(x.Merchant) && !x.CreateNewMerchant,
            () =>
            {
                RuleFor(x => x.Merchant).MustAsync(async (model, value, _) =>
                {
                    return (await _merchantRepo.Find(x => true)).Any(x => x.Name == value);
                }).WithMessage("Please Select a merchant from the list or create a new Merchant.");
            });
        When(x => !string.IsNullOrEmpty(x.Merchant) && x.CreateNewMerchant,
            () =>
            {
                RuleFor(x => x.Merchant).MustAsync(async (model, value, _) =>
                {
                    return !(await _merchantRepo.Find(x => true)).Any(x => x.Name == value);
                }).WithMessage("A merchant already exists with that name");
            });
    }
}
public class ExpenseRequestValidators : AbstractValidator<ExpenseRequest>
{
    public ExpenseRequestValidators(IExpenseRepository expenseRepository)
    {
        var earliestExpense = expenseRepository.Find(x => true).Result.OrderBy(x => x.Date).Select(x => x.Date).FirstOrDefault();
        When(x => x.DateOptions != 0, () =>
        {
            RuleFor(x => x.DateOptions).IsInEnum().NotEmpty().WithMessage("Date Options must be specificied. Valid options are 1 through 12.");
        });
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(5, 100);
        RuleFor(x => x.BeginDate).Must(beginDate => beginDate >= earliestExpense).WithMessage("There are no expenses available before that date.");
        RuleFor(x => x.BeginDate).Must(beginDate => beginDate < DateTime.Today.AddDays(1)).WithMessage("The Begin Date cannot be in the future.");
        RuleFor(x => x.EndDate).Must(endDate => endDate < DateTime.Today.AddDays(1)).WithMessage("You can't search future dates");
        RuleFor(x => x.EndDate).Must(endDate => endDate > earliestExpense).WithMessage($"The end date cannot be before {earliestExpense.DateTime.ToShortDateString()}.");
    }
}
public class ExpenseSearchAmountValidator : AbstractValidator<AmountSearchRequest>
{
    public ExpenseSearchAmountValidator()
    {
        RuleFor(x => x.Query).NotEmpty().GreaterThan(0);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(5, 100);
    }
}

/* MERCHANTS */
public class MerchantValidator : AbstractValidator<MerchantRequest>
{
    public MerchantValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(5, 100);
    }
}
public class AddEditMerchantValidator : AbstractValidator<Merchant>
{
    public AddEditMerchantValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
/* SUB CATEGORIES */
public class SubCategoryValidator : AbstractValidator<SubCategoryRequest>
{
    public SubCategoryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(5, 100);
    }
}

public class AddEditSubCategoryValidator : AbstractValidator<AddEditSubCategory>
{
    public AddEditSubCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(37); //the size of a GUID
    }
}
/* MAIN CATEGORY */
public class AddEditMainCategoryValidator : AbstractValidator<AddEditMainCategory>
{
    public AddEditMainCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(37);
    }
}
/* INCOME CATEGORY */
public class AddEditIncomeCategoryValidator : AbstractValidator<AddEditIncomeCategory>
{
    public AddEditIncomeCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(37);
    }
}
public class IncomeCategoryValidator : AbstractValidator<IncomeCategoryRequest>
{
    public IncomeCategoryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(5, 100);
    }
}

/* INCOME */
public class IncomeRequestValidators : AbstractValidator<IncomeRequest>
{
    public IncomeRequestValidators(IIncomeRepository incomeRepository)
    {
        var earliestIncome = incomeRepository.Find(x => true).Result.OrderBy(x => x.Date).Select(x => x.Date).FirstOrDefault();
        When(x => x.DateOptions != 0, () =>
        {
            RuleFor(x => x.DateOptions).IsInEnum().NotEmpty().WithMessage("Date Options must be specificied. Valid options are 1 through 12.");
        });
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(5, 100);
        RuleFor(x => x.BeginDate).Must(beginDate => beginDate >= earliestIncome).WithMessage("There is no income available before that date.");
        RuleFor(x => x.BeginDate).Must(beginDate => beginDate < DateTime.Today.AddDays(1)).WithMessage("The Begin Date cannot be in the future.");
        RuleFor(x => x.EndDate).Must(endDate => endDate < DateTime.Today.AddDays(1)).WithMessage("You can't search future dates");
        RuleFor(x => x.EndDate).Must(endDate => endDate > earliestIncome).WithMessage($"The end date cannot be before {earliestIncome.DateTime.ToShortDateString()}.");
    }
}
public class IncomeValidators : AbstractValidator<Income>
{
    public IncomeValidators(ISubCategoryRepository _categoryRepo, IIncomeSourceRepository _sourceRepo)
    {
        RuleFor(x => x.Amount).NotEmpty().GreaterThan(0);
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Date).Must(x => x < DateTime.Today.AddDays(1)).WithMessage("The Income Date cannot be in the future.");
        When(x => x.Category != null,
            () =>
            {
                RuleFor(x => x.Category).MustAsync(async (model, value, _) =>
                {
                    return (await _categoryRepo.Find(x => true)).Count() >= int.Parse(value);
                }).WithMessage("Invalid Category");
            });
        When(x => !string.IsNullOrEmpty(x.Source) && !x.CreateNewSource,
            () =>
            {
                RuleFor(x => x.Source).MustAsync(async (model, value, _) =>
                        {
                            return (await _sourceRepo.Find(x => true)).Any(x => x.Name == value);
                        }).WithMessage("Please Select an income source from the list or check \"Create New Source\"");
            });
        When(x => !string.IsNullOrEmpty(x.Source) && x.CreateNewSource,
            () =>
            {
                RuleFor(x => x.Source).MustAsync(async (model, value, _) =>
                        {
                            return !(await _sourceRepo.Find(x => true)).Any(x => x.Name == value);
                        }).WithMessage("An Income Source already exists with that name");

            });
    }

    /* Transactions To Review */
    public class ExpenseReviewValidator : AbstractValidator<ExpenseReviewRequest>
    {
        public ExpenseReviewValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(5, 100);
        }
    }
    public class IncomeReviewValidator : AbstractValidator<IncomeReviewRequest>
    {
        public IncomeReviewValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(5, 100);
        }
    }
}