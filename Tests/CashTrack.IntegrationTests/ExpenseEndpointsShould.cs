using CashTrack.IntegrationTests.Common;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Shouldly;
using Newtonsoft.Json;
using CashTrack.Models.ExpenseModels;
using System.Net;

namespace CashTrack.IntegrationTests
{
    public class ExpenseEndpointsShould : IClassFixture<TestServerFixture>
    {
        private TestServerFixture _fixture;
        private ITestOutputHelper _output;
        const string ENDPOINT = "/api/expense";

        public ExpenseEndpointsShould(TestServerFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }
        #region Single Expense
        [Theory]
        [ExpenseIdData]
        public async Task ReturnASingleExpense(string id)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/detail/" + id);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            _output.WriteLine(responseString);

            Assert.Contains($"\"id\":{id}", responseString);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task ErrorWithInvalidId(int id)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/detail/" + id);
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

            var responseString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseString);

            Assert.Contains($"No expense found with an id of {id}", responseString);
        }
        #endregion

        #region Multiple Expenses
        [Fact]
        public async Task ReturnAllExpenses()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

            var response = await _fixture.Client.GetAsync(ENDPOINT + "?dateoptions=1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.ShouldContain("totalCount\":7276,\"");
            responseString.ShouldContain("totalPages\":292,\"");
            var responseObject = JsonConvert.DeserializeObject<ExpenseResponse>(await response.Content.ReadAsStringAsync(), settings);
            _output.WriteLine(responseObject.ToString());
            //running into issues deserializing inherited objects
            //responseObject.TotalPages.ShouldBeGreaterThan(287);
            //responseObject.TotalCount.ShouldBeGreaterThan(7000);
        }
        [Theory]
        [InlineData("2016-02-14")]
        [InlineData("2020-01-01")]
        [InlineData("2012-01-01")]
        public async Task ReturnsExpensesFromAGivenDate(string date)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=2&beginDate={date}");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ExpenseResponse>(await response.Content.ReadAsStringAsync());
            _output.WriteLine(responseObject.ToString());
            //responseObject.PageNumber.ShouldBeGreaterThan(0);
            responseObject.ListItems.Count().ShouldBeGreaterThan(0);
            //responseObject.TotalCount.ShouldBeGreaterThan(1);
        }
        [Theory]
        [InlineData("2016-02-16")]
        [InlineData("2021-01-01")]
        [InlineData("2012-04-24")]
        public async Task ReturnsExpensesForAGivenMonth(string date)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=3&beginDate={date}");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<ExpenseResponse>(await response.Content.ReadAsStringAsync());
            responseObject.PageNumber.ShouldBeGreaterThan(0);
            responseObject.ListItems.Count().ShouldBeGreaterThan(0);

            var expenseList = responseObject.ListItems.ToList();
            var testMonth = DateTime.Parse(date).Month;
            foreach (var exp in expenseList)
            {
                exp.Date.Month.ShouldBe(testMonth);
            }
            _output.WriteLine(responseObject.ToString());
        }
        [Theory]
        [InlineData("2018-02-16")]
        [InlineData("2014-01-01")]
        [InlineData("2013-04-24")]
        public async Task ReturnsExpensesForAGivenQuarter(string date)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + $"?dateoptions=4&beginDate={date}");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<ExpenseResponse>(await response.Content.ReadAsStringAsync());
            responseObject.PageNumber.ShouldBeGreaterThanOrEqualTo(1);
            responseObject.ListItems.Count().ShouldBeGreaterThan(1);

            var expenseList = responseObject.ListItems.ToList();
            var testYear = DateTime.Parse(date).Year;
            foreach (var exp in expenseList)
            {
                exp.Date.Year.ShouldBe(testYear);
            }
            _output.WriteLine(responseObject.ToString());
        }
        [Theory]
        [InlineData("2019-02-16")]
        [InlineData("2017-01-01")]
        [InlineData("2015-04-24")]
        public async Task ReturnsExpensesForAGivenYear(string date)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=5&beginDate={date}");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<ExpenseResponse>(await response.Content.ReadAsStringAsync());
            responseObject.PageNumber.ShouldBeGreaterThanOrEqualTo(1);
            responseObject.ListItems.Count().ShouldBeGreaterThanOrEqualTo(1);

            var expenseList = responseObject.ListItems.ToList();
            var testYear = DateTime.Parse(date).Year;
            foreach (var exp in expenseList)
            {
                exp.Date.Year.ShouldBe(testYear);
            }
            _output.WriteLine(responseObject.ToString());
        }
        [Theory]
        [InlineData("2012-03-01", "2012-03-14")]
        [InlineData("2016-11-03", "2021-01-06")]
        [InlineData("2015-04-24", "2016-04-24")]
        public async Task ReturnsExpensesForAGivenDateRange(string beginDate, string endDate)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=6&beginDate={beginDate}&endDate={endDate}");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<ExpenseResponse>(await response.Content.ReadAsStringAsync());
            responseObject.PageNumber.ShouldBeGreaterThanOrEqualTo(1);
            responseObject.ListItems.Count().ShouldBeGreaterThan(1);

            var expenseList = responseObject.ListItems.ToList();
            foreach (var exp in expenseList)
            {
                exp.Date.ShouldBeInRange(DateTimeOffset.Parse(beginDate), DateTimeOffset.Parse(endDate));
            }
            _output.WriteLine(responseObject.ToString());
        }
        [Fact(Skip = "None in the database")]
        public async Task ReturnsExpensesFromLast30Days()
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=7");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<ExpenseResponse>(await response.Content.ReadAsStringAsync());
            responseObject.PageNumber.ShouldBeGreaterThanOrEqualTo(1);
            responseObject.ListItems.Count().ShouldBeGreaterThanOrEqualTo(1);

            var expenseList = responseObject.ListItems.ToList();
            foreach (var exp in expenseList)
            {
                exp.Date.ShouldBeGreaterThan(DateTimeOffset.Now.AddDays(-31));
            }
            _output.WriteLine(responseObject.ToString());
        }
        [Fact(Skip = "No expenses entered this month")]
        public async Task ReturnsExpensesForThisMonth()
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=8");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<ExpenseResponse>(await response.Content.ReadAsStringAsync());
            responseObject.PageNumber.ShouldBeGreaterThanOrEqualTo(1);
            responseObject.ListItems.Count().ShouldBeGreaterThan(0);

            var expenseList = responseObject.ListItems.ToList();
            var thisMonth = DateTimeOffset.Now.Month;
            foreach (var exp in expenseList)
            {
                exp.Date.Month.ShouldBeEquivalentTo(thisMonth);
            }
            _output.WriteLine(responseObject.ToString());
        }
        [Fact(Skip = "No expenses this quarter")]
        public async Task ReturnsExpensesForThisQuarter()
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=9");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<ExpenseResponse>(await response.Content.ReadAsStringAsync());
            responseObject.PageNumber.ShouldBeGreaterThanOrEqualTo(1);
            responseObject.ListItems.Count().ShouldBeGreaterThan(0);

            var expenseList = responseObject.ListItems.ToList();
            var thisYear = DateTimeOffset.Now.Year;
            foreach (var exp in expenseList)
            {
                exp.Date.Year.ShouldBeEquivalentTo(thisYear);
            }
            _output.WriteLine(responseObject.ToString());
        }
        [Fact(Skip = "No expenses this Year")]
        public async Task ReturnsExpensesForThisYear()
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=10");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<ExpenseResponse>(await response.Content.ReadAsStringAsync());
            responseObject.PageNumber.ShouldBeGreaterThanOrEqualTo(1);
            responseObject.ListItems.Count().ShouldBeGreaterThan(0);

            var expenseList = responseObject.ListItems.ToList();
            var thisYear = DateTimeOffset.Now.Year;
            foreach (var exp in expenseList)
            {
                exp.Date.Year.ShouldBeEquivalentTo(thisYear);
            }
            _output.WriteLine(responseObject.ToString());
        }
        [Theory]
        [InlineData("2015-04-24", "2016-04-245")]
        [InlineData("2016-04-24", "mitchell")]
        [InlineData("2017-04-24", "2185138642")]

        public async Task ErrorWhenDateRangeSearchDoesntHaveEndDate(string beginDate, string endDate)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=6&beginDate={beginDate}&endDate={endDate}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.ShouldContain("EndDate");
            _output.WriteLine(responseString);
        }

        [Theory]
        [InlineData("2016-02-142")]
        [InlineData("Mitchell")]
        [InlineData("2185138642")]
        [EmptyData]
        public async Task ErrorWhenDateSearchDoesntHaveADate(string date)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=2&beginDate={date}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.ShouldContain("BeginDate");
            _output.WriteLine(responseString);
        }
        [Theory]
        [InlineData("2016-02-142")]
        [InlineData("Mitchell")]
        [InlineData("2185138642")]
        [EmptyData]
        public async Task ErrorWhenMonthSearchDoesntHaveADate(string date)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=3&beginDate={date}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.ShouldContain("BeginDate");
            _output.WriteLine(responseString);
        }
        [Theory]
        [InlineData("-1")]
        [InlineData("13")]
        [InlineData("#")]
        [EmptyData]
        public async Task ErrorWhenDateOptionsIsntValid(string invalidOptions)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions={invalidOptions}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var responseString = await response.Content.ReadAsStringAsync();
            responseString.ShouldContain("DateOptions");
            _output.WriteLine(responseString);
        }
        [Theory]
        [InlineData("-100")]
        [InlineData("-4")]
        [InlineData("101")]
        [InlineData("Q")]
        [InlineData("$")]
        [EmptyData]
        public async Task ErrorWhenPageSizeIsntValid(string invalidPageSize)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=1&pageSize={invalidPageSize}");
            var responseString = await response.Content.ReadAsStringAsync();

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            responseString.ShouldContain("PageSize");
            _output.WriteLine(responseString);
        }
        [Theory]
        [InlineData("-100")]
        [InlineData("0")]
        [InlineData("Q")]
        [InlineData("$")]
        [EmptyData]
        public async Task ErrorWhenPageNumberIsntValid(string invalidPageNumber)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=1&pageNumber={invalidPageNumber}");
            var responseString = await response.Content.ReadAsStringAsync();

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            responseString.ShouldContain("PageNumber");
            _output.WriteLine(responseString);
        }
        [Theory]
        [InlineData("1984-04-24")]
        [InlineData("2984-04-24")]
        [InlineData("Mitchell")]
        [InlineData("218-513-8642")]
        [EmptyData]
        public async Task ErrorWhenBeginDateIsntValid(string invalidDate)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=1&beginDate={invalidDate}");
            var responseString = await response.Content.ReadAsStringAsync();

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            responseString.ShouldContain("BeginDate");
            _output.WriteLine(responseString);
        }
        [Theory]
        [InlineData("1984-04-24")]
        [InlineData("2984-04-24")]
        [InlineData("Mitchell")]
        [InlineData("218-513-8642")]
        [EmptyData]
        public async Task ErrorWhenEndDateIsntValid(string invalidDate)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=1&endDate={invalidDate}");
            var responseString = await response.Content.ReadAsStringAsync();

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            responseString.ShouldContain("EndDate");
            _output.WriteLine(responseString);
        }
        [Theory]
        [InlineData("Henry")]
        [InlineData("Sarah")]
        [InlineData("Lydia")]
        public async Task ReturnsExpensesForAGivenSearchTerm(string query)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + $"/notes?query={query}");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ExpenseResponse>(await response.Content.ReadAsStringAsync());
            _output.WriteLine(responseObject.ToString());
            //responseObject.PageNumber.ShouldBeGreaterThan(0);
            responseObject.ListItems.Count().ShouldBeGreaterThan(0);
            //responseObject.TotalCount.ShouldBeGreaterThan(1);
        }
        [Theory]
        [EmptyData]
        public async Task ErrorWhenSearchTermIsntValid(string invalidSearch)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + $"/notes?query={invalidSearch}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var responseString = await response.Content.ReadAsStringAsync();
            responseString.ShouldContain("search term");
            _output.WriteLine(responseString);
        }
        [Theory]
        [InlineData(25)]
        [InlineData(25.5)]
        [InlineData(25.5011111)]
        public async Task ReturnsExpensesForAGivenSearchAmount(decimal query)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + $"/amount?query={query}");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            var responseObject = JsonConvert.DeserializeObject<ExpenseResponse>(await response.Content.ReadAsStringAsync());
            _output.WriteLine(responseObject.ToString());
            //running into issues deserializing inherited objects
            //responseObject.PageNumber.ShouldBeGreaterThan(0);
            responseObject.ListItems.Count().ShouldBeGreaterThan(0);
            //responseObject.TotalCount.ShouldBeGreaterThan(1);
        }
        [Theory]
        [InlineData(0.00)]
        [InlineData(-25.000000000033)]
        public async Task ErrorWhenSearchAmountIsntValid(decimal query)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + $"/amount?query={query}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var responseString = await response.Content.ReadAsStringAsync();
            responseString.ShouldContain("query");
            _output.WriteLine(responseString);
        }

        #endregion
        #region Create Update Delete
        [Fact]
        public async Task CreateUpdateDeleteAnExpense()
        {
            var testId = 0;
            try
            {
                var model = GetAddEditExpense();
                //Create
                var createResponse = await _fixture.SendPostRequestAsync(ENDPOINT, model);
                createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
                var createResponseObject = JsonConvert.DeserializeObject<AddEditExpense>(await createResponse.Content.ReadAsStringAsync());
                createResponse.Headers.Location!.AbsolutePath.ToLower().ShouldBe($"/expense/detail/{createResponseObject.Id.ToString()}");
                testId = createResponseObject.Id!.Value;

                //Update


                createResponseObject.Id = createResponseObject.Id.Value;
                createResponseObject.Notes = "UPDATE";
                createResponseObject.Date = DateTimeOffset.UtcNow;
                createResponseObject.Amount = 5.00m;
                createResponseObject.SubCategoryId = 31;

                var updateResponse = await _fixture.SendPutRequestAsync(ENDPOINT, createResponseObject);
                var responseString = await updateResponse.Content.ReadAsStringAsync();
                updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            }
            finally
            {
                //delete
                var deleteResponse = await _fixture.Client.DeleteAsync(ENDPOINT + $"/{testId}");
                deleteResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            }
        }
        [Theory]
        [InlineData(-25.00)]
        [InlineData(0)]
        public async Task ErrorWhenAddingExpenseWithInvalidAmount(decimal invalidAmount)
        {
            var expense = GetAddEditExpense();
            expense.Amount = invalidAmount;
            var response = await _fixture.SendPostRequestAsync(ENDPOINT, expense);
            var responseString = await response.Content.ReadAsStringAsync();

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            responseString.ShouldContain(nameof(AddEditExpense.Amount));
            _output.WriteLine(responseString);
        }
        [Theory]
        [InlineData(-25)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        public async Task ErrorWhenAddingExpenseWithInvalidMerchantId(int invalidMerchant)
        {
            var expense = GetAddEditExpense();
            expense.MerchantId = invalidMerchant;
            var response = await _fixture.SendPostRequestAsync(ENDPOINT, expense);
            var responseString = await response.Content.ReadAsStringAsync();

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            responseString.ShouldContain(nameof(AddEditExpense.MerchantId));
            _output.WriteLine(responseString);
        }
        [Theory]
        [InlineData("2984-04-24")]
        public async Task ErrorWhenAddingExpenseWithInvalidPurchaseDate(DateTimeOffset invalidDate)
        {
            var expense = GetAddEditExpense();
            expense.Date = invalidDate;
            var response = await _fixture.SendPostRequestAsync(ENDPOINT, expense);
            var responseString = await response.Content.ReadAsStringAsync();

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            responseString.ShouldContain(nameof(AddEditExpense.Date));
            _output.WriteLine(responseString);
        }
        #endregion
        private void PrintRequestAndResponse(object request, object response)
        {
            _output.WriteLine(request.ToString());
            _output.WriteLine(response.ToString());
        }
        private AddEditExpense GetAddEditExpense()
        {
            return new AddEditExpense()
            {
                Date = DateTimeOffset.UtcNow,
                Amount = 25.00m,
                SubCategoryId = 31
            };
        }
    }
}
