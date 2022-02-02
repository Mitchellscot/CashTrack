using CashTrack.Models.MainCategoryModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Shouldly;
using Newtonsoft.Json;
using System.Net;

namespace CashTrack.IntegrationTests
{
    //need
    //tests for detail
    //test for sad path, exceptions (duplicate name, category not found)
    public class MainCategoryEndpointsShould : IClassFixture<TestServerFixture>
    {
        private readonly TestServerFixture _fixture;
        private readonly ITestOutputHelper _output;
        const string ENDPOINT = "api/maincategory";

        public MainCategoryEndpointsShould(TestServerFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }
        [Fact]
        public async Task ReturnAllMainCategories()
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT);
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<MainCategoryResponse>(await response.Content.ReadAsStringAsync());
            _output.WriteLine(await response.Content.ReadAsStringAsync());
            responseObject.TotalMainCategories.ShouldBeGreaterThan(15);
            var categoryNumber = responseObject.TotalMainCategories;
            responseObject.MainCategories.Count().ShouldBe(categoryNumber);
        }
        [Fact]
        public async Task SearchBySearchTerm()
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + $"?query=food");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<MainCategoryResponse>(await response.Content.ReadAsStringAsync());
            _output.WriteLine(await response.Content.ReadAsStringAsync());
            responseObject.MainCategories.Count().ShouldBe(1);
            responseObject.MainCategories.FirstOrDefault()!.Name.ShouldBe("Food");
        }
        [Fact]
        public async Task CreateUpdateDeleteMainCategory()
        {
            //refactor if you use an in memory database in the future
            var testId = 0;
            try
            {
                //create
                var uniqueName = Guid.NewGuid().ToString();
                var request = new AddEditMainCategory() { Name = uniqueName };
                var response = await _fixture.SendPostRequestAsync(ENDPOINT, request);
                response.StatusCode.ShouldBe(HttpStatusCode.Created);
                var responseObject = JsonConvert.DeserializeObject<AddEditMainCategory>(await response.Content.ReadAsStringAsync());
                testId = responseObject.Id!.Value;
                response.Headers.Location!.ToString().ShouldContain(responseObject.Id.ToString()!);
                responseObject.Name.ShouldBe(uniqueName);
                responseObject.Id.ShouldNotBeNull();
                //update
                var updateObject = new AddEditMainCategory()
                {
                    Id = testId,
                    Name = Guid.NewGuid().ToString()
                };
                var updateResponse = await _fixture.SendPutRequestAsync(ENDPOINT, updateObject);
                updateResponse.EnsureSuccessStatusCode();
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
            var request = new AddEditMainCategory();
            request.Name = "Food";
            var response = await _fixture.SendPostRequestAsync(ENDPOINT, request);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task ErrorWithDuplicateNameOnUpdate()
        {
            var request = new AddEditMainCategory();
            request.Id = 1;
            request.Name = "Food";
            var response = await _fixture.SendPutRequestAsync(ENDPOINT, request);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task ErrorWithWrongIdOnUpdate()
        {
            var request = new AddEditMainCategory() { Id = int.MaxValue, Name = "Food" };
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
