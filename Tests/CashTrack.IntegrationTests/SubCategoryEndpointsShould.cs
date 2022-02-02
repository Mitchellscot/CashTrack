using CashTrack.Models.SubCategoryModels;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CashTrack.IntegrationTests
{
    //need
    //tests for detail
    //test for sad path, exceptions (duplicate name, category not found)
    public class SubCategoryEndpointsShould : IClassFixture<TestServerFixture>
    {
        private readonly TestServerFixture _fixture;
        private readonly ITestOutputHelper _output;
        const string ENDPOINT = "api/subcategory";

        public SubCategoryEndpointsShould(TestServerFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }
        [Fact]
        public async Task ReturnAllCategories()
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT);
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<SubCategoryResponse>(await response.Content.ReadAsStringAsync());
            _output.WriteLine(await response.Content.ReadAsStringAsync());
            responseObject.TotalPages.ShouldBeGreaterThan(1);
            responseObject.TotalCount.ShouldBe(73);
            responseObject.PageNumber.ShouldBe(1);
            responseObject.ListItems.ShouldNotBeEmpty<SubCategoryListItem>();
        }
        [Fact]
        public async Task ReturnAllCategoriesWithPagination()
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "?pageNumber=2&pageSize=50");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<SubCategoryResponse>(await response.Content.ReadAsStringAsync());
            _output.WriteLine(await response.Content.ReadAsStringAsync());
            responseObject.TotalPages.ShouldBeGreaterThan(1);
            responseObject.TotalCount.ShouldBe(73);
            responseObject.PageNumber.ShouldBe(2);
            responseObject.ListItems.ShouldNotBeEmpty<SubCategoryListItem>();
        }
        [Theory]
        [InlineData("doc")]
        [InlineData("soft")]
        [InlineData("car")]
        public async Task ReturnSubCategoriesWithMatchingSearchTerm(string searchTerm)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + $"?searchterm={searchTerm}");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<SubCategoryResponse>(await response.Content.ReadAsStringAsync());
            _output.WriteLine(await response.Content.ReadAsStringAsync());
            responseObject.TotalPages.ShouldBeGreaterThanOrEqualTo(1);
            responseObject.TotalCount.ShouldBeGreaterThan(1);
            responseObject.ListItems.ShouldNotBeEmpty<SubCategoryListItem>();
        }
        [Fact]
        public async Task CreateUpdateDeleteSubCategories()
        {
            var testId = 0;
            try
            {
                //create
                var uniqueName = Guid.NewGuid().ToString();
                var request = new AddEditSubCategory() { Name = uniqueName, InUse = true, MainCategoryId = 12 };
                var response = await _fixture.SendPostRequestAsync(ENDPOINT, request);
                response.StatusCode.ShouldBe(HttpStatusCode.Created);
                var responseObject = JsonConvert.DeserializeObject<AddEditSubCategory>(await response.Content.ReadAsStringAsync());
                testId = responseObject.Id!.Value;
                response.Headers.Location!.ToString().ShouldContain(responseObject.Id.ToString()!);
                responseObject.Name.ShouldBe(uniqueName);
                //update
                var updatedObject = new AddEditSubCategory() { Id = testId, Name = Guid.NewGuid().ToString(), MainCategoryId = 12, InUse = false };
                var updatedResponse = await _fixture.SendPutRequestAsync(ENDPOINT, updatedObject);
                updatedResponse.EnsureSuccessStatusCode();
            }
            finally
            {
                //delete
                var deleteResponse = await _fixture.Client.DeleteAsync(ENDPOINT + $"/{testId}");
                deleteResponse.EnsureSuccessStatusCode();
            }
        }
        [Fact]
        public async Task ErrorWithDuplicateNameOnCreate()
        {
            var request = new AddEditSubCategory() { Name = "AAA" };
            var response = await _fixture.SendPostRequestAsync(ENDPOINT, request);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task ErrorWithDuplicateNameOnUpdate()
        {
            var request = new AddEditSubCategory() { Id = 1, Name = "AAA" };
            var response = await _fixture.SendPutRequestAsync(ENDPOINT, request);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task ErrorWithWrongIdOnUpdate()
        {
            var request = new AddEditSubCategory() { Id = int.MaxValue, Name = "z" };
            var response = await _fixture.SendPutRequestAsync(ENDPOINT, request);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task ErrorWithWrongIdOnDelete()
        {
            var response = await _fixture.Client.DeleteAsync(ENDPOINT + $"/{int.MaxValue}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}
