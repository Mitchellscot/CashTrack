using CashTrack.Models.MerchantModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Shouldly;
using Newtonsoft.Json;
using System.Net;

namespace CashTrack.IntegrationTests
{

    public class MerchantsEndpointsShould : IClassFixture<TestServerFixture>
    {
        private readonly TestServerFixture _fixture;
        private ITestOutputHelper _output;
        const string ENDPOINT = "api/merchants";

        public MerchantsEndpointsShould(TestServerFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }
        [Fact]
        public async Task ReturnAllMerchants()
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT);
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<MerchantResponse>(await response.Content.ReadAsStringAsync());
            _output.WriteLine(await response.Content.ReadAsStringAsync());
            responseObject.TotalPages.ShouldBeGreaterThan(19);
            responseObject.ListItems.Count().ShouldBe(25);
            responseObject.PageNumber.ShouldBe(1);
        }
        [Fact]
        public async Task ReturnAllMerchantsWithPagination()
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "?pageNumber=2&pageSize=50");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<MerchantResponse>(await response.Content.ReadAsStringAsync());
            _output.WriteLine(await response.Content.ReadAsStringAsync());
            responseObject.TotalPages.ShouldBeGreaterThan(9);
            responseObject.ListItems.Count().ShouldBe(50);
            responseObject.PageNumber.ShouldBe(2);
        }
        [Theory]
        [InlineData("Costco")]
        [InlineData("costco")]
        [InlineData("John")]
        [InlineData("Home")]
        public async Task ReturnMerchantsWithMatchingSearchTerm(string query)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + $"?query={query}");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<MerchantResponse>(await response.Content.ReadAsStringAsync());
            responseObject.ListItems.Count().ShouldBeGreaterThan(1);
            responseObject.ListItems.First().Name.ShouldContain(query);
            PrintRequestAndResponse(ENDPOINT + $"?query={query}", await response.Content.ReadAsStringAsync());
        }

        [Theory]
        [InlineData(17)]
        [InlineData(85)]
        [InlineData(350)]
        public async Task ReturnMerchantDetail(int id)
        {
            var expectedMerchantNames = new List<string>() { "Amazon", "Costco", "Walmart" };
            var response = await _fixture.Client.GetAsync(ENDPOINT + $"/detail/{id}");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<MerchantDetail>(await response.Content.ReadAsStringAsync());
            responseObject.ShouldNotBeNull();
            expectedMerchantNames.ShouldContain(responseObject.Name);
            responseObject.AnnualExpenseStatistics.ForEach(x => x.Average.ShouldBe(Math.Round(x.Total / x.Count, 2)));
            responseObject.AnnualExpenseStatistics.ForEach(x => x.Year.ShouldBeGreaterThan(2011));
            responseObject.AnnualExpenseStatistics.ForEach(x => x.Max.ShouldBeGreaterThan(0));
            responseObject.AnnualExpenseStatistics.ForEach(x => x.Min.ShouldBeGreaterThan(0));
            responseObject.ExpenseTotals.TotalSpentAllTime.ShouldBeGreaterThan(1);
            responseObject.MostUsedCategory.ShouldNotBeEmpty();
            responseObject.PurchaseCategoryOccurances.ShouldNotBeEmpty();
            responseObject.PurchaseCategoryTotals.ShouldNotBeEmpty();
            responseObject.RecentExpenses.Count.ShouldBeGreaterThan(0);
            PrintRequestAndResponse(ENDPOINT + $"/{id}", await response.Content.ReadAsStringAsync());
        }
        [Theory]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task ThrowExceptionWithInvalidId(int id)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + $"/detail/{id}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains($"No merchant found with an id of {id}", responseString);
            PrintRequestAndResponse(ENDPOINT + $"/99999999", await response.Content.ReadAsStringAsync());
        }
        [Fact]
        public async Task CreateAndDeleteMerchant()
        {
            var model = GetAddEditMerchant();
            var testId = 0;
            try
            {
                //create merchant
                var response = await _fixture.SendPostRequestAsync(ENDPOINT, model);
                response.StatusCode.ShouldBe(HttpStatusCode.Created);
                var responseObject = JsonConvert.DeserializeObject<AddEditMerchant>(await response.Content.ReadAsStringAsync());
                testId = responseObject.Id!.Value;
                response.Headers.Location!.AbsolutePath.ToLower().ShouldBe($"/merchants/detail/{responseObject.Id.ToString()}");
            }
            finally
            {
                //delete merchant
                var response = await _fixture.Client.DeleteAsync(ENDPOINT + $"/{testId}");
                response.StatusCode.ShouldBe(HttpStatusCode.OK);
            }

        }
        [Fact]
        public async Task UpdateAMerchant()
        {
            var modelToCreate = GetAddEditMerchant();
            var testId = 0;
            try
            {
                //create merchant
                var response = await _fixture.SendPostRequestAsync(ENDPOINT, modelToCreate);
                response.StatusCode.ShouldBe(HttpStatusCode.Created);
                var responseObject = JsonConvert.DeserializeObject<AddEditMerchant>(await response.Content.ReadAsStringAsync());
                testId = responseObject.Id!.Value;
                //update merchant
                var updatedObject = responseObject with
                {
                    Id = testId,
                    Name = $"TEST UPDATE {Guid.NewGuid()}",
                    City = "Baxter",
                    State = "MN",
                    SuggestOnLookup = false,
                    IsOnline = true,
                    Notes = "Updated with test method"
                };
                var updatedResponse = await _fixture.SendPutRequestAsync(ENDPOINT, updatedObject);
                var responseString = await updatedResponse.Content.ReadAsStringAsync();
                updatedResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            }
            finally
            {
                //delete merchant
                var response = await _fixture.Client.DeleteAsync(ENDPOINT + $"/{testId}");
                response.StatusCode.ShouldBe(HttpStatusCode.OK);
            }
        }
        [Fact]
        public async Task ThrowExceptionIfMerchantNameExists()
        {
            var model = new AddEditMerchant()
            {
                Id = null,
                Name = "Costco",
                City = "Long Beach",
                State = "CA",
                SuggestOnLookup = true,
                IsOnline = false,
                Notes = "Created with test method"
            };
            var response = await _fixture.SendPostRequestAsync(ENDPOINT, model);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        private AddEditMerchant GetAddEditMerchant()
        {
            var model = new AddEditMerchant()
            {
                Id = null,
                Name = $"TEST {Guid.NewGuid()}",
                City = "Long Beach",
                State = "CA",
                SuggestOnLookup = true,
                IsOnline = false,
                Notes = "Created with test method"
            };
            return model;
        }
        private void PrintRequestAndResponse(object request, object response)
        {
            _output.WriteLine(request.ToString());
            _output.WriteLine(response.ToString());
        }
    }
}
