using CashTrack.Services.SummaryService;
using Shouldly;
using System.Threading.Tasks;
using Xunit;
using CashTrack.Models.SummaryModels;
using CashTrack.Repositories.BudgetRepository;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Tests.Services.Common;
using System;
using CashTrack.Data;
using System.Linq;
using CashTrack.Data.Entities;
using CashTrack.Models.BudgetModels;
using System.Collections;
using System.Collections.Generic;

namespace CashTrack.Tests.Services
{
    public class SummaryServiceTests
    {
        private readonly SummaryService _service;
        private AppDbContext _db;
        public SummaryServiceTests()
        {
            var sharedDB = new AppDbContextFactory().CreateDbContext();
            var budgetRepo = new BudgetRepository(sharedDB);
            var expenseRepo = new ExpenseRepository(sharedDB);
            var incomeRepo = new IncomeRepository(sharedDB);
            _service = new SummaryService(budgetRepo, expenseRepo, incomeRepo);
            _db = sharedDB;
        }
        [Theory]
        [MemberData(nameof(MonthlySummaryData))]
        public void Can_Get_Monthly_Summary(int year, int month, decimal re, decimal ri, int bi, int bn, int bw, int bs, int rs, int u, int es)
        {
            var testExpenses = new ExpenseEntity[]
            {
                new ExpenseEntity() { Date=new DateTime(year, month, 1), Amount=re }
            };
            var testIncome = new IncomeEntity[]
            {
                new IncomeEntity() {Date=new DateTime(year, month, 1), Amount=ri }
            };
            var testBudgets = new BudgetEntity[]
            {
                new BudgetEntity(){ BudgetType=BudgetType.Need, Year=year, Month=month, Amount=bn },
                new BudgetEntity(){ BudgetType=BudgetType.Want, Year=year, Month=month, Amount=bw },
                new BudgetEntity(){ BudgetType=BudgetType.Income, Year=year, Month=month, Amount=bi },
                new BudgetEntity(){ BudgetType=BudgetType.Savings, Year=year, Month=month, Amount=bs },
            };

            var result = _service.GetMonthlySummary(testExpenses, testIncome, testBudgets, year, month);

            result.BudgetedIncome.ShouldBe(bi);
            result.BudgetedExpenses.ShouldBe(bn + bw);
            result.BudgetedSavings.ShouldBe(bs);
            result.RealizedIncome.ShouldBe(ri);
            result.RealizedExpenses.ShouldBe(re);
            result.RealizedSavings.ShouldBe(rs);
            result.Unspent.ShouldBe(u);
            result.EstimatedSavings.ShouldBe(es);
        }
        public static IEnumerable<object[]> MonthlySummaryData =>
            new List<object[]>
            {
                //in the past
                //happy path, perfectly budgeted everything
                new object[] {1999, 4, 90, 100, 0, 20, 70, 10, 10, 0, 10 },    
                //happy path - expenses and savings less than realized income so estimated savings up
                new object[] {1999, 4, 70, 100, 0, 30, 30, 10, 10, 20, 30},
                //Spent and saved more than I earned, so dipped into planned savings
                new object[] {1999, 4, 95, 100, 0, 70, 25, 10, 5, 0, 5},
                //Spent more than I earned, dipped into actual savings
                new object[] {1999, 4, 110, 100, 0, 70, 30, 10, 0, 0, -10},
                //current month
                //happy path, perfectly budgeted everything
                new object[] {DateTime.Now.Year, DateTime.Now.Month, 90, 0, 100, 20, 70, 10, 10, 0, 10  },
                //happy path - expenses and savings less than realized income so estimated savings up
                new object[] {DateTime.Now.Year, DateTime.Now.Month, 70, 0, 100, 30, 30, 10, 10, 20, 30 },
                //Spent and saved more than I earned, so dipped into planned savings
                new object[] {DateTime.Now.Year, DateTime.Now.Month, 95, 0, 100, 70, 25, 10, 5, 0, 5 },
                //Spent more than I earned, dipped into actual savings
                new object[] {DateTime.Now.Year, DateTime.Now.Month, 110, 0, 100, 70, 30, 10, 0, 0, -10 },
            };
    }
}
