using CashTrack.IntegrationTests.Common;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Shouldly;
using Newtonsoft.Json;
using CashTrack.Models.IncomeModels;
using System.Net;

namespace CashTrack.IntegrationTests
{
    public class IncomeEndpointsShould : IClassFixture<TestServerFixture>
    {
        private TestServerFixture _fixture;
        private ITestOutputHelper _output;
        const string ENDPOINT = "/api/Income";

        public IncomeEndpointsShould(TestServerFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }
        #region Single Income
        [Theory]
        [InlineData("1")]
        [InlineData("50")]
        [InlineData("150")]
        [InlineData("250")]
        [InlineData("650")]
        public async Task ReturnASingleIncome(string id)
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
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var responseString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseString);

            Assert.Contains($"No Income found with an id of {id}", responseString);
        }
        //[Theory]
        //[InlineData("%")]
        //[InlineData("A")]
        //[EmptyData]
        //public async Task ErrorWithInvalidInput(object input)
        //{
        //    var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + input);
        //    response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        //    PrintRequestAndResponse(input,
        //        JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()));
        //}
        #endregion

        #region Multiple Incomes
        [Fact]
        public async Task ReturnAllIncomes()
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "?dateoptions=1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<IncomeResponse>(await response.Content.ReadAsStringAsync());
            _output.WriteLine(responseObject.ToString());
            responseObject.ListItems.Count().ShouldBe(25);
            //having issues deserializing abstract inherited classes
            //responseObject.TotalPages.ShouldBeGreaterThan(29);
            //responseObject.TotalCount.ShouldBeGreaterThan(744);
        }
        [Theory]
        [InlineData("2017-08-11")]
        [InlineData("2014-08-07")]
        [InlineData("2016-06-13")]
        public async Task ReturnsIncomesFromAGivenDate(string date)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + $"?dateoptions=2&beginDate={date}");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<IncomeResponse>(await response.Content.ReadAsStringAsync());
            _output.WriteLine(responseObject.ToString());
            //responseObject.PageNumber.ShouldBeGreaterThan(0);
            responseObject.ListItems.Count().ShouldBeGreaterThan(0);
            //responseObject.TotalCount.ShouldBe(1);
        }
        [Theory]
        [InlineData("2016-02-16")]
        [InlineData("2021-01-01")]
        [InlineData("2012-04-24")]
        public async Task ReturnsIncomesForAGivenMonth(string date)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=3&beginDate={date}");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<IncomeResponse>(await response.Content.ReadAsStringAsync());
            responseObject.PageNumber.ShouldBeGreaterThan(0);
            responseObject.ListItems.Count().ShouldBeGreaterThan(0);

            var IncomeList = responseObject.ListItems.ToList();
            var testMonth = DateTime.Parse(date).Month;
            foreach (var exp in IncomeList)
            {
                exp.Date.Month.ShouldBe(testMonth);
            }
            _output.WriteLine(responseObject.ToString());
        }
        [Theory]
        [InlineData("2018-02-16")]
        [InlineData("2014-01-01")]
        [InlineData("2016-04-24")]
        public async Task ReturnsIncomesForAGivenQuarter(string date)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + $"?dateoptions=4&beginDate={date}");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<IncomeResponse>(await response.Content.ReadAsStringAsync());
            responseObject.PageNumber.ShouldBeGreaterThanOrEqualTo(1);
            responseObject.ListItems.Count().ShouldBeGreaterThan(1);

            var IncomeList = responseObject.ListItems.ToList();
            var testYear = DateTime.Parse(date).Year;
            foreach (var exp in IncomeList)
            {
                exp.Date.Year.ShouldBe(testYear);
            }
            _output.WriteLine(responseObject.ToString());
        }
        [Theory]
        [InlineData("2019-02-16")]
        [InlineData("2017-01-01")]
        [InlineData("2015-04-24")]
        public async Task ReturnsIncomesForAGivenYear(string date)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=5&beginDate={date}");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<IncomeResponse>(await response.Content.ReadAsStringAsync());
            responseObject.PageNumber.ShouldBeGreaterThanOrEqualTo(1);
            responseObject.ListItems.Count().ShouldBeGreaterThanOrEqualTo(1);

            var IncomeList = responseObject.ListItems.ToList();
            var testYear = DateTime.Parse(date).Year;
            foreach (var exp in IncomeList)
            {
                exp.Date.Year.ShouldBe(testYear);
            }
            _output.WriteLine(responseObject.ToString());
        }
        [Theory]
        [InlineData("2012-03-01", "2012-03-14")]
        [InlineData("2016-11-03", "2021-01-06")]
        [InlineData("2015-04-24", "2016-04-24")]
        public async Task ReturnsIncomesForAGivenDateRange(string beginDate, string endDate)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=6&beginDate={beginDate}&endDate={endDate}");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<IncomeResponse>(await response.Content.ReadAsStringAsync());
            responseObject.PageNumber.ShouldBeGreaterThanOrEqualTo(1);
            responseObject.ListItems.Count().ShouldBeGreaterThan(1);

            var IncomeList = responseObject.ListItems.ToList();
            foreach (var exp in IncomeList)
            {
                exp.Date.ShouldBeInRange(DateTimeOffset.Parse(beginDate), DateTimeOffset.Parse(endDate));
            }
            _output.WriteLine(responseObject.ToString());
        }
        [Fact(Skip = "None in the database")]
        public async Task ReturnsIncomesFromLast30Days()
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=7");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<IncomeResponse>(await response.Content.ReadAsStringAsync());
            responseObject.PageNumber.ShouldBeGreaterThanOrEqualTo(1);
            responseObject.ListItems.Count().ShouldBeGreaterThanOrEqualTo(1);

            var IncomeList = responseObject.ListItems.ToList();
            foreach (var exp in IncomeList)
            {
                exp.Date.ShouldBeGreaterThan(DateTimeOffset.Now.AddDays(-31));
            }
            _output.WriteLine(responseObject.ToString());
        }
        [Fact(Skip = "No Incomes entered this month")]
        public async Task ReturnsIncomesForThisMonth()
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=8");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<IncomeResponse>(await response.Content.ReadAsStringAsync());
            responseObject.PageNumber.ShouldBeGreaterThanOrEqualTo(1);
            responseObject.ListItems.Count().ShouldBeGreaterThan(0);

            var IncomeList = responseObject.ListItems.ToList();
            var thisMonth = DateTimeOffset.Now.Month;
            foreach (var exp in IncomeList)
            {
                exp.Date.Month.ShouldBeEquivalentTo(thisMonth);
            }
            _output.WriteLine(responseObject.ToString());
        }
        [Fact(Skip = "No Incomes this quarter")]
        public async Task ReturnsIncomesForThisQuarter()
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=9");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<IncomeResponse>(await response.Content.ReadAsStringAsync());
            responseObject.PageNumber.ShouldBeGreaterThanOrEqualTo(1);
            responseObject.ListItems.Count().ShouldBeGreaterThan(0);

            var IncomeList = responseObject.ListItems.ToList();
            var thisYear = DateTimeOffset.Now.Year;
            foreach (var exp in IncomeList)
            {
                exp.Date.Year.ShouldBeEquivalentTo(thisYear);
            }
            _output.WriteLine(responseObject.ToString());
        }
        [Fact(Skip = "No Incomes this Year")]
        public async Task ReturnsIncomesForThisYear()
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/" + $"?dateoptions=10");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<IncomeResponse>(await response.Content.ReadAsStringAsync());
            responseObject.PageNumber.ShouldBeGreaterThanOrEqualTo(1);
            responseObject.ListItems.Count().ShouldBeGreaterThan(0);

            var IncomeList = responseObject.ListItems.ToList();
            var thisYear = DateTimeOffset.Now.Year;
            foreach (var exp in IncomeList)
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
        //[Theory]
        //[InlineData("Henry")]
        //[InlineData("Sarah")]
        //[InlineData("Lydia")]
        //public async Task ReturnsIncomesForAGivenSearchTerm(string query)
        //{
        //    var response = await _fixture.Client.GetAsync(ENDPOINT + $"/notes?query={query}");
        //    response.EnsureSuccessStatusCode();
        //    var responseString = await response.Content.ReadAsStringAsync();
        //    var responseObject = JsonConvert.DeserializeObject<IncomeResponse>(await response.Content.ReadAsStringAsync());
        //    _output.WriteLine(responseObject.ToString());
        //    responseObject.PageNumber.ShouldBeGreaterThan(0);
        //    responseObject.ListItems.Count().ShouldBeGreaterThan(0);
        //    responseObject.TotalCount.ShouldBeGreaterThan(1);
        //}
        //[Theory]
        //[EmptyData]
        //public async Task ErrorWhenSearchTermIsntValid(string invalidSearch)
        //{
        //    var response = await _fixture.Client.GetAsync(ENDPOINT + $"/notes?query={invalidSearch}");
        //    response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        //    var responseString = await response.Content.ReadAsStringAsync();
        //    responseString.ShouldContain("search term");
        //    _output.WriteLine(responseString);
        //}
        //[Theory]
        //[InlineData(25)]
        //[InlineData(25.5)]
        //[InlineData(25.5011111)]
        //public async Task ReturnsIncomesForAGivenSearchAmount(decimal query)
        //{
        //    var response = await _fixture.Client.GetAsync(ENDPOINT + $"/amount?query={query}");
        //    response.EnsureSuccessStatusCode();
        //    var responseString = await response.Content.ReadAsStringAsync();
        //    var responseObject = JsonConvert.DeserializeObject<IncomeResponse>(await response.Content.ReadAsStringAsync());
        //    _output.WriteLine(responseObject.ToString());
        //    responseObject.PageNumber.ShouldBeGreaterThan(0);
        //    responseObject.ListItems.Count().ShouldBeGreaterThan(0);
        //    responseObject.TotalCount.ShouldBeGreaterThan(1);
        //}
        //[Theory]
        //[InlineData(0.00)]
        //[InlineData(-25.000000000033)]
        //public async Task ErrorWhenSearchAmountIsntValid(decimal query)
        //{
        //    var response = await _fixture.Client.GetAsync(ENDPOINT + $"/amount?query={query}");
        //    response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        //    var responseString = await response.Content.ReadAsStringAsync();
        //    responseString.ShouldContain("query");
        //    _output.WriteLine(responseString);
        //}

        #endregion
        #region Create Update Delete
        [Fact]
        public async Task CreateUpdateDeleteAnIncome()
        {
            var testId = 0;
            try
            {
                var model = GetAddEditIncome();
                //Create
                var createResponse = await _fixture.SendPostRequestAsync(ENDPOINT, model);
                createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
                var createResponseObject = JsonConvert.DeserializeObject<AddEditIncome>(await createResponse.Content.ReadAsStringAsync());
                createResponse.Headers.Location!.AbsolutePath.ToLower().ShouldBe($"/income/detail/{createResponseObject.Id.ToString()}");
                testId = createResponseObject.Id!.Value;

                //Update
                createResponseObject.Id = createResponseObject.Id.Value;
                createResponseObject.Notes = "UPDATE";
                createResponseObject.Date = DateTimeOffset.UtcNow;
                createResponseObject.Amount = 5.00m;
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
        public async Task ErrorWhenAddingIncomeWithInvalidAmount(decimal invalidAmount)
        {
            var Income = GetAddEditIncome();
            Income.Amount = invalidAmount;
            var response = await _fixture.SendPostRequestAsync(ENDPOINT, Income);
            var responseString = await response.Content.ReadAsStringAsync();

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            responseString.ShouldContain(nameof(AddEditIncome.Amount));
            _output.WriteLine(responseString);
        }
        [Theory]
        [InlineData(-25)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        public async Task ErrorWhenAddingIncomeWithInvalidSourceId(int invalidSource)
        {
            var Income = GetAddEditIncome();
            Income.SourceId = invalidSource;
            var response = await _fixture.SendPostRequestAsync(ENDPOINT, Income);
            var responseString = await response.Content.ReadAsStringAsync();

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            responseString.ShouldContain(nameof(AddEditIncome.SourceId));
            _output.WriteLine(responseString);
        }
        [Theory]
        [InlineData("2984-04-24")]
        public async Task ErrorWhenAddingIncomeWithInvalidIncomeDate(DateTimeOffset invalidDate)
        {
            var Income = GetAddEditIncome();
            Income.Date = invalidDate;
            var response = await _fixture.SendPostRequestAsync(ENDPOINT, Income);
            var responseString = await response.Content.ReadAsStringAsync();

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            responseString.ShouldContain(nameof(AddEditIncome.Date));
            _output.WriteLine(responseString);
        }
        #endregion
        private void PrintRequestAndResponse(object request, object response)
        {
            _output.WriteLine(request.ToString());
            _output.WriteLine(response.ToString());
        }
        private AddEditIncome GetAddEditIncome()
        {
            return new AddEditIncome()
            {
                Date = DateTimeOffset.UtcNow,
                Amount = 25.00m,
                CategoryId = 1,
                SourceId = 1
            };
        }
    }
}
