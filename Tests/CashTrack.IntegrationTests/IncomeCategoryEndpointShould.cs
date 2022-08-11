//using CashTrack.Models.IncomeCategoryModels;
//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;
//using Xunit.Abstractions;
//using Shouldly;
//using Newtonsoft.Json;
//using System.Net;

//namespace CashTrack.IntegrationTests
//{
//    public class IncomeCategoryEndpointShould : IClassFixture<TestServerFixture>
//    {
//        private readonly TestServerFixture _fixture;
//        private readonly ITestOutputHelper _output;
//        const string ENDPOINT = "api/incomecategory";
//        public IncomeCategoryEndpointShould(TestServerFixture fixture, ITestOutputHelper output)
//        {
//            _fixture = fixture;
//            _output = output;
//        }
//        [Fact]
//        public async Task ReturnAllIncomeCategories()
//        {
//            var response = await _fixture.Client.GetAsync(ENDPOINT);
//            response.EnsureSuccessStatusCode();
//            var responseObject = JsonConvert.DeserializeObject<IncomeCategoryResponse>(await response.Content.ReadAsStringAsync());
//            _output.WriteLine(await response.Content.ReadAsStringAsync());
//            responseObject.TotalCount.ShouldBe(14);
//            var categoryNumber = responseObject.ListItems.Count();
//            responseObject.ListItems.Count().ShouldBe(categoryNumber);
//            responseObject.PageNumber.ShouldBe(1);
//        }
//        [Fact]
//        public async Task SearchBySearchTerm()
//        {
//            var response = await _fixture.Client.GetAsync(ENDPOINT + $"?query=bonus");
//            response.EnsureSuccessStatusCode();
//            var responseObject = JsonConvert.DeserializeObject<IncomeCategoryResponse>(await response.Content.ReadAsStringAsync());
//            _output.WriteLine(await response.Content.ReadAsStringAsync());
//            responseObject.ListItems.Count().ShouldBe(1);
//            responseObject.ListItems.FirstOrDefault()!.Name.ShouldBe("Bonus");
//        }
//        [Fact]
//        public async Task CreateUpdateDeleteIncomeCategory()
//        {
//            //refactor if you use an in memory database in the future
//            var testId = 0;
//            try
//            {
//                //create
//                var uniqueName = Guid.NewGuid().ToString();
//                var request = new AddEditIncomeCategory();
//                request.Name = uniqueName;
//                var response = await _fixture.SendPostRequestAsync(ENDPOINT, request);
//                response.StatusCode.ShouldBe(HttpStatusCode.Created);
//                var responseObject = JsonConvert.DeserializeObject<AddEditIncomeCategory>(await response.Content.ReadAsStringAsync());
//                testId = responseObject.Id!.Value;
//                response.Headers.Location!.ToString().ShouldContain(responseObject.Id.ToString()!);
//                responseObject.Name.ShouldBe(uniqueName);
//                responseObject.Id.ShouldNotBeNull();
//                //update
//                var updateObject = new AddEditIncomeCategory()
//                {
//                    Id = testId,
//                    Name = Guid.NewGuid().ToString()
//                };
//                var updateResponse = await _fixture.SendPutRequestAsync(ENDPOINT, updateObject);
//                updateResponse.EnsureSuccessStatusCode();
//            }
//            finally
//            {
//                //delete
//                var deleteResponse = await _fixture.Client.DeleteAsync(ENDPOINT + $"/{testId}");
//                deleteResponse.EnsureSuccessStatusCode();
//            }
//        }
//        [Fact]
//        public async Task ErrorWithDuplicateNameOnCreate()
//        {
//            var request = new AddEditIncomeCategory() { Name = "Paycheck" };
//            var response = await _fixture.SendPostRequestAsync(ENDPOINT, request);
//            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
//        }
//        [Fact]
//        public async Task ErrorWithDuplicateNameOnUpdate()
//        {
//            var request = new AddEditIncomeCategory() { Id = 1, Name = "Paycheck" };
//            var response = await _fixture.SendPutRequestAsync(ENDPOINT, request);
//            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
//        }
//        [Fact]
//        public async Task ErrorWithWrongIdOnUpdate()
//        {
//            var request = new AddEditIncomeCategory() { Id = int.MaxValue, Name = "Paycheck" };
//            var response = await _fixture.SendPutRequestAsync(ENDPOINT, request);
//            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
//        }
//        [Fact]
//        public async Task ErrorWithWrongIdOnDelete()
//        {
//            var response = await _fixture.Client.DeleteAsync(ENDPOINT + $"/{int.MaxValue}");
//            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
//        }
//    }
//}
