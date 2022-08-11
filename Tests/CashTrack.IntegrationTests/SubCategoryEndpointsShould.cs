//using CashTrack.Models.SubCategoryModels;
//using Newtonsoft.Json;
//using Shouldly;
//using System;
//using System.Net;
//using System.Threading.Tasks;
//using Xunit;
//using Xunit.Abstractions;

//namespace CashTrack.IntegrationTests
//{
//    //need
//    //tests for detail
//    //test for sad path, exceptions (duplicate name, category not found)
//    public class SubCategoryEndpointsShould : IClassFixture<TestServerFixture>
//    {
//        private readonly TestServerFixture _fixture;
//        private readonly ITestOutputHelper _output;
//        const string ENDPOINT = "api/subcategory";

//        public SubCategoryEndpointsShould(TestServerFixture fixture, ITestOutputHelper output)
//        {
//            _fixture = fixture;
//            _output = output;
//        }

//        [Theory]
//        [InlineData("doc")]
//        [InlineData("soft")]
//        [InlineData("car")]
//        public async Task ReturnSubCategoriesWithMatchingCategoryNames(string searchTerm)
//        {
//            var response = await _fixture.Client.GetAsync(ENDPOINT + $"?categoryName={searchTerm}");
//            response.EnsureSuccessStatusCode();
//            var responseObject = JsonConvert.DeserializeObject<SubCategoryResponse>(await response.Content.ReadAsStringAsync());
//            _output.WriteLine(await response.Content.ReadAsStringAsync());
//            responseObject.TotalPages.ShouldBeGreaterThanOrEqualTo(1);
//            responseObject.TotalCount.ShouldBeGreaterThan(1);
//            responseObject.ListItems.ShouldNotBeEmpty<SubCategoryListItem>();
//        }

//        [Fact]
//        public async Task ErrorWithWrongIdOnDelete()
//        {
//            var response = await _fixture.Client.DeleteAsync(ENDPOINT + $"/{int.MaxValue}");
//            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
//        }
//    }
//}
